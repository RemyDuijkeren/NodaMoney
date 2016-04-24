#This build assumes the following directory structure
#
#   \Build    	- This is where the project build scripts lives (this file)
#   \Artifacts	- This folder is created if it is missing and contains output of the build
#	\Tools		- This is where tools, utilities and executables are stored that the builds need
#
Framework "4.6"
FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Properties {
	$CoverallsToken = $env:COVERALLS_REPO_TOKEN
	$NugetApiKey = $env:NUGET_API_KEY
	$NugetFeed = "https://staging.nuget.org" #/packages?replace=true"
	
	$RootDir = Resolve-Path ..
	$ArtifactsDir = "$RootDir/Artifacts"
	$ToolsDir = "$RootDir/Tools"
}

Task Default -depends Clean, ApplyVersioning, Compile, Test, Package, Zip, PushCoverage
Task Deploy  -depends Default, PushCoverage, PushPackage

Task Clean {
    "Clean Artifacts directory"	
	Remove-Item $ArtifactsDir -Recurse -Force -ErrorAction SilentlyContinue #| out-null
	New-Item $ArtifactsDir -ItemType directory | out-null
    
	"Clean solution"
    exec { msbuild "$RootDir\NodaMoney.sln" /t:Clean /p:Configuration="Release" /p:Platform="Any CPU" /maxcpucount /verbosity:minimal /nologo }
}

Task CalculateVersion {
	$gitVersionExe = Join-Path $ToolsDir -ChildPath "GitVersion.exe"
	
    $json = exec { & $gitVersionExe }
	foreach ($line in $json) { "$line" }
	
	$versionInfo = $json -join "`n" | ConvertFrom-Json
    
	$script:AssemblyVersion = $versionInfo.AssemblySemVer
	"Use '$script:AssemblyVersion' as AssemblyVersion"
	$script:AssemblyFileVersion = $versionInfo.ClassicVersion
	"Use '$script:AssemblyFileVersion' as AssemblyFileVersion"    
	$script:InformationalVersion = $versionInfo.NuGetVersion
	"Use '$script:InformationalVersion' as InformationalVersion"
    $script:NuGetVersion = $versionInfo.NuGetVersion
	"Use '$script:NuGetVersion' as NuGetVersion"
}

Task ApplyVersioning -depends CalculateVersion {
	$assemblyInfo = "$RootDir\GlobalAssemblyInfo.cs"
	
	"Updating $assemblyInfo with versioning"
	(Get-Content $assemblyInfo ) | ForEach-Object {
        Foreach-Object { $_ -replace 'AssemblyVersion.+$', "AssemblyVersion(`"$script:AssemblyVersion`")]" } |
        Foreach-Object { $_ -replace 'AssemblyFileVersion.+$', "AssemblyFileVersion(`"$script:AssemblyFileVersion`")]" } |
        Foreach-Object { $_ -replace 'AssemblyInformationalVersion.+$', "AssemblyInformationalVersion(`"$script:InformationalVersion`")]" }
    } | Set-Content $assemblyInfo
}

Task RestoreNugetPackages {
	$nugetExe = Join-Path $ToolsDir -ChildPath "NuGet.exe"	
	exec { & $nugetExe restore "$RootDir\NodaMoney.sln" }
}

Task Compile -depends RestoreNugetPackages {
	$logger = if(isAppVeyor) { "/logger:C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" }
	
	exec { msbuild "$RootDir\NodaMoney.sln" /t:Build /p:Configuration="Release" /p:Platform="Any CPU" /maxcpucount /verbosity:minimal /nologo $logger }
}

Task Test {
	$openCoverExe = Resolve-Path "$rootDir\packages\OpenCover.*\tools\OpenCover.Console.exe"	
	$logger = if(isAppVeyor) { "Appveyor" } else { "trx" }
	$VsTestConsoleExe = if(isAppVeyor) { "vstest.console.exe" } else { "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" }
	
	# change current working dir to get TestResults (trx) in artifacts folder
	Set-Location $ArtifactsDir | out-null
	
	exec {
		& $openCoverExe -register:user -target:$VsTestConsoleExe "-targetargs:""$RootDir\NodaMoney.UnitTests\bin\Release\NodaMoney.UnitTests.dll"" /Logger:$logger" -filter:"+[NodaMoney*]*" -output:"$ArtifactsDir\coverage.xml"
	}
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
		& $7zExe a -tzip "$ArtifactsDir\NodaMoney.zip" "$RootDir\NodaMoney.Serialization.AspNet\bin\Release\*" "$RootDir\README.md" "$RootDir\LICENSE.txt" -x!"*.CodeAnalysisLog.xml" -x!"*.lastcodeanalysissucceeded"
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
