using CommandLine;
using Microsoft.WindowsAPICodePack.Shell;
using System.Text.RegularExpressions;

Parser.Default.ParseArguments<Options>(args).WithParsed(option =>
{
    Directory.EnumerateFiles(option.DirectoryPath, "*", option.AllDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList().ForEach(filePath =>
    {
        var mimeType = MimeMapping.MimeUtility.GetMimeMapping(filePath);
        if (!new string[] { "image", "video", "audio" }.Contains(mimeType.Split("/")[0]))
        {
            return;
        }

        var fileInfo = ShellFile.FromFilePath(filePath);
        // 降低修改時間精度到秒級
        var lastWriteTime = DateTime.Parse(fileInfo.Properties.System.DateModified.Value.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        var dateFinder = new Regex("(\\d{4}).{0,1}(\\d{2}).{0,1}(\\d{2}).{0,1}(\\d{2}).{0,1}(\\d{2}).{0,1}(\\d{2})");
        var findResult = dateFinder.Match(fileInfo.Name);
        if (findResult.Success)
        {
            var fileTime = DateTime.Parse(findResult.Groups[1].Value + "/" + findResult.Groups[2].Value + "/" + findResult.Groups[3].Value + " " + findResult.Groups[4].Value + ":" + findResult.Groups[5].Value + ":" + findResult.Groups[6].Value);
            if (fileTime < lastWriteTime)
            {
                File.SetLastWriteTime(fileInfo.Path, fileTime);
            }
        }
        else
        {
            var mediaTime = GetMediaTime(fileInfo, ["System.Photo.DateTaken", "System.Media.DateEncoded"]);
            if (mediaTime < lastWriteTime && mediaTime > DateTime.Parse("2000/01/01"))
            {
                File.SetLastWriteTime(fileInfo.Path, mediaTime);
            }
        }
    });

    if (option.AllDirectory)
    {
        Console.WriteLine($"Verbose output enabled. Current Arguments: -v {option.AllDirectory}");
        Console.WriteLine("Quick Start Example! App is in Verbose mode!");
    }
    else
    {
        Console.WriteLine($"Current Arguments: -v {option.AllDirectory}");
        Console.WriteLine("Quick Start Example!");
    }
});



static DateTime GetMediaTime(ShellFile fileInfo, string[] propertyKeys)
{
    var times = new List<DateTime> { DateTime.MaxValue };
    var properties = fileInfo.Properties.DefaultPropertyCollection;

    foreach (var propertyKey in propertyKeys)
    {
        var time = DateTime.MaxValue;
        var timeProperty = properties.Where(p => p.CanonicalName == propertyKey).SingleOrDefault();
        if (timeProperty != null && timeProperty.ValueAsObject != null)
        {
            time = DateTime.Parse(timeProperty.ValueAsObject.ToString());
            times.Add(time);
        }
    }

    var mediaTime = times.Min();
    return mediaTime;
}

public class Options
{
    [Option('a', "all-directory", Required = false, HelpText = "掃瞄子目錄", Default = false)]
    public bool AllDirectory { get; set; }

    [Option('m', "minimum-date", Required = false, HelpText = "忽略小於此日期的時間屬性值", Default = "2000/01/01")]
    public string? MinimumDate { get; set; }

    [Option('p', "property-keys", Required = false, HelpText = "屬性值名稱，以分號區隔", Default = "System.Photo.DateTaken;System.Media.DateEncoded")]
    public string? PropertyKeys { get; set; }

    [Option('f', "file-name", Required = false, HelpText = "檔名符合時間格式優先取代屬性值", Default = true)]
    public bool FileName { get; set; }

    [Option('t', "mime-type", Required = false, HelpText = "MIME類型", Default = "image;video")]
    public string? MimeType { get; set; }

    [Value(0, MetaName = "directory path", HelpText = "要進行對齊的目錄路徑", Required = true)]
    public required string DirectoryPath { get; set; }
}
