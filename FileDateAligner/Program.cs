using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using System.Reflection;
using System.Windows.Forms.VisualStyles;

var oFileInfo = ShellFile.FromFilePath("D:\\Users\\y1938\\Desktop\\IMG_0552.MOV");

//Console.WriteLine($"FileName: {oFileInfo.Name}");

var c = oFileInfo.Properties.DefaultPropertyCollection;
//oFileInfo.Properties.GetProperty()
foreach (var a in c)
{
    if (a.PropertyKey.FormatId == new Guid("46802c11-ada9-41b7-8ebe-65ba6699358b"))
    {
        Console.WriteLine($"{("Media_FormatCompliance")}=>{(a.ValueAsObject == null ? "無" : a.ValueAsObject.ToString())}");
    }
    else
    {
        Console.WriteLine($"{(a.Description.DisplayName == null ? a.CanonicalName : a.Description.DisplayName)}=>{(a.ValueAsObject == null ? "無" : a.ValueAsObject.ToString())}");
    }

}
