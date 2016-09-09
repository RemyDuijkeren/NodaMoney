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
	$TestsDir = "$RootDir\tests"
	$ArtifactsDir = "$RootDir\artifacts"
	$ToolsDir = "$RootDir\tools"	
	$NugetExe = Join-Path $ToolsDir -ChildPath "\NuGet*\nuget.exe"

	$global:config = "debug"
}

Task default -depends local
Task local -depends init, build, test, zip
Task ci -depends release, local, pushcoverage
Task deploy -depends ci, pushpackage

Task release {
    $global:config = "release"
}

Task init {
	"Install dotnet, if not available"
	Install-Dotnet

	"Restore packages for build"
	exec { & $NugetExe restore "$RootDir\build\packages.config" -SolutionDirectory $RootDir  }

    "(Re)create Artifacts directory"
	Remove-Item $ArtifactsDir -Recurse -Force -ErrorAction SilentlyContinue
	New-Item $ArtifactsDir -ItemType directory | out-null
}

Task version {
	"Calculate version"
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
	$script:InformationalVersion = $versionInfo.InformationalVersion
	$script:NuGetVersion = $versionInfo.NuGetVersion	
	
	"AssemblyVersion      = '$script:AssemblyVersion'"
	"AssemblyFileVersion  = '$script:AssemblyFileVersion'" 
	"InformationalVersion = '$script:InformationalVersion'"	
	"NuGetVersion         = '$script:NuGetVersion'"

	#$version = if ($env:APPVEYOR_BUILD_NUMBER -ne $NULL) { $env:APPVEYOR_BUILD_NUMBER } else { '0' }
	#$version = "{0:D5}" -f [convert]::ToInt32($version, 10)

	"Set assemblyInfo files to calculated version"
	$assemblyInfoFiles = Get-ChildItem -File -Path $SrcDir -Filter AssemblyInfo.cs -Recurse
	
	foreach ($file in $assemblyInfoFiles) {
		applyVersioning $file.FullName $script:AssemblyVersion $script:AssemblyFileVersion $script:InformationalVersion
	}

	"Set project.json files to calculated version"
	$projectJsonFiles = Get-ChildItem -File -Path $SrcDir -Filter project.json -Recurse

	foreach ($file in $projectJsonFiles) {
		Write-Output "Apply version $NuGetVersion to $file"

		(Get-Content $file.FullName ) | ForEach-Object {
        	Foreach-Object { $_ -replace '"version": .+$', "`"version`": `"$NuGetVersion`"," }
    	} | Set-Content $file.FullName
	}
}

Task build -depends version { 
  	$projectsToBuild = Get-ChildItem -File -Path $SrcDir -Filter project.json -Recurse
		
	foreach ($proj in $projectsToBuild) {
		exec { & dotnet restore $proj.FullName }
		exec { & dotnet build $proj.FullName --configuration $config }
		exec { & dotnet pack $proj.FullName --no-build --configuration $config --output $ArtifactsDir }
	}

	# if(isAppVeyor) {
	# 	Get-ChildItem $ArtifactsDir *.nupkg | ForEach-Object { Push-AppveyorArtifact ($_ | Resolve-Path).Path }
	# }
}

Task test {
	$openCoverExe = Resolve-Path "$rootDir\packages\OpenCover.*\tools\OpenCover.Console.exe"	
	$dotnetExe = Resolve-Path "C:\Program Files\dotnet\dotnet.exe"
	$projectsToTest = Get-ChildItem -File -Path $TestsDir -Filter project.json -Recurse
	
	foreach ($proj in $projectsToTest) {
		Write-Host $proj.FullName
		exec { & dotnet restore $proj.FullName }

		# Run OpenCover, which in turns will run dotnet test
		$targetArgs = "test --configuration $config " + $proj.FullName
		exec {
			& $openCoverExe -register:user `
							-target:$dotnetExe `
							-targetargs:$targetArgs `
							"-filter:+[NodaMoney*]* -[NodaMoney.Tests]*" `
							-output:"$ArtifactsDir\coverage.xml" `
							-returntargetcode `
							-oldStyle
		}
	}
}

Task zip {
	$7zExe = Join-Path $ToolsDir -ChildPath "\7-Zip*\7z.exe"

	exec { & $7zExe u -tzip "$ArtifactsDir\NodaMoney.$NugetVersion.zip" "$RootDir\src\NodaMoney\bin\$config\*" "$RootDir\README.md" "$RootDir\LICENSE.txt" -x!"*.CodeAnalysisLog.xml" -x!"*.lastcodeanalysissucceeded" }
	exec { & $7zExe u -tzip "$ArtifactsDir\NodaMoney.$NugetVersion.zip" "$RootDir\src\NodaMoney.Serialization.AspNet\bin\$config\*" -x!"*.CodeAnalysisLog.xml" -x!"*.lastcodeanalysissucceeded"	}	
}

Task pushcoverage `
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

Task pushpackage -requiredVariable NugetApiKey {
	exec { & $NugetExe push "$ArtifactsDir\*.nupkg" $NugetApiKey -source $NugetFeed }
}

function isAppVeyor() {
	Test-Path -Path env:\APPVEYOR
}

function applyVersioning($assemblyInfoFile, $assemblyVersion, $assemblyFileVersion, $informationalVersion) {
	Write-Output "Apply to $assemblyInfoFile AssemblyVersion $assemblyVersion, AssemblyFileVersion: $assemblyFileVersion and "
	Write-Output "InformationalVersion: $informationalVersion"
	
	(Get-Content $assemblyInfoFile ) | ForEach-Object {
        Foreach-Object { $_ -replace 'AssemblyVersion.+$', "AssemblyVersion(`"$assemblyVersion`")]" } |
        Foreach-Object { $_ -replace 'AssemblyFileVersion.+$', "AssemblyFileVersion(`"$assemblyFileVersion`")]" } |
        Foreach-Object { $_ -replace 'AssemblyInformationalVersion.+$', "AssemblyInformationalVersion(`"$informationalVersion`")]" }
    } | Set-Content $assemblyInfoFile
}

function Install-Dotnet {
    $dotnetcli = Where-Is('dotnet')
	
    if($dotnetcli -eq $null)
    {
		$dotnetPath = "$pwd\.dotnet"
		$dotnetCliVersion = if ($env:DOTNET_CLI_VERSION -eq $null) { 'Latest' } else { $env:DOTNET_CLI_VERSION }
		$dotnetInstallScriptUrl = 'https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/install.ps1'
		$dotnetInstallScriptPath = '.\scripts\obtain\install.ps1'

		md -Force ".\scripts\obtain\" | Out-Null
		curl $dotnetInstallScriptUrl -OutFile $dotnetInstallScriptPath
		& .\scripts\obtain\install.ps1 -Channel "preview" -version $dotnetCliVersion -InstallDir $dotnetPath -NoPath
		$env:Path = "$dotnetPath;$env:Path"
	}
}

function Where-Is($command) {
    (ls env:\path).Value.split(';') | `
        where { $_ } | `
        %{ [System.Environment]::ExpandEnvironmentVariables($_) } | `
        where { test-path $_ } |`
        %{ ls "$_\*" -include *.bat,*.exe,*cmd } | `
        %{  $file = $_.Name; `
            if($file -and ($file -eq $command -or `
			   $file -eq ($command + '.exe') -or  `
			   $file -eq ($command + '.bat') -or  `
			   $file -eq ($command + '.cmd'))) `
            { `
                $_.FullName `
            } `
        } | `
        select -unique
}