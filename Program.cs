using CommandLine;
using Microsoft.WindowsAPICodePack.Shell;
using System.Text.RegularExpressions;

Parser.Default.ParseArguments<Options>(args).WithParsed(option =>
{
    Directory.EnumerateFiles(option.DirectoryPath, "*", option.AllDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList().ForEach(filePath =>
    {
        var mimeType = MimeMapping.MimeUtility.GetMimeMapping(filePath);
        if (!option.MimeType.Split(";").Contains(mimeType.Split("/")[0]))
        {
            return;
        }

        var fileInfo = ShellFile.FromFilePath(filePath);
        // 降低修改時間精度到秒級
        var lastWriteTime = DateTime.Parse(fileInfo.Properties.System.DateModified.Value.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        var dateFinder = new Regex("(\\d{4}).{0,1}(\\d{2}).{0,1}(\\d{2}).{0,1}(\\d{2}).{0,1}(\\d{2}).{0,1}(\\d{2})");
        var findResult = dateFinder.Match(fileInfo.Name);
        if (option.FileName && findResult.Success)
        {
            var fileTime = DateTime.Parse(findResult.Groups[1].Value + "/" + findResult.Groups[2].Value + "/" + findResult.Groups[3].Value + " " + findResult.Groups[4].Value + ":" + findResult.Groups[5].Value + ":" + findResult.Groups[6].Value);
            if (fileTime < lastWriteTime && fileTime > DateTime.Parse(option.MinimumDate))
            {
                File.SetLastWriteTime(fileInfo.Path, fileTime);
                return;
            }
        }

        var mediaTime = GetMediaTime(fileInfo, option.PropertyKeys.Split(";"));
        if (mediaTime < lastWriteTime && mediaTime > DateTime.Parse(option.MinimumDate))
        {
            File.SetLastWriteTime(fileInfo.Path, mediaTime);
            return;
        }
    });
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
    [Value(0, MetaName = "directory path", HelpText = "要對齊檔案時間的目錄路徑", Required = true)]
    public required string DirectoryPath { get; set; }

    [Option('a', "all-directories", Required = false, HelpText = "掃瞄子目錄")]
    public bool AllDirectories { get; set; }

    [Option('t', "mime-type", Required = false, HelpText = "要對齊的檔案MIME類型", Default = "image;video")]
    public required string MimeType { get; set; }

    [Option('p', "property-keys", Required = false, HelpText = "作為對齊來源的時間屬性名稱，以分號區隔", Default = "System.Photo.DateTaken;System.Media.DateEncoded")]
    public required string PropertyKeys { get; set; }

    [Option('f', "file-name", Required = false, HelpText = "檔名若符合時間格式則先使用")]
    public bool FileName { get; set; }

    [Option('m', "minimum-date", Required = false, HelpText = "忽略小於此日期的時間屬性值", Default = "1970/01/01")]
    public required string MinimumDate { get; set; }
}
