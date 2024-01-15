using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using System.Reflection;
using System.Windows.Forms.VisualStyles;

//var fileInfo = ShellFile.FromFilePath("\\\\personalcloud.local\\Public\\miki_ipad\\相機膠捲\\IMG_0127.JPG");
var fileInfo = ShellFile.FromFilePath("\\\\usun-filesvr.usuntek.com\\TW-資訊系統部\\資訊系統部內共用分享\\03.個人文件\\y1938\\image_20240115_133546.png");
//var fileInfo = ShellFile.FromFilePath("\\\\usun-filesvr.usuntek.com\\TW-資訊系統部\\資訊系統部內共用分享\\20220721-SmartIT會議.mp4");

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
    }
}
