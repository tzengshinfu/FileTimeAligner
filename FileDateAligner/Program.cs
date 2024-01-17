using Microsoft.WindowsAPICodePack.Shell;
using System.Text.RegularExpressions;

Directory.EnumerateFiles("D:\\Users\\y1938\\Documents\\Desktop", "*", SearchOption.AllDirectories).ToList().ForEach(filePath =>
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

static DateTime GetMediaTime(ShellFile fileInfo, String[] propertyKeys)
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
