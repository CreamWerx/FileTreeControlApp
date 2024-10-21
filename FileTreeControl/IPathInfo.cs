using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FileTreeControl;
public interface IPathInfo
{
    public event EventHandler<IPathInfo> NameChanged;
    public DateTime Date { get; set; }
    public string Path { get; set; }
    public string Dir { get; set; }
    public string Name { get; set; }
    public ImageSource FileIcon { get; set; }
    //public Image FolderIcon { get; set; }
    public string Type { get; set; }
    public long Length { get; set; }
    public string Size { get; set; }
    //public FileInfo fileInfo { get; set; }
    //public DirectoryInfo dirInfo { get; set; }
    public System.Windows.Media.Brush ForeColor { get; set; }
}
