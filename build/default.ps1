# This build assumes the following directory structure (https://gist.github.com/davidfowl/ed7564297c61fe9ab814):
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
	$SrcDir = "$RootDir\src"
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
	$assemblyInfoFiles = Get-ChildItem -File -Path $SrcDir -Filter AssemblyInfo.cs -Recurse
	
	"Set version to calculated version"
	foreach ($file in $assemblyInfoFiles) {
		applyVersioning $file.FullName $script:AssemblyVersion $script:AssemblyFileVersion $script:InformationalVersion
	}
	
	"Compile solution"
	exec { msbuild "$RootDir\NodaMoney.sln" /t:Build /p:Configuration="Release" /p:Platform="Any CPU" /maxcpucount /verbosity:minimal /nologo $logger }

	"`nReset version to zero again (to prevent git checkin)"
	foreach ($file in $assemblyInfoFiles) {
		applyVersioning $file.FullName "0.0.0.0" "0.0.0.0" "0.0.0.0"
	}
}

Task Test -depends Compile {
	$openCoverExe = Resolve-Path "$rootDir\packages\OpenCover.*\tools\OpenCover.Console.exe"	
	$xunitConsoleExe = Resolve-Path "$rootDir\packages\xunit.runner.console.*\tools\xunit.console.exe"
	
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
	
	# Run OpenCover, which in turn will run Xunit
	exec {
		& $openCoverExe -register:user -target:$xunitConsoleExe "-targetargs:$testAssemblies -nologo -noappdomain -xml $ArtifactsDir\xunit.xml" "-filter:+[NodaMoney*]* -[NodaMoney.Tests]*" -output:"$ArtifactsDir\coverage.xml" -returntargetcode
	}
}

Task TestNew {
	$projectsToPackage = Get-ChildItem -File -Path $SrcDir -Filter project.json -Recurse
	
	foreach ($proj in $projectsToPackage) {
		exec { & dotnet test --no-build --configuration Release --output $ArtifactsDir $proj.FullName }
	}
}

Task PushCoverage `
	-requiredVariable CoverallsToken `
	-precondition { return $env:APPVEYOR_PULL_REQUEST_NUMBER -eq $null } `
{
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
	$projectsToPackage = Get-ChildItem -File -Path $SrcDir -Filter project.json -Recurse
	
	foreach ($proj in $projectsToPackage) {
		$json = Get-Content -Raw -Path $proj.FullName | ConvertFrom-Json
		#$json.version = $script:InformationalVersion
		#$json | ConvertTo-Json  | Set-Content $proj.FullName

		#$jsonpath = $scriptDir + "\project.json"
		#$json = Get-Content -Raw -Path $jsonpath | ConvertFrom-Json
		#$versionString = $json.version
		#$patchInt = [convert]::ToInt32($versionString.Split(".")[2], 10)
		#[int]$incPatch = $patchInt + 1
		#$patchUpdate = $versionString.Split(".")[0] + "." + $versionString.Split(".")[1] + "." + ($incPatch -as [string])
		#$json.version = $patchUpdate
		#$json | ConvertTo-Json -depth 999 | Out-File $jsonpath

		exec { & dotnet pack --no-build --configuration Release --output $ArtifactsDir $proj.FullName }
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
