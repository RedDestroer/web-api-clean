### OPTIONS ###

$majorVersion = 0
$minorVersion = 0
$patchVersion = 0
$buildNumber = $env:Build_BuildID % 65535 

$preReleaseName = ''
$featureName = ''
$branchName = ''
$masterBranchName = 'main'

$devEnvironments = $env:DevEnv
$featureEnvironments = $env:FeatureEnv
$releaseEnvironments = $env:ReleaseEnv
$hotfixEnvironments = $env:HotfixEnv
$masterEnvironments = $env:MasterEnv
$supportEnvironments = $env:SupportEnv

$updateVersionInfoValue = "True"

# do update *.csproj [.NET Core / Sdk] and AssemblyInfo.cs files
$updateVersionInfo = $updateVersionInfoValue -like "True"

### GET VCS INFO ###

$currentBranch = $env:Build_SourceBranch 

Write-Host "Resolve current branch symbolic name: '$currentBranch'" # refs/heads/develop, refs/heads/release/0.0.11

$currentHash = (git rev-parse HEAD | Out-String).Trim()
Write-Host "Resolve current hash: '$currentHash'" # 19f1fe11d4ee6043f56996ea7c8aa185f7cf5745

$currentShortHash = (git rev-parse --short HEAD | Out-String).Trim()
Write-Host "Resolve current short hash: '$currentShortHash'" # 19f1fe11

### RESOLVE AND CALC VERSION FUNCTIONS ###

function ResolveLastReleaseVersion
{
    $lastReleaseBranch = (git for-each-ref "refs/remotes/origin/release/*.*.*" "--sort=-v:refname" "--format=%(refname)" --count=1 | Out-String).Trim()
    Write-Host "Resolve last release branch symbolic name: '$lastReleaseBranch'" # refs/heads/release/0.0.3

    $lastReleaseTag = (git for-each-ref "refs/tags/*.*.*" "--sort=-v:refname" "--format=%(refname)" --count=1 | Out-String).Trim()
    Write-Host "Resolve last release tag symbolic name: '$lastReleaseTag'" # refs/tags/0.4.0

    if($lastReleaseBranch -match '^refs\/remotes\/origin\/release\/(?<major>[0-9]{1,7})\.(?<minor>[0-9]{1,7})\.(?<patch>[0-9]{1,7})$') {
        $branchMajorVersion = $matches.major -as [int]
        $branchMinorVersion = $matches.minor -as [int]
        $branchPatchVersion = $matches.patch -as [int]
        # log
        Write-Host "Resolved last release branch version: '$branchMajorVersion.$branchMinorVersion.$branchPatchVersion'"
        # calculate greater version
        if(($global:majorVersion -lt $branchMajorVersion) `
            -or (($global:majorVersion -eq $branchMajorVersion) -and ($global:minorVersion -lt $branchMinorVersion)) `
            -or (($global:majorVersion -eq $branchMajorVersion) -and ($global:minorVersion -eq $branchMinorVersion) -and ($global:patchVersion -lt $branchPatchVersion))) {
            # setup
            $global:majorVersion = $branchMajorVersion
            $global:minorVersion = $branchMinorVersion
            $global:patchVersion = $branchPatchVersion
        }
    }

    if($lastReleaseTag -match '^refs\/tags\/(?<major>[0-9]{1,7})\.(?<minor>[0-9]{1,7})\.(?<patch>[0-9]{1,7})$') {
        $tagMajorVersion = $matches.major -as [int]
        $tagMinorVersion = $matches.minor -as [int]
        $tagPatchVersion = $matches.patch -as [int]
        # log
        Write-Host "Resolved last release tag version: '$tagMajorVersion.$tagMinorVersion.$tagPatchVersion'"
        # calculate greater version
        if(($global:majorVersion -lt $tagMajorVersion) `
            -or (($global:majorVersion -eq $tagMajorVersion) -and ($global:minorVersion -lt $tagMinorVersion)) `
            -or (($global:majorVersion -eq $tagMajorVersion) -and ($global:minorVersion -eq $tagMinorVersion) -and ($global:patchVersion -lt $tagPatchVersion))) {
            # setup
            $global:majorVersion = $tagMajorVersion
            $global:minorVersion = $tagMinorVersion
            $global:patchVersion = $tagPatchVersion
        }
    }
}

function ResolveDescribeVersion
{
    # get version describe info
    $gitDescribe = (git describe --tags --long --candidates 1 --match "*.*.*" | Out-String).Trim()
    Write-Host "Resolve describe tag: '$gitDescribe'" # 0.4.0-202-g19f1fe11
    # parse
    if($gitDescribe -match '^(v(er(sion)?)?[\.\s-]?)?(?<major>[0-9]{1,7})\.(?<minor>[0-9]{1,7})\.(?<patch>[0-9]{1,7})-(?<commitsCount>\d+)-(?<hash>\w+)$') {
        # parse
        $describeMajorVersion = $matches.major -as [int]
        $describeMinorVersion = $matches.minor -as [int]
        $describePatchVersion = $matches.patch -as [int]
        $vtagCommits = $matches.commitsCount -as [int]
        $vtagHash = $matches.hash
        # log
        Write-Host "Resolved describe tag version: '$describeMajorVersion.$describeMinorVersion.$describePatchVersion'"
        Write-Host "Resolved describe tag commits count: '$vtagCommits'"
        Write-Host "Resolved describe tag hash: '$vtagHash'"
        # setup
        $global:majorVersion = $describeMajorVersion
        $global:minorVersion = $describeMinorVersion
        $global:patchVersion = $describePatchVersion
    } else {
        Write-Host "Warning: the describe command does not contain version information!"
    }
}


### RESOLVE VERSION FROM CURRENT BRANCH

#### master release tag
if($currentBranch -match '^refs\/tags\/(?<major>[0-9]{1,7})\.(?<minor>[0-9]{1,7})\.(?<patch>[0-9]{1,7})$') {
    # log
    Write-Host "Set build version method: Continuous Delivery (master/tag)"
    $majorVersion = $matches.major -as [int]
    $minorVersion = $matches.minor -as [int]
    $patchVersion = $matches.patch -as [int]
    # log
    Write-Host "Resolved master tag version: '$majorVersion.$minorVersion.$patchVersion'"
    # set branch name
    $branchName = $masterBranchName
    $environments = $masterEnvironments
}
#### master
elseif($currentBranch -match '^refs\/heads\/(master|main)$') {
    # log
    Write-Host "Set build version method: Continuous Delivery (master)"
    # resolve version
    ResolveDescribeVersion
    # set branch name
    $branchName = $masterBranchName
    $environments = $masterEnvironments
}
#### develop
elseif($currentBranch -match '^refs\/heads\/dev(elop(ment)?)?$') {
    # log
    Write-Host "Set build version method: Continuous Deployment (develop)"
    # resolve last version
    ResolveLastReleaseVersion
    # increment minor version and clear patch (for CD method)
    $minorVersion = $minorVersion + 1
    $patchVersion = 0
    # set pre-release version tag
    $preReleaseName = "alpha"
    # set branch name
    $branchName = "dev"
    $environments = $devEnvironments
}
#### feature
elseif($currentBranch -match '^refs\/heads\/feature\/(?<feature>[\w-]+)$') {
    # log
    Write-Host "Set build version method: Continuous Deployment (feature)"
    # parse feature name
    $featureName = $matches.feature
    Write-Host "Resolved feature name: '$featureName'"
    # resolve last version
    ResolveLastReleaseVersion
    # increment minor version and clear patch (for CD method)
    $minorVersion = $minorVersion + 1
    $patchVersion = 0
    # set pre-release version tag
    $preReleaseName = "alpha"
    # set branch name
    $branchName = "feature"
    $environments = $featureEnvironments
}
#### release
elseif($currentBranch -match '^refs\/heads\/release\/(?<major>[0-9]{1,7})\.(?<minor>[0-9]{1,7})\.(?<patch>[0-9]{1,7})$') {
    # log
    Write-Host "Set build version method: Continuous Deployment (release)"
    $majorVersion = $matches.major -as [int]
    $minorVersion = $matches.minor -as [int]
    $patchVersion = $matches.patch -as [int]
    # log
    Write-Host "Resolved release branch version: '$majorVersion.$minorVersion.$patchVersion'"
    # set pre-release version tag
    $preReleaseName = "beta"
    # set branch name
    $branchName = "release"
    $environments = $releaseEnvironments
}
#### hotfix
elseif($currentBranch -match '^refs\/heads\/hotfix\/(?<major>[0-9]{1,7})\.(?<minor>[0-9]{1,7})\.(?<patch>[0-9]{1,7})$') {
    # log
    Write-Host "Set build version method: Continuous Deployment (hotfix)"
    $majorVersion = $matches.major -as [int]
    $minorVersion = $matches.minor -as [int]
    $patchVersion = $matches.patch -as [int]
    # log
    Write-Host "Resolved hotfix branch version: '$majorVersion.$minorVersion.$patchVersion'"
    # set pre-release version tag
    $preReleaseName = "beta"
    # set branch name
    $branchName = "hotfix"
    $environments = $hotfixEnvironments
}
#### support
elseif($currentBranch -match '^refs\/heads\/support\/(?<feature>[\w-]?)$') {
    # log
    Write-Host "Set build version method: Continuous Deployment (support)"
    # parse feature name
    $featureName = $matches.feature
    Write-Host "Resolved feature name: '$featureName'"
    # resolve version
    ResolveDescribeVersion
    # set pre-release version tag
    $preReleaseName = "gamma"
    # set branch name
    $branchName = "support"
    $environments = $supportEnvironments
}
#### not resolved
else {
    Write-Host "Error: not supported branch name specification!"
    Exit 255
}


### SETUP VERSION INFO

$shortVersion = "$majorVersion.$minorVersion.$patchVersion"
$buildVersion = "$majorVersion.$minorVersion.$patchVersion.$buildNumber"

$semVer = $buildVersion

if($preReleaseName) {
    $semVer = $semVer + "-$preReleaseName"
#    if($buildNumber -gt 0) {
#        $semVer = $semVer + ".$buildNumber"
#    }
}

$semanticVersion = $semVer + "+build-$buildNumber"

if($branchName) {
    $semanticVersion = $semanticVersion + ".branch-$branchName"
}

if($featureName) {
    $semanticVersion = $semanticVersion + ".feature-$featureName"
}

$currentDate = Get-Date -format "yyyy-MM-dd"
$semanticVersion = $semanticVersion + ".date-$currentDate"

if($currentShortHash) {
    $semanticVersion = $semanticVersion + ".sha-$currentShortHash";
}

### LOG OUT

Write-Host "===================================================================="
Write-Host "MajorVersion = '$majorVersion'"
Write-Host "MinorVersion = '$minorVersion'"
Write-Host "PatchVersion = '$patchVersion'"
Write-Host "ShortVersion = '$shortVersion'"
Write-Host "BuildVersion = '$buildVersion'"
Write-Host "SemVer = '$semVer'"
Write-Host "SemanticVersion = '$semanticVersion'"
Write-Host "PackageVersion = '$semVer'"
Write-Host "VcsHash = '$currentHash'"
Write-Host "VcsShortHash = '$currentShortHash'"
Write-Host "BuildDate = '$currentDate'"
Write-Host "buildNumber = '$semVer'"
Write-Host "Will deploy to '$environments'"
Write-Host "===================================================================="


### SET TEAMCITY VARIABLES
Write-Host "##vso[task.setvariable variable=environments;]$($environments)"
# version numbers
Write-Host "##vso[task.setvariable variable=MajorVersion;]$($majorVersion)"
Write-Host "##vso[task.setvariable variable=MinorVersion;]$($minorVersion)"
Write-Host "##vso[task.setvariable variable=PatchVersion;]$($patchVersion)"

# version
Write-Host "##vso[task.setvariable variable=ShortVersion;]$($shortVersion)"
Write-Host "##vso[task.setvariable variable=BuildVersion;]$($buildVersion)"

# semantic version
Write-Host "##vso[task.setvariable variable=SemVer;]$($semVer)"
Write-Host "##vso[task.setvariable variable=SemanticVersion;]$($semanticVersion)"

# package version
Write-Host "##vso[task.setvariable variable=PackageVersion;]$($semVer)"

# other variables
if($currentHash) {
    Write-Host "##vso[task.setvariable variable=VcsHash;]$($currentHash)"
}
if($currentShortHash) {
    Write-Host "##vso[task.setvariable variable=VcsShortHash;]$($currentShortHash)"
}
Write-Host "##vso[task.setvariable variable=BuildDate;]$($currentDate)"

# general version (build number)
Write-Host $semVer

### UPDATE *.csproj [.NET Core / Sdk] AND AssemblyInfo.cs ###

function UpdateNetCoreCsproj($file) {
    $filePath = $file.FullName
    $xml = [xml](Get-Content $file)
    $project = $xml.Project
    if($project -and $project.Sdk -like "Microsoft.NET.Sdk*" ) {
        # Write-Host "found .NET Core csproj:" $filePath
        $propertyGroupList = $project.SelectNodes('PropertyGroup')
        if($propertyGroupList.Count -eq 0) {
            Write-Host "Warning: not found PropertyGroup section in csproj (Sdk) '$filePath'"
        } else {
            $propertyGroup = $propertyGroupList[0]

            if($null -eq $propertyGroup.Version) {
                $propertyGroup.AppendChild($xml.CreateElement("Version")) | Out-Null
            }
            if($null -eq $propertyGroup.AssemblyVersion) { 
                $propertyGroup.AppendChild($xml.CreateElement("AssemblyVersion")) | Out-Null
            }
            if($null -eq $propertyGroup.FileVersion) { 
                $propertyGroup.AppendChild($xml.CreateElement("FileVersion")) | Out-Null
            }

            $propertyGroup.Version = $global:semanticVersion
            $propertyGroup.AssemblyVersion = $global:buildVersion
            $propertyGroup.FileVersion = $global:buildVersion

            $xml.Save($filePath)
            Write-Host "Updated csproj (Sdk):" $filePath
            return $true
        }
    }
    return $false
}

function UpdateAssemblyInfo($folder) {
    $assemblyInfoPath = "$folder\AssemblyInfo.cs"
    $assemblyInfo = $null
    if(Test-Path $assemblyInfoPath) {
        $assemblyInfo = Get-Content $assemblyInfoPath
    } else {
        $assemblyInfoPath = "$folder\Properties\AssemblyInfo.cs"
        if(Test-Path $assemblyInfoPath) {
            $assemblyInfo = Get-Content -Path $assemblyInfoPath
        } else {
            $assemblyInfoPath = "$folder\SharedAssemblyInfo.cs"
            if(Test-Path $assemblyInfoPath) {
                $assemblyInfo = Get-Content -Path $assemblyInfoPath
            } else {
                $assemblyInfoPath = "$folder\Properties\SharedAssemblyInfo.cs"
                if(Test-Path $assemblyInfoPath) {
                    $assemblyInfo = Get-Content -Path $assemblyInfoPath
                }
            }
        } 
    } 
    if($assemblyInfo) {
        #Write-Host "found assembly info:" $assemblyInfoPath
        $assemblyVersionPattern = '\[\s*assembly\s*:\s*(System\.Reflection\.)?AssemblyVersion\s*\(["\d\.\*]+\)\]'
        $assemblyFileVersionPattern = '\[\s*assembly\s*:\s*(System\.Reflection\.)?AssemblyFileVersion\s*\(["\d\.\*]+\)\]'
        $assemblyInformationalVersionPattern = '\[\s*assembly\s*:\s*(System\.Reflection\.)?AssemblyInformationalVersion\s*\(["\w\.\*\-\+_]+\)\]'
        if($assemblyInfo -cmatch $assemblyVersionPattern) {
            $assemblyInfo = $assemblyInfo -replace $assemblyVersionPattern, "[assembly: System.Reflection.AssemblyVersion(`"$buildVersion`")]"
            $assemblyInfo = $assemblyInfo -replace $assemblyFileVersionPattern, "[assembly: System.Reflection.AssemblyFileVersion(`"$buildVersion`")]"

            if($assemblyInfo -cmatch $assemblyInformationalVersionPattern) {
                $assemblyInfo = $assemblyInfo -replace $assemblyInformationalVersionPattern, "[assembly: System.Reflection.AssemblyInformationalVersion(`"$semanticVersion`")]"
            } else {
                $newLine = [Environment]::NewLine
                $assemblyInfo += "$newLine[assembly: System.Reflection.AssemblyInformationalVersion(`"$semanticVersion`")]"
            }

            Set-Content -Path $assemblyInfoPath -Value $assemblyInfo -Encoding UTF8
            Write-Host "Updated AssemblyInfo:" $assemblyInfoPath
            return $true
        }
    }
    return $false
}

function UpdateVersionInfoRecursive($folder) {
    $hasCsproj = $false
    Get-ChildItem -Path "$folder\*" -include *.csproj | Foreach-Object {
        $hasCsproj = $true
        $result = UpdateNetCoreCsproj $_
    }
    $result = UpdateAssemblyInfo $folder
    if(!$hasCsproj) {
        Get-ChildItem -Path $folder -directory | Foreach-Object {
            $folderName = $_.Name
            if($folderName -like "node_modules" `
                -or $folderName -like "bin" `
                -or $folderName -like "obj" `
                -or $folderName -like "wwwwroot" `
                -or $folderName -like "packages") {
                    # nope
                }
            else {
                UpdateVersionInfoRecursive $_.FullName
            }
        }
    }
}

if($updateVersionInfo) {
    UpdateVersionInfoRecursive "."  # start from current folder
}