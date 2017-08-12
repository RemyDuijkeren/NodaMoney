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
	$PackagesDir = "$RootDir\packages"
	$ArtifactsDir = "$RootDir\artifacts"
	$ToolsDir = "$RootDir\tools"
	$NugetExe = Join-Path $ToolsDir -ChildPath "\NuGet*\nuget.exe"

	$global:config = "debug"
}

Task default -depends local
Task local -depends init, restore, version, build, test, zip
Task ci -depends release, local
Task deploy -depends ci, pushpackage

Task release {
    $global:config = "release"
}

Task init {
	"Install dotnet, if not available"
	Install-Dotnet

    "(Re)create Artifacts directory"
	Remove-Item $ArtifactsDir -Recurse -Force -ErrorAction SilentlyContinue
	New-Item $ArtifactsDir -ItemType directory | out-null
}

Task restore { 
	"Restore packages for build"
	exec { & dotnet restore "$RootDir\build\packages.csproj" --packages $PackagesDir }
	Remove-Item "$RootDir\build\obj"  -Recurse -Force -ErrorAction SilentlyContinue

	"Restore packages for source projects"
  	$projectsToBuild = Get-ChildItem -File -Path $SrcDir -Filter *.csproj -Recurse		
	foreach ($proj in $projectsToBuild) {
		Push-Location $proj.PSParentPath
		exec { & dotnet restore }
		Pop-Location
	}

	"Restore packages for test projects"
  	$projectsToBuild = Get-ChildItem -File -Path $TestDir -Filter *.csproj -Recurse		
	foreach ($proj in $projectsToBuild) {
		Push-Location $proj.PSParentPath
		exec { & dotnet restore }
		Pop-Location
	}
}

Task version {
	$gitVersionExe = Resolve-Path "$PackagesDir\gitVersion.commandline\*\tools\GitVersion.exe"	

	"Send updated version to AppVeyor"
	if (isAppVeyor) { exec { & $gitVersionExe /output buildserver } }
	
	"Calculate version"
	$json = exec { & $gitVersionExe }		
	$versionInfo = $json -join "`n" | ConvertFrom-Json
	
	$script:BuildVersion = if ($env:APPVEYOR_BUILD_NUMBER -ne $NULL) { $env:APPVEYOR_BUILD_NUMBER } else { '0' }
	$script:AssemblyVersion =  [string]$versionInfo.Major + ".0.0.0" # Minor and Patch versions should work with base Major version
	$script:AssemblyFileVersion = $versionInfo.MajorMinorPatch + "." + $BuildVersion
	$script:InformationalVersion = $versionInfo.FullSemVer
	$script:NuGetVersion = $versionInfo.NuGetVersion

	"Update Directory.build.props"
	Write-Output "Apply Version $NuGetVersion, AssemblyVersion $assemblyVersion, AssemblyFileVersion $assemblyFileVersion and InformationalVersion $informationalVersion"
	$versionFile = "$SrcDir\Directory.build.props"
	$xml = [xml](Get-Content $versionFile)
	$xml.Project.PropertyGroup.Version = $NuGetVersion
	$xml.Project.PropertyGroup.AssemblyVersion = $AssemblyVersion
	$xml.Project.PropertyGroup.FileVersion = $AssemblyFileVersion
	$xml.Project.PropertyGroup.InformationalVersion = $InformationalVersion	

	$xml.Save($versionFile)
}

Task build { 
  	$projectsToBuild = Get-ChildItem -File -Path $SrcDir -Filter *.csproj -Recurse		
	foreach ($proj in $projectsToBuild) {
		Push-Location $proj.PSParentPath
		exec { & dotnet build --configuration $config }
		exec { & dotnet pack --no-build --configuration $config --output $ArtifactsDir }
		Pop-Location
	}
}

Task test {
	$projectsToTest = Get-ChildItem -File -Path $TestsDir -Filter *.csproj -Recurse	
	foreach ($proj in $projectsToTest) {
		Push-Location $proj.PSParentPath
		exec { & dotnet test --configuration $config }
		Pop-Location
	}
}

# Doesn't currently work wit .NET Core :-(
Task testWithCoverage {
	$openCoverExe = Resolve-Path "$PackagesDir\OpenCover.*\tools\OpenCover.Console.exe"
	$dotnetExe = Where-Is('dotnet')
	$projectsToTest = Get-ChildItem -File -Path $TestsDir -Filter *.csproj -Recurse
	
	foreach ($proj in $projectsToTest) {
		Write-Host $proj.FullName
		exec { & dotnet restore $proj.FullName }

		# Run OpenCover, which in turns will run dotnet test
		$targetArgs = "test /p:DebugType=full --configuration $config " + $proj.FullName
		exec {
			& $openCoverExe -register:user `
							-target:$dotnetExe `
							-targetargs:$targetArgs `
							"-filter:+[NodaMoney*]* -[NodaMoney.Tests]*" `
							-output:"$ArtifactsDir\coverage.xml" `
							-returntargetcode `
							-mergeoutput `
							-hideskipped:File `
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
	$coverallsExe = Resolve-Path "$PackagesDir\coveralls.net.*\tools\csmacnz.Coveralls.exe"
	
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

function Install-Dotnet {
    $dotnetcli = Where-Is('dotnet')	
    if($dotnetcli -eq $null)
    {
		$dotnetPath = "$pwd\.dotnet"
		$dotnetCliVersion = if ($env:DOTNET_CLI_VERSION -eq $null) { 'Latest' } else { $env:DOTNET_CLI_VERSION }
		$dotnetInstallScriptUrl = 'https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.ps1'
		$dotnetInstallScriptPath = '.\scripts\obtain\dotnet-install.ps1'

		mddir -Force ".\scripts\obtain\" | Out-Null
		Invoke-WebRequest $dotnetInstallScriptUrl -OutFile $dotnetInstallScriptPath
		& .\scripts\obtain\dotnet-install.ps1 -Channel "preview" -version $dotnetCliVersion -InstallDir $dotnetPath -NoPath
		$env:Path = "$dotnetPath;$env:Path"
	}
}

function Where-Is($command) {
    (Get-ChildItem env:\path).Value.split(';') | `
        Where-Object { $_ } | `
        ForEach-Object{ [System.Environment]::ExpandEnvironmentVariables($_) } | `
        Where-Object { test-path $_ } |`
        ForEach-Object{ ls "$_\*" -include *.bat,*.exe,*cmd } | `
        ForEach-Object{  $file = $_.Name; `
            if($file -and ($file -eq $command -or `
			   $file -eq ($command + '.exe') -or  `
			   $file -eq ($command + '.bat') -or  `
			   $file -eq ($command + '.cmd'))) `
            { `
                $_.FullName `
            } `
        } | `
        Select-Object -unique
}