#This build assumes the following directory structure
#
#   \Build    	- This is where the project build code lives (this file)
#   \Artifacts	- This folder is created if it is missing and contains output of the build
#	\Tools		- This is where tools, utilities and executables are stored that the builds need
#
Framework "4.6"

Properties {
    $Configuration = "Release"
	$VsTestConsoleExe = "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
	$CoverallsToken = $env:COVERALLS_REPO_TOKEN
	$NugetApiKey = $env:NUGET_API_KEY
	$NugetFeed = "https://staging.nuget.org" #/packages?replace=true"
}

FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task Default -depends Clean, ApplyVersioning, RestoreNugetPackages, Compile, Test, Package, Zip

Task Clean {
    "Clean Artifacts directory"
	$artifactsDir = get-artifactsDirectory	
    if (Test-Path $artifactsDir) 
    {   
        rd $artifactsDir -rec -force -ErrorAction SilentlyContinue | out-null
    }

    mkdir $artifactsDir | out-null

    "Clean solution"
	$rootDir = get-rootDirectory
    exec {
		msbuild "$rootDir\NodaMoney.sln" /t:Clean /p:Configuration=$Configuration /p:Platform="Any CPU" /v:m /nologo
	}
}

Task CalculateVersion {
	$toolsDir = get-toolsDirectory
	$gitVersionExe = Join-Path $toolsDir -ChildPath "GitVersion.exe"
	
    $json = exec { .$gitVersionExe }
	
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
	$rootDir = get-rootDirectory;
	$info = "$rootDir\GlobalAssemblyInfo.cs"
	
	"Updating $info"
	(Get-Content $info ) | ForEach-Object {
        % {$_ -replace 'AssemblyVersion.+$', "AssemblyVersion(`"$script:AssemblyVersion`")]" } |
        % {$_ -replace 'AssemblyFileVersion.+$', "AssemblyFileVersion(`"$script:AssemblyFileVersion`")]" } |
        % {$_ -replace 'AssemblyInformationalVersion.+$', "AssemblyInformationalVersion(`"$script:InformationalVersion`")]" }
    } | Set-Content $info
}

Task Compile -depends ApplyVersioning, RestoreNugetPackages {
	$rootDir = get-rootDirectory
    exec { 
        msbuild "$rootDir\NodaMoney.sln" /t:Rebuild /p:Configuration=$Configuration /p:Platform="Any CPU" /v:m /nologo
    }
}

Task RestoreNugetPackages {
	$rootDir = get-rootDirectory
	$toolsDir = get-toolsDirectory
	$nugetExe = Join-Path $toolsDir -ChildPath "NuGet.exe"
	
	exec { .$nugetExe restore "$rootDir\NodaMoney.sln" }
}

Task Test {
	$rootDir = get-rootDirectory
	$artifactsDir = get-artifactsDirectory
	$openCoverExe = Resolve-Path "$rootDir\packages\OpenCover.*\tools\OpenCover.Console.exe"
	
	cd $artifactsDir | out-null # move current working dir to get TestResults in artifacts
	
	& vstest.console.exe /Logger:Appveyor ..\NodaMoney.UnitTests\bin\Release\NodaMoney.UnitTests.dll
	
	if(isAppVeyor) {
		exec {
			.$openCoverExe -register:user -target:"vstest.console.exe" -targetargs:"/Logger:Appveyor '..\NodaMoney.UnitTests\bin\Release\NodaMoney.UnitTests.dll'" -filter:"+[NodaMoney*]*" -output:"$artifactsDir\coverage.xml"
		}
	} else {
		.$openCoverExe -register:user -target:$VsTestConsoleExe -filter:"+[NodaMoney*]*" -targetargs:"..\NodaMoney.UnitTests\bin\Release\NodaMoney.UnitTests.dll /InIsolation /Logger:trx" -output:"$artifactsDir\coverage.xml"
	}
}

Task PushCoverage {
	"Pushing coverage to coveralls.io"
	$rootDir = get-rootDirectory
	$artifactsDir = get-artifactsDirectory
	$coverallsExe = Resolve-Path "$rootDir\packages\coveralls.net.*\tools\csmacnz.Coveralls.exe"	
	
	if(isAppVeyor) {
		exec {
			.$coverallsExe --opencover -i coverage.xml --repoToken $CoverallsToken --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_BUILD_NUMBER --serviceName appveyor
		}
	} else {
		exec { .$coverallsExe --opencover -i $artifactsDir\coverage.xml --repoToken $CoverallsToken }
	}	   
}

Task Package {
	$rootDir = get-rootDirectory
	$artifactsDir = get-artifactsDirectory
	$toolsDir = get-toolsDirectory;
	$nugetExe = Join-Path $toolsDir -ChildPath "NuGet.exe";
	
	exec { .$nugetExe pack "$rootDir\NodaMoney\NodaMoney.csproj" -OutputDirectory $artifactsDir	}
	exec { .$nugetExe pack "$rootDir\NodaMoney.Serialization.AspNet\NodaMoney.Serialization.AspNet.csproj" -OutputDirectory $artifactsDir }	

	if(isAppVeyor) {
		$packages = Get-ChildItem $ArtifactsDirectory *.nupkg
		foreach ($package in $packages) {
			Push-AppveyorArtifact ($package | Resolve-Path).Path;
		}
	}
}

Task PushPackage {
	$artifactsDir = get-artifactsDirectory
	$toolsDir = get-toolsDirectory	
	$nugetExe = Join-Path $toolsDir -ChildPath "NuGet.exe"	
		
	exec { .$nugetExe push "$artifactsDir\*.nupkg" $NugetApiKey -source $NugetFeed }
}

Task Zip {
	$rootDir = get-rootDirectory
	$artifactsDir = get-artifactsDirectory
	$toolsDir = get-toolsDirectory;	
	$7zExe = Join-Path $toolsDir -ChildPath "7z.exe"
	
	exec {
		.$7zExe a -tzip "$artifactsDir\NodaMoney.zip" "$rootDir\NodaMoney.Serialization.AspNet\bin\Release\*" "$rootDir\README.md" "$rootDir\LICENSE.txt" -x!"*.CodeAnalysisLog.xml" -x!"*.lastcodeanalysissucceeded"
	}	
}

function get-rootDirectory {
	return "." | Resolve-Path | Join-Path -ChildPath "../";
}

function get-artifactsDirectory {
	return "." | Resolve-Path | Join-Path -ChildPath "../Artifacts";
}

function get-toolsDirectory {
	return "." | Resolve-Path | Join-Path -ChildPath "../Tools";
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
