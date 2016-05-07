# This build assumes the following directory structure (https://gist.github.com/davidfowl/ed7564297c61fe9ab8140):
#   \build    	- Build customizations (custom msbuild files/psake/fake/albacore/etc) scripts
#   \artifacts	- Build outputs go here. Doing a build.cmd generates artifacts here (nupkgs, zips, etc.)
#	\docs		- Documentation stuff, markdown files, help files, etc
#	\lib		- Binaries which are linked to in the source but are not distributed through NuGet
#	\packages	- Nuget packages
#	\samples    - Sample projects
#	\src		- Main projects (the source code)
#	\tests      - Test projects
#	\tools		- Binaries which are used as part of the build script (e.g. test runners, external tools)

Framework "4.6"
FormatTaskName "`r`n`r`n-------- Executing {0} Task --------"
 
Properties {
	$CoverallsToken = $env:COVERALLS_REPO_TOKEN
	$NugetApiKey = $env:NUGET_API_KEY
	$NugetFeed = "https://staging.nuget.org" #/packages?replace=true"
	
	$RootDir = Resolve-Path ..
	$ArtifactsDir = "$RootDir\artifacts"
	$ToolsDir = "$RootDir\tools"	
	$NugetExe = Join-Path $ToolsDir -ChildPath "\NuGet*\nuget.exe"
}

Task Default -depends Init, Compile, Test, Package, Zip
Task DefaultAndCoveralls -depends Default, PushCoverage
Task Deploy -depends Default, PushCoverage, PushPackage

Task Init {
    "(Re)create Artifacts directory"	
	Remove-Item $ArtifactsDir -Recurse -Force -ErrorAction SilentlyContinue
	New-Item $ArtifactsDir -ItemType directory | out-null
    
	"Clean solution"
    exec { msbuild "$RootDir\NodaMoney.sln" /t:Clean /p:Configuration="Release" /p:Platform="Any CPU" /maxcpucount /verbosity:minimal /nologo }
}

Task CalculateVersion {
	$gitVersionExe = Join-Path $ToolsDir -ChildPath "\GitVersion*\GitVersion.exe"
	   	
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
	
	"AssemblyVersion      = '$script:AssemblyVersion'"
	"AssemblyFileVersion  = '$script:AssemblyFileVersion'" 
	"InformationalVersion = '$script:InformationalVersion'"	
	"NuGetVersion         = '$script:InformationalVersion'"	
}

Task RestoreNugetPackages {	
	"Restore build packages"
	exec { & $NugetExe restore "$RootDir\build\packages.config" -SolutionDirectory $RootDir  }
	
	"`nRestore solution packages"
	exec { & $NugetExe restore "$RootDir\NodaMoney.sln" }
}

Task Compile -depends RestoreNugetPackages, CalculateVersion {
	$logger = if(isAppVeyor) { "/logger:C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" }
	$assemblyInfo = "$RootDir\src\GlobalAssemblyInfo.cs"
	
	"Set version to calculated version"
	applyVersioning $assemblyInfo $script:AssemblyVersion $script:AssemblyFileVersion $script:InformationalVersion
	
	"Compile solution"
	exec { msbuild "$RootDir\NodaMoney.sln" /t:Build /p:Configuration="Release" /p:Platform="Any CPU" /maxcpucount /verbosity:minimal /nologo $logger }
	
	"`nReset version to zero again (to prevent git checkin)"
	applyVersioning $assemblyInfo "0.0.0.0" "0.0.0.0" "0.0.0.0"
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

Task PushCoverage -requiredVariable CoverallsToken {
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
	$projectsToPackage = Get-ChildItem -File -Path $RootDir -Filter *.nuspec -Recurse | ForEach-Object { $_.FullName -replace "nuspec", "csproj" }
	
	foreach ($proj in $projectsToPackage) {
		exec { & $NugetExe pack $proj -OutputDirectory $ArtifactsDir }
	}
	
	#if(isAppVeyor) {
	#	Get-ChildItem $ArtifactsDir *.nupkg | ForEach-Object { Push-AppveyorArtifact ($_ | Resolve-Path).Path }
	#}
}

Task PushPackage -requiredVariable NugetApiKey {
	exec { & $NugetExe push "$ArtifactsDir\*.nupkg" $NugetApiKey -source $NugetFeed }
}

Task Zip -depends Compile {
	$7zExe = Join-Path $ToolsDir -ChildPath "\7-Zip*\7z.exe"
	
	exec {
		& $7zExe a -tzip "$ArtifactsDir\NodaMoney.$script:InformationalVersion.zip" "$RootDir\src\NodaMoney.Serialization.AspNet\bin\Release\*" "$RootDir\README.md" "$RootDir\LICENSE.txt" -x!"*.CodeAnalysisLog.xml" -x!"*.lastcodeanalysissucceeded"
	}	
}

Task Clean {
	Get-ChildItem $RootDir -Recurse -Include 'bin','obj','packages','artifacts' | ForEach-Object { Remove-Item $_ -Recurse -Force;  Write-Host Deleted $_ }
}

function isAppVeyor() {
	Test-Path -Path env:\APPVEYOR
}

function applyVersioning($assemblyInfoFile, $assemblyVersion, $assemblyFileVersion, $informationalVersion) {
	Write-Output "Apply versioning to $assemblyInfoFile"
	Write-Output "AssemblyVersion: $assemblyVersion"
	Write-Output "AssemblyFileVersion: $assemblyFileVersion"
	Write-Output "InformationalVersion: $informationalVersion"
	
	(Get-Content $assemblyInfoFile ) | ForEach-Object {
        Foreach-Object { $_ -replace 'AssemblyVersion.+$', "AssemblyVersion(`"$assemblyVersion`")]" } |
        Foreach-Object { $_ -replace 'AssemblyFileVersion.+$', "AssemblyFileVersion(`"$assemblyFileVersion`")]" } |
        Foreach-Object { $_ -replace 'AssemblyInformationalVersion.+$', "AssemblyInformationalVersion(`"$informationalVersion`")]" }
    } | Set-Content $assemblyInfoFile
	
	Write-Output "Versioning applied.`n"
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
