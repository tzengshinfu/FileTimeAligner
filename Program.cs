using CommandLine;
using Microsoft.WindowsAPICodePack.Shell;
using System.Text.RegularExpressions;

Parser.Default.ParseArguments<Options>(args).WithParsed(option =>
{
    var now = DateTime.Now.ToString("yyyyMMdd_HHmmss");
    using var logWriter = File.CreateText($"FileTimeAligner-{now}.log");
    logWriter.WriteLine("FilePath\tLastWriteTime\tAlignFrom\tAlignTime\tResult\tMessage");

    Directory.EnumerateFiles(option.DirectoryPath, "*", option.AllDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList().ForEach(filePath =>
    {
        try
        {
            var mimeType = MimeMapping.MimeUtility.GetMimeMapping(filePath);
            if (!option.MimeType.Split(";").Contains(mimeType.Split("/")[0]))
            {
                return;
            }

            var fileInfo = ShellFile.FromFilePath(filePath);
            // 降低修改時間精度到秒級
            var lastWriteTime = DateTime.Parse(fileInfo.Properties.System.DateModified.Value.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            var fileNameTimeFinder = new Regex("(\\d{4})\\D{0,1}(\\d{2})\\D{0,1}(\\d{2})\\D{0,1}(\\d{2})\\D{0,1}(\\d{2})\\D{0,1}(\\d{2})");
            var fileNameTimeFindResult = fileNameTimeFinder.Match(fileInfo.Name);
            if (option.FileName && fileNameTimeFindResult.Success)
            {
                if (DateTime.TryParse(fileNameTimeFindResult.Groups[1].Value + "/" + fileNameTimeFindResult.Groups[2].Value + "/" + fileNameTimeFindResult.Groups[3].Value + " " + fileNameTimeFindResult.Groups[4].Value + ":" + fileNameTimeFindResult.Groups[5].Value + ":" + fileNameTimeFindResult.Groups[6].Value, out DateTime fileNameTime))
                {
                    if (fileNameTime < lastWriteTime && fileNameTime >= DateTime.Parse(option.MinimumDate))
                    {
                        File.SetLastWriteTime(fileInfo.Path, fileNameTime);
                        logWriter.WriteLine($"{filePath}\t{lastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")}\tFileName\t{fileNameTime.ToString("yyyy-MM-dd HH:mm:ss")}\tSuccess\t");
                        return;
                    }
                }
            }

            var unixTimestampFinder = new Regex("(\\d{10})(\\d{3})?");
            var unixTimestampFindResult = unixTimestampFinder.Match(fileInfo.Name);
            if (option.FileName && unixTimestampFindResult.Success)
            {
                var fileNameTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(double.Parse(unixTimestampFindResult.Groups[1].Value)).ToLocalTime();
                if (fileNameTime < lastWriteTime && fileNameTime >= DateTime.Parse(option.MinimumDate))
                {
                    File.SetLastWriteTime(fileInfo.Path, fileNameTime);
                    logWriter.WriteLine($"{filePath}\t{lastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")}\tFileName\t{fileNameTime.ToString("yyyy-MM-dd HH:mm:ss")}\tSuccess\t");
                    return;
                }
            }

            var mediaPropertyTime = GetMediaPropertyTime(fileInfo, option.PropertyKeys.Split(";"), option.MinimumDate);
            if (mediaPropertyTime.Value < lastWriteTime)
            {
                File.SetLastWriteTime(fileInfo.Path, mediaPropertyTime.Value);
                logWriter.WriteLine($"{filePath}\t{lastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")}\t{mediaPropertyTime.Key}\t{mediaPropertyTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}\tSuccess\t");
                return;
            }

            logWriter.WriteLine($"{filePath}\t{lastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")}\tNone\tNone\tNone\t");
        }
        catch (Exception ex)
        {
            logWriter.WriteLine($"{filePath}\tNone\tNone\tNone\tFail\t{ex.Message}");
        }
    });
});

static KeyValuePair<string, DateTime> GetMediaPropertyTime(ShellFile fileInfo, string[] propertyKeys, string minimumDate)
{
    var propertyTimes = new Dictionary<string, DateTime> { };
    var properties = fileInfo.Properties.DefaultPropertyCollection;
    foreach (var propertyKey in propertyKeys)
    {
        var timeProperty = properties.Where(p => p.CanonicalName == propertyKey).SingleOrDefault();
        if (timeProperty != null && timeProperty.ValueAsObject != null)
        {
            var time = DateTime.Parse(timeProperty.ValueAsObject.ToString());
            if (time >= DateTime.Parse(minimumDate))
            {
                propertyTimes.Add(propertyKey, time);
            }
        }
    }

    var mediaTime = propertyTimes.Count > 0 ? propertyTimes.MinBy(t => t.Value) : new KeyValuePair<string, DateTime>("NoPropertyKey", DateTime.MaxValue);
    return mediaTime;
}

public class Options
{
    [Value(0, MetaName = "directory path", HelpText = "要對齊檔案時間的目錄路徑", Required = true)]
    public required string DirectoryPath { get; set; }

    [Option('a', "all-directories", Required = false, HelpText = "掃瞄子目錄")]
    public bool AllDirectories { get; set; }

    [Option('t', "mime-type", Required = false, HelpText = "要對齊的檔案MIME類型，以分號區隔", Default = "image;video")]
    public required string MimeType { get; set; }

    [Option('p', "property-keys", Required = false, HelpText = "作為對齊來源的時間屬性名稱，以分號區隔", Default = "System.Photo.DateTaken;System.Media.DateEncoded")]
    public required string PropertyKeys { get; set; }

    [Option('f', "file-name", Required = false, HelpText = "檔案名稱若符合時間格式(yyyyMMddHHmmss)則優先使用")]
    public bool FileName { get; set; }

    [Option('m', "minimum-date", Required = false, HelpText = "忽略小於此日期的時間屬性值", Default = "1970/01/01")]
    public required string MinimumDate { get; set; }
}
