using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileTreeControl;
public class NPFileInfo : IPathInfo
{
    public event EventHandler<IPathInfo>? NameChanged;
    public System.Windows.Media.Brush ForeColor { get; set; } = System.Windows.Media.Brushes.White;
    public FileSystemInfo? fileInfo { get; set; }
    public string Dir { get; set; }
    public DateTime Date { get; set; }
    public string Path { get; set; } = "";
    private string name = "";
    public string Name
    {
        get => name;
        set
        {
            name = value;
            //NameChanged?.Invoke(this, this);
            //Debug.WriteLine(name);
        }
    }

    public NPFileInfo()
    {

    }
    public string Size { get; set; }
    public long Length { get; set; } = 0;
    //public Image FolderIcon { get; set; }
    public ImageSource FileIcon { get; set; }
    public string Type { get; set; } = "File";
    public string FileNameNoExtension
    {
        get { return System.IO.Path.GetFileNameWithoutExtension(Name); }
    }

    public NPFileInfo(FileInfo fi)
    {
        Date = fi.LastWriteTime;
        Path = fi.FullName;
        Name = fi.Name;
        Length = fi.Length;
        Size = fi.Length.ToSizeString();
        fileInfo = fi;
        Dir = System.IO.Path.GetDirectoryName(Path);
        var icon = ExtractIcon(fi.FullName);
        using (Bitmap bmp = icon.ToBitmap())
        {
            var stream = new MemoryStream();
            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            FileIcon = BitmapFrame.Create(stream);
        }
    }

    protected void OnNameChanged()
    {
        NameChanged?.Invoke(this, null);
    }

    public Icon ExtractIcon(string path)
    {
        System.Drawing.Icon? icon = SystemIcons.WinLogo;
        try
        {
            icon = System.Drawing.Icon.ExtractAssociatedIcon(path);
        }
        catch (Exception)
        {
        }
        return icon == null ? SystemIcons.WinLogo : icon;
    }
}
