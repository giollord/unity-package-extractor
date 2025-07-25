# unity-package-extractor
Extractor for Unity packages (files with extension *.unitypackage) on .Net 9.0. Have option to extract *.meta files and preview images if they are present.

## Requirements on Windows:
- .Net 7.0 Runtime (can be downloaded [on Microsoft website](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)).<br/>
  If you don't have it and don't want to install .Net 9.0, then just use `win-x86-full.zip` or `win-x64-full.zip` from [latest release](https://github.com/giollord/unity-package-extractor/releases/latest)

## Usage on Windows:
1. Download [latest release](https://github.com/giollord/unity-package-extractor/releases/latest) for x86 or x64 and unpack .exe file.
2. Place your *.unitypackage files (they can be inside other folders as well) near executable. If you want to copy Unity packages which you downloaded through Unity Editor, you can find them in `%AppData%\Unity\Asset Store-5.x\`, or just run application with `-s "%AppData%\Unity\Asset Store-5.x"` parameter.
3. Run `UnityPackageExtractor.exe` and wait for it to finish. Optionally, you can add parameters, like `UnityPackageExtractor.exe -m -p -s "%AppData%\Unity\Asset Store-5.x"`:
    - `--help` prints help menu, `UnityPackageExtractor.exe --help`
    - `-s [path]` or `--source [path]` specify directory where assets are located. For example, command `UnityPackageExtractor.exe -s "%AppData%\Unity\Asset Store-5.x"` will start extracting all assets, downloaded from Unity Asset Store
    - `-d [path]` or `--destination [path]` specify base directory to extract assets
    - `-m` or `--meta` extract *.meta files as well
    - `-p` or `--preview` extract preview images as well (if present)
    - `-i` or `--inPlace` ignore --destination and extract assets to same directory, where *.unitypackage file is located. It is not recommended to use this flag if you set --source to Asset Store downloads folder
4. If you need to stop execution, can just press Ctrl+C
