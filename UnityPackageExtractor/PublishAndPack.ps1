$pubProfiles = 'Properties/PublishProfiles'
$pubProfilesCfg = (@"
[
    { "pubxml": "FolderProfile-x86.pubxml", "outDir": "bin/Release/net7.0/publish/win-x86", "archiveName": "win-x86.zip" },
    { "pubxml": "FolderProfile-x64.pubxml", "outDir": "bin/Release/net7.0/publish/win-x64", "archiveName": "win-x64.zip" },
    { "pubxml": "FolderProfile-x86-full.pubxml", "outDir": "bin/Release/net7.0/publish/win-x86-full", "archiveName": "win-x86-full.zip" },
    { "pubxml": "FolderProfile-x64-full.pubxml", "outDir": "bin/Release/net7.0/publish/win-x64-full", "archiveName": "win-x64-full.zip" }
]
"@ | ConvertFrom-Json)
$filesToZip = @('UnityPackageExtractor.exe')
$zipPath = 'bin/Release/net7.0/publish'

foreach($cfg in $pubProfilesCfg) {
    dotnet publish "UnityPackageExtractor.csproj" /p:PublishProfile="$pubProfiles/$($cfg.pubxml)"

    Compress-Archive -Path ($filesToZip | ForEach-Object { "$($cfg.outDir)/$_" }) -DestinationPath "$zipPath/$($cfg.archiveName)"
}