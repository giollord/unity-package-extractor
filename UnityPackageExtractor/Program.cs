using CommandLine;
using System.Formats.Tar;
using System.IO.Compression;

var parsedArgs = Parser.Default.ParseArguments<CommandLineOptions>(args)?.Value;
if (parsedArgs == null)
    return;

// path to look for assets
var sourcePath = parsedArgs.SourcePath ?? Directory.GetCurrentDirectory();
// destination path; usually assets unpack to directory Assets under destinationPath
var destinationPath = parsedArgs.DestinationPath ?? Directory.GetCurrentDirectory();
// override destination path, so that unity package will be unpacked near unity package itself
var copyToDirectoryNearAsset = parsedArgs.InPlace;
// whether to unpack *.meta files or no
var generateMeta = parsedArgs.GenerateMeta;
// whether to unpack preview images or no
var generatePreview = parsedArgs.Preview;

// looking for all packages in sourcePath and subfolders
var allAssetFiles = Directory.EnumerateFiles(sourcePath, "*.unitypackage", SearchOption.AllDirectories).ToList();
Console.WriteLine($"Found {allAssetFiles.Count} *.unitypackage files.");
var packageCounter = 0;

foreach(var unityPackageFile in allAssetFiles)
{
    Console.WriteLine($"Processing {++packageCounter}/{allAssetFiles.Count} '{Path.GetFileName(unityPackageFile)}' at '{Path.GetDirectoryName(unityPackageFile)}'...");
    var currentDestinationPath = copyToDirectoryNearAsset ? Path.GetDirectoryName(unityPackageFile)! : destinationPath;
    var resultFilePaths = new Dictionary<string, string>();

    // First pass in unitypackage - collecting file names
    Console.Write($"Discovering contents of unity package...");
    using (var fileStream = File.OpenRead(unityPackageFile))
    using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
    using (var tarReader = new TarReader(gzipStream))
    {
        TarEntry? entry;
        while ((entry = tarReader.GetNextEntry()) != null)
        {
            if (entry.EntryType == TarEntryType.Directory || !entry.Name.EndsWith("pathname") || entry.DataStream == null)
                continue;

            using var streamReader = new StreamReader(entry.DataStream, leaveOpen: true);
            var filePath = streamReader.ReadToEnd();
            filePath = filePath.Substring(0, filePath.IndexOf('\n'));

            // getting key of asset - it it's top folder name
            var key = entry.Name.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
            resultFilePaths[key] = filePath;
        }
    }

    // Second pass in unitypackage - unpacking 
    Console.Write($"Found {resultFilePaths.Count} files within asset. Extracting...");
    using (var fileStream = File.OpenRead(unityPackageFile))
    using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
    using (var tarReader = new TarReader(gzipStream))
    {
        TarEntry? entry;
        while ((entry = tarReader.GetNextEntry()) != null)
        {
            if (entry.EntryType == TarEntryType.Directory)
                continue;

            // detecting type of asset - is it asset, meta file or preview image
            var type = entry.Name.Split(new char[] { '\\', '/' }).Last();
            if (type != "asset" && (!generateMeta || type != "asset.meta") && (!generatePreview || !type.StartsWith("preview")))
                continue;

            // getting key of asset - it it's top folder name
            var key = entry.Name.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries)[0];
            var destFilePath = Path.Combine(currentDestinationPath, resultFilePaths[key]);
            if (destFilePath == null)
            {
                Console.WriteLine($"Incorrect asset name, skipping: {resultFilePaths[key]}");
            }

            if (generatePreview && type.StartsWith("preview"))
                destFilePath = Path.Combine(currentDestinationPath, "__Preview", resultFilePaths[key] + Path.GetExtension(type));
            else if (generateMeta && type == "asset.meta")
                destFilePath += ".meta";

            var destDirectory = Path.GetDirectoryName(destFilePath);
            Directory.CreateDirectory(destDirectory!);
            entry.ExtractToFile(destFilePath!, true);
        }
        Console.WriteLine(" DONE");
    }
}

Console.WriteLine("Finished!");


class CommandLineOptions
{
    [Option('s', "source", HelpText = "Directory to search for unity packages")]
    public string? SourcePath { get; set; }
    [Option('d', "destination", HelpText = "Directory where files will be put. Ignored if --inPlace parameter is set")]
    public string? DestinationPath { get; set; }
    [Option('m', "meta", HelpText = "Generate *.meta files")]
    public bool GenerateMeta { get; set; }
    [Option('i', "inPlace", HelpText = "Extract files to directory nearby unity package, instead of using --destination")]
    public bool InPlace { get; set; }
    [Option('p', "preview", HelpText = "Unpack preview images. Previews will be put to '__Previews' folder within --destination folder")]
    public bool Preview { get; set; }
}
