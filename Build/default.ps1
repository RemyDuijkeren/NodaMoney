#This build assumes the following directory structure
#
#   \Build    	- This is where the project build scripts lives (this file)
#   \Artifacts	- This folder is created if it is missing and contains output of the build
#	\Tools		- This is where tools, utilities and executables are stored that the builds need
#
Framework "4.6"

Properties {
    $Configuration = "Release"
	$CoverallsToken = $env:COVERALLS_REPO_TOKEN
	$NugetApiKey = $env:NUGET_API_KEY
	$NugetFeed = "https://staging.nuget.org" #/packages?replace=true"
	
	$RootDir = Resolve-Path ..
	$ArtifactsDir = "$RootDir/Artifacts"
	$ToolsDir = "$RootDir/Tools"
	$VsTestConsoleExe = "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
}

FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task Default -depends Clean, ApplyVersioning, RestoreNugetPackages, Compile, Test, Package, Zip

Task Clean {
    "Clean Artifacts directory"	
    if (Test-Path $ArtifactsDir) {   
        rd $ArtifactsDir -rec -force -ErrorAction SilentlyContinue | out-null
    }

    mkdir $ArtifactsDir | out-null

    "Clean solution"
    exec {
		msbuild "$RootDir\NodaMoney.sln" /t:Clean /p:Configuration=$Configuration /p:Platform="Any CPU" /v:m /nologo
	}
}

Task CalculateVersion {
	$gitVersionExe = Join-Path $ToolsDir -ChildPath "GitVersion.exe"
	
    $json = exec { & $gitVersionExe }	
	$versionInfo = $json -join "`n" | ConvertFrom-Json
        
	$script:AssemblyVersion = $versionInfo.AssemblySemVer
	$script:AssemblyFileVersion = $versionInfo.ClassicVersion
    $script:InformationalVersion = $versionInfo.NuGetVersion
    $script:NuGetVersion = $versionInfo.NuGetVersion	
	
	"Calculated AssemblyVersion: $script:AssemblyVersion"
	"Calculated AssemblyFileVersion: $script:AssemblyFileVersion"
	"Calculated InformationalVersion: $script:InformationalVersion"
	"Calculated NuGetVersion: $script:NuGetVersion"	
}

Task ApplyVersioning -depends CalculateVersion {
	$assemblyInfo = "$RootDir\GlobalAssemblyInfo.cs"
	
	"Updating $assemblyInfo"
	(Get-Content $assemblyInfo ) | ForEach-Object {
        % {$_ -replace 'AssemblyVersion.+$', "AssemblyVersion(`"$script:AssemblyVersion`")]" } |
        % {$_ -replace 'AssemblyFileVersion.+$', "AssemblyFileVersion(`"$script:AssemblyFileVersion`")]" } |
        % {$_ -replace 'AssemblyInformationalVersion.+$', "AssemblyInformationalVersion(`"$script:InformationalVersion`")]" }
    } | Set-Content $assemblyInfo
}

Task Compile -depends ApplyVersioning, RestoreNugetPackages {	
	if(isAppVeyor) {
		exec {
			msbuild "$RootDir\NodaMoney.sln" /t:Rebuild /p:Configuration=$Configuration /p:Platform="Any CPU" /v:m /nologo /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"
		}
	} else {
		exec {
			msbuild "$RootDir\NodaMoney.sln" /t:Rebuild /p:Configuration=$Configuration /p:Platform="Any CPU" /v:m /nologo
		}
	}
}

Task RestoreNugetPackages {
	$nugetExe = Join-Path $ToolsDir -ChildPath "NuGet.exe"	
	exec { & $nugetExe restore "$RootDir\NodaMoney.sln" }
}

Task Test {
	$openCoverExe = Resolve-Path "$rootDir\packages\OpenCover.*\tools\OpenCover.Console.exe"
	
	if(isAppVeyor) {
		exec {
			& $openCoverExe -register:user -target:"vstest.console.exe" "-targetargs:""$RootDir\NodaMoney.UnitTests\bin\Release\NodaMoney.UnitTests.dll"" /Logger:Appveyor" -filter:"+[NodaMoney*]*" -output:"$ArtifactsDir\coverage.xml"
		}
	} else {
		cd $ArtifactsDir | out-null # change current working dir to get TestResults (trx) in artifacts folder		
		exec {
			& $openCoverExe -register:user -target:$VsTestConsoleExe "-targetargs:""$RootDir\NodaMoney.UnitTests\bin\Release\NodaMoney.UnitTests.dll"" /Logger:trx" -filter:"+[NodaMoney*]*" -output:"$ArtifactsDir\coverage.xml"
		}
	}
}

Task PushCoverage {
	"Pushing coverage to coveralls.io"
	$coverallsExe = Resolve-Path "$RootDir\packages\coveralls.net.*\tools\csmacnz.Coveralls.exe"	
	
	if(isAppVeyor) {
		exec {
			& $coverallsExe --opencover -i $ArtifactsDir\coverage.xml --repoToken $CoverallsToken --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_BUILD_NUMBER --serviceName appveyor
		}
	} else {
		exec {
			& $coverallsExe --opencover -i $ArtifactsDir\coverage.xml --repoToken $CoverallsToken
		}
	}	   
}

Task Package {
	$nugetExe = Join-Path $ToolsDir -ChildPath "NuGet.exe";
	
	exec {
		& $nugetExe pack "$RootDir\NodaMoney\NodaMoney.csproj" -OutputDirectory $ArtifactsDir
	}
	exec {
		& $nugetExe pack "$RootDir\NodaMoney.Serialization.AspNet\NodaMoney.Serialization.AspNet.csproj" -OutputDirectory $ArtifactsDir
	}	

	#if(isAppVeyor) {
	#	$packages = Get-ChildItem $ArtifactsDir *.nupkg
	#	foreach ($package in $packages) {
	#		Push-AppveyorArtifact ($package | Resolve-Path).Path
	#	}
	#}
}

Task PushPackage {
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
