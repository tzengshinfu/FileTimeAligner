using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;

//var fileInfo = ShellFile.FromFilePath("\\\\personalcloud.local\\Public\\miki_ipad\\相機膠捲\\IMG_0127.JPG");
//var fileInfo = ShellFile.FromFilePath("\\\\usun-filesvr.usuntek.com\\TW-資訊系統部\\資訊系統部內共用分享\\03.個人文件\\y1938\\image_20240115_133546.png");
//var fileInfo = ShellFile.FromFilePath("\\\\usun-filesvr.usuntek.com\\TW-資訊系統部\\資訊系統部內共用分享\\20220721-SmartIT會議.mp4");
var fileInfo = ShellFile.FromFilePath("\\\\personalcloud.local\\Public\\Liu 的 iPad (001416616837)\\相機膠卷\\IMG_0260.MOV");
var fileName = fileInfo.Name;
var regex = new Regex("(\\d{4}).{0,1}(\\d{2}).{0,1}(\\d{2}).{0,1}(\\d{2}).{0,1}(\\d{2}).{0,1}(\\d{2})");


//Console.WriteLine($"FileName: {oFileInfo.Name}");

var properties = fileInfo.Properties.DefaultPropertyCollection;
//oFileInfo.Properties.GetProperty()
foreach (var property in properties)
{
    var key = new Func<string>(() =>
    {
        if (property.PropertyKey.FormatId == new Guid("46802c11-ada9-41b7-8ebe-65ba6699358b"))
        {
            return "Media_FormatCompliance";
        }
        else if (property.PropertyKey.FormatId == new Guid("9e5e05ac-1936-4a75-94f7-4704b8b01923"))
        {
            return "UnknownProperty";

        }
        else if (property.Description.DisplayName == null)
        {
            return property.CanonicalName;
        }

        return property.Description.DisplayName;
    })();
    var value = new Func<string>(() =>
    {
        if (property.ValueAsObject == null)
        {
            return "無";
        }

        return property.ValueAsObject.ToString();
    })();

    if (property.ValueType.ToString().Contains("System.DateTime")) {
Console.WriteLine($"{"property.Description.DisplayName"}=>{property.Description.DisplayName}");
Console.WriteLine($"{"property.CanonicalName"}=>{property.CanonicalName}");
    Console.WriteLine($"{key}=>{value}");
    //property.Description.DisplayName=>拍攝日期
    //property.CanonicalName=>System.Photo.DateTaken
    //拍攝日期=>2013/6/15 下午 06:34:12
    //property.Description.DisplayName=>建立的媒體
    //property.CanonicalName=>System.Media.DateEncoded
    //建立的媒體=>2013/4/22 下午 06:04:33
    //(\d{4}).{0,1}(\d{2}).{0,1}(\d{2}).{0,1}(\d{2}).{0,1}(\d{2}).{0,1}(\d{2})
    }
}
