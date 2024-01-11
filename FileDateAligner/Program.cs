using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using System.Reflection;
using System.Windows.Forms.VisualStyles;

var fileInfo = ShellFile.FromFilePath("\\\\personalcloud.local\\Public\\miki_ipad\\相機膠捲\\IMG_0127.JPG");

//Console.WriteLine($"FileName: {oFileInfo.Name}");

var properties = fileInfo.Properties.DefaultPropertyCollection;
//oFileInfo.Properties.GetProperty()
foreach (var property in properties)
{
    if (property.PropertyKey.FormatId == new Guid("46802c11-ada9-41b7-8ebe-65ba6699358b"))
    {
        Console.WriteLine($"{("Media_FormatCompliance")}=>{(property.ValueAsObject == null ? "無" : property.ValueAsObject.ToString())}");
    }
    else
    {
        Console.WriteLine($"{(property.Description.DisplayName == null ? property.CanonicalName : property.Description.DisplayName)}=>{(property.ValueAsObject == null ? "無" : property.ValueAsObject.ToString())}");
    }

}
