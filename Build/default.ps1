#This build assumes the following directory structure
#
#   \build    	- This is where the project build scripts lives (this file)
#   \artifacts	- This folder is created if it is missing and contains output of the build
#	\tools		- This is where tools, utilities and executables are stored that the builds need
#	\packages	- Nuget packages will be installed in here
#
Framework "4.6"
FormatTaskName ("`n" + ("-"*25) + "[{0}]" + ("-"*25))
 
Properties {
	$CoverallsToken = $env:COVERALLS_REPO_TOKEN
	$NugetApiKey = $env:NUGET_API_KEY
	$NugetFeed = "https://staging.nuget.org" #/packages?replace=true"
	
	$RootDir = Resolve-Path ..
	$ArtifactsDir = "$RootDir\artifacts"
	$ToolsDir = "$RootDir\tools"
}

Task Default -depends Init, ApplyVersioning, Compile, Test, Package, Zip, ResetVersioning
Task DefaultAndCoveralls -depends Default, PushCoverage
Task Deploy  -depends Default, PushCoverage, PushPackage

Task Init {
    "(Re)create Artifacts directory"	
	Remove-Item $ArtifactsDir -Recurse -Force -ErrorAction SilentlyContinue
	New-Item $ArtifactsDir -ItemType directory | out-null
    
	"Clean solution"
    exec { msbuild "$RootDir\NodaMoney.sln" /t:Clean /p:Configuration="Release" /p:Platform="Any CPU" /maxcpucount /verbosity:minimal /nologo }
	
	Get-ChildItem $RootDir -Recurse -Include 'bin','obj','packages' | ForEach-Object { Remove-Item $_ -Recurse -Force;  Write-Host Deleted $_ }
}

Task CalculateVersion {
	$gitVersionExe = Join-Path $ToolsDir -ChildPath "GitVersion.exe"
	   	
	if(isAppVeyor) { exec { & $gitVersionExe /output buildserver } }

	$json = exec { & $gitVersionExe }		
	$versionInfo = $json -join "`n" | ConvertFrom-Json
		
	if ($versionInfo.Major -ne 0) {
		$script:AssemblyVersion =  $versionInfo.Major + ".0.0.0"
	} else {
		$script:AssemblyVersion = $versionInfo.MajorMinorPatch + ".0"
	}
	$script:AssemblyFileVersion = $versionInfo.MajorMinorPatch + "." + $versionInfo.BuildMetaData
	$script:InformationalVersion = $versionInfo.NuGetVersion	
	
	"Use '$script:AssemblyVersion' as AssemblyVersion"
	"Use '$script:AssemblyFileVersion' as AssemblyFileVersion" 
	"Use '$script:InformationalVersion' as InformationalVersion & NuGet version"	
}

Task ApplyVersioning -depends CalculateVersion {
	$assemblyInfo = "$RootDir\src\GlobalAssemblyInfo.cs"
	
	"Updating $assemblyInfo with versioning"
	(Get-Content $assemblyInfo ) | ForEach-Object {
        Foreach-Object { $_ -replace 'AssemblyVersion.+$', "AssemblyVersion(`"$script:AssemblyVersion`")]" } |
        Foreach-Object { $_ -replace 'AssemblyFileVersion.+$', "AssemblyFileVersion(`"$script:AssemblyFileVersion`")]" } |
        Foreach-Object { $_ -replace 'AssemblyInformationalVersion.+$', "AssemblyInformationalVersion(`"$script:InformationalVersion`")]" }
    } | Set-Content $assemblyInfo
}

Task ResetVersioning {
	$assemblyInfo = "$RootDir\src\GlobalAssemblyInfo.cs"
	
	"Updating $assemblyInfo to version zero"
	(Get-Content $assemblyInfo ) | ForEach-Object {
        Foreach-Object { $_ -replace 'AssemblyVersion.+$', "AssemblyVersion(`"0.0.0.0`")]" } |
        Foreach-Object { $_ -replace 'AssemblyFileVersion.+$', "AssemblyFileVersion(`"0.0.0.0`")]" } |
        Foreach-Object { $_ -replace 'AssemblyInformationalVersion.+$', "AssemblyInformationalVersion(`"0.0.0.0`")]" }
    } | Set-Content $assemblyInfo
}

Task RestoreNugetPackages {
	$nugetExe = Join-Path $ToolsDir -ChildPath "NuGet.exe"
	
	"Restore build packages"
	exec { & $nugetExe restore "$RootDir\build\packages.config" -SolutionDirectory $RootDir  }
	
	"Restore solution packages"
	exec { & $nugetExe restore "$RootDir\NodaMoney.sln" }
}

Task Compile -depends RestoreNugetPackages {
	$logger = if(isAppVeyor) { "/logger:C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" }
	
	exec { msbuild "$RootDir\NodaMoney.sln" /t:Build /p:Configuration="Release" /p:Platform="Any CPU" /maxcpucount /verbosity:minimal /nologo $logger }
}

Task Test -depends Compile {
	$openCoverExe = Resolve-Path "$rootDir\packages\OpenCover.*\tools\OpenCover.Console.exe"	
	$logger = if(isAppVeyor) { "Appveyor" } else { "trx" }
	$VsTestConsoleExe = if(isAppVeyor) { "vstest.console.exe" } else { "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" }
	
	# Get test assemblies
	$testAssembliesFiles = Get-ChildItem -Recurse $RootDir\tests\*\bin\Release\*Tests*.dll
	if ($testAssembliesFiles.Count -eq 0) {
		Throw "No test assemblies found!"
	} else {
		"Found test assemblies:"
		$testAssembliesFiles | ForEach-Object { Write-Output $_.Name }
		""
	}
	
	# join files paths into to one string
	$testAssembliesPaths = $testAssembliesFiles | ForEach-Object { "`"`"" + $_.FullName + "`"`"" } 
	$testAssemblies = [string]::Join(" ", $testAssembliesPaths)
	
	# vstest console doesn't have any option to change the output directory
	# so we need to change the working directory to the artifacts folder
	Push-Location $ArtifactsDir
	
	# Run OpenCover, which in turn will run VSTest
	exec {
		& $openCoverExe -register:user -target:$VsTestConsoleExe "-targetargs:$testAssemblies /Logger:$logger" "-filter:+[NodaMoney*]* -[NodaMoney.Tests]*" -output:"$ArtifactsDir\coverage.xml"
	}
	
	Pop-Location

	# move the .trx file back to artifacts directory
	Get-ChildItem  $ArtifactsDir\TestResults\*.trx | Select-Object -Last 1 | Move-Item -Destination $ArtifactsDir\MSTest.trx -Force
	Remove-Item $ArtifactsDir\TestResults -Recurse -Force -ErrorAction SilentlyContinue
}

Task PushCoverage -precondition { return $CoverallsToken } {
	$coverallsExe = Resolve-Path "$RootDir\packages\coveralls.net.*\tools\csmacnz.Coveralls.exe"
	
	"Pushing coverage to coveralls.io"
	if(isAppVeyor) { 
		exec {
			& $coverallsExe --opencover -i $ArtifactsDir\coverage.xml --repoToken $CoverallsToken --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_BUILD_NUMBER --serviceName appveyor
		}
	} else {
		exec { & $coverallsExe --opencover -i $ArtifactsDir\coverage.xml --repoToken $CoverallsToken }
	}
}

Task Package {
	$nugetExe = Join-Path $ToolsDir -ChildPath "NuGet.exe";
	
	$projectsToPackage = Get-ChildItem -File -Path $RootDir -Filter *.nuspec -Recurse | ForEach-Object { $_.FullName -replace "nuspec", "csproj" }
	
	foreach ($proj in $projectsToPackage) {
		exec { & $nugetExe pack $proj -OutputDirectory $ArtifactsDir }
	}
	
	#if(isAppVeyor) {
	#	Get-ChildItem $ArtifactsDir *.nupkg | ForEach-Object { Push-AppveyorArtifact ($_ | Resolve-Path).Path }
	#}
}

Task PushPackage -precondition { return $NugetApiKey } {
	$nugetExe = Join-Path $ToolsDir -ChildPath "NuGet.exe"			
	exec { & $nugetExe push "$ArtifactsDir\*.nupkg" $NugetApiKey -source $NugetFeed }
}

Task Zip {
	$7zExe = Join-Path $ToolsDir -ChildPath "7z.exe"
	
	exec {
		& $7zExe a -tzip "$ArtifactsDir\NodaMoney.zip" "$RootDir\src\NodaMoney.Serialization.AspNet\bin\Release\*" "$RootDir\README.md" "$RootDir\LICENSE.txt" -x!"*.CodeAnalysisLog.xml" -x!"*.lastcodeanalysissucceeded"
	}	
}

function isAppVeyor() {
	Test-Path -Path env:\APPVEYOR
}

function applyXslTransform($xmlFile, $xslFile, $outputFile) {
	Write-Output "XML File: $xmlFile"
	Write-Output "XSL File: $xslFile"
	Write-Output "Output File: $outputFile"
	
	$xslt = New-Object System.Xml.Xsl.XslCompiledTransform;
	$s = New-Object System.Xml.Xsl.XsltSettings $true, $true;
	$xslt.Load($xslFile, $s, $null);
	$xslt.Transform($xmlFile, $outputFile);

	Write-Output "XSL Transform completed."

	if(isAppVeyor) {
			Push-AppveyorArtifact $outputFile;
	}
}
