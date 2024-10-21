using System.IO;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using Icon = System.Drawing.Icon;
using Image = System.Windows.Controls.Image;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileTreeControl;
public class NPTreeInfo : TreeViewItem, IPathInfo
{
    public event EventHandler<IPathInfo>? NameChanged;
    public event EventHandler<NPTreeInfo>? TreeItemClick;

    const string openFolderPath = @"Images\openFolder.png";
    const string closedFolderPath = @"Images\closedFolder.png";
    private string name = "";
    StackPanel headerPanel = new StackPanel();
    Image icon = new Image();
    TextBlock headerText = new TextBlock();

    public System.Windows.Media.Brush ForeColor { get; set; } = System.Windows.Media.Brushes.Gold;
    public FileSystemInfo? dirInfo { get; set; }
    public DateTime Date { get; set; }
    public string Path { get; set; } = "";
    public string Dir { get; set; }
    public string ImageSource { get; set; }
    public bool IsOpen { get; set; } = false;
    public string Name
    {
        get => name;
        set
        {
            name = value;
            Header = name;
            //OnNameChanged();
        }
    }
    public long Length { get; set; } = 0;
    public string Size { get; set; } = "0";
    public ImageSource FileIcon { get; set; }
    public Image FolderIcon { get; set; } = new Image();
    public string Type { get; set; } = "Directory";

    public NPTreeInfo()
    {

    }
    public NPTreeInfo(DirectoryInfo di, bool isExpanded = false)
    {
        Date = di.LastWriteTime;
        Path = di.FullName;
        Name = di.Name.TrimEnd('\\');
        dirInfo = di;
        Dir = di.FullName;
        ForeColor = Brushes.Gold;
        FileIcon = ImageSourceFromIcon(DefaultIcons.FolderLarge);
        Expanded += NPDirInfo_Expanded;
        Collapsed += NPDirInfo_Collapsed;

        if (!isExpanded)
        {
            ImageSource = closedFolderPath;
        }
        else
        {
            ImageSource = openFolderPath;
        }
        IsOpen = isExpanded;

        headerPanel.Orientation = Orientation.Horizontal;
        headerPanel.MouseUp += HeaderPanel_MouseUp;

        icon.Source = new BitmapImage(new Uri(ImageSource, UriKind.Relative));
        icon.Width = 18;

        headerText.Foreground = Brushes.Silver;
        headerText.Margin = new Thickness(10, 0, 0, 0);
        headerText.Text = Name;

        headerPanel.Children.Add(icon);
        headerPanel.Children.Add(headerText);

        Header = headerPanel;
    }

    private void NPDirInfo_Collapsed(object sender, RoutedEventArgs e)
    {
        //Debug.WriteLine($"osource: {e.OriginalSource}");
        if (e.OriginalSource == this)
        {
            //Debug.WriteLine($"{Name} collaped");
            ImageSource = closedFolderPath;
            icon.Source = new BitmapImage(new Uri(ImageSource, UriKind.Relative));
            icon.Width = 18;
            //Debug.WriteLine($"{icon.Source}");
        }
        //e.Handled = true;
    }
    private void NPDirInfo_Expanded(object sender, RoutedEventArgs e)
    {
        //Debug.WriteLine($"osource: {e.OriginalSource}");
        if (e.OriginalSource == this)
        {
            //Debug.WriteLine($"{Name} expanded");
            ImageSource = openFolderPath;
            icon.Source = new BitmapImage(new Uri(ImageSource, UriKind.Relative));
            icon.Width = 18;
        }

    }
    private void HeaderPanel_MouseUp(object sender, MouseButtonEventArgs e)
    {
        //Debug.WriteLine("HeaderPanel_MouseUp");
        if (e.ChangedButton == MouseButton.Left)
        {

            TreeItemClick?.Invoke(this, this);
        }

        //if (e.ChangedButton != MouseButton.Left)
        //{
        //    return;
        //}
        //NPDirInfo tvi = (NPDirInfo)sender;
        //if (tvi.IsSelected)
        //{
        //    //var directories = MainWindow.TreeViewItemRefresh(tvi);
        //    if (tvi.HasItems)
        //    {
        //        tvi.IsExpanded = true;
        //    }
        //    //Dispatcher.Invoke(()=> MainWindow. PopulateGrid(tvi, directories));
        //}

        //Debug.WriteLine(ImageSource);
    }
    public override string ToString()
    {
        return Name;//.TrimEnd(['\\', ':']);

    }
    public ImageSource ImageSourceFromIcon(Icon icon)
    {
        ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

        return imageSource;
    }
}

public class NPDirInfo : IPathInfo
{
    public event EventHandler<IPathInfo>? NameChanged;
    private string name = "";
    
    public System.Windows.Media.Brush ForeColor { get; set; } = System.Windows.Media.Brushes.Gold;
    public FileSystemInfo? dirInfo { get; set; }
    public DateTime Date { get; set; }
    public string Path { get; set; } = "";
    public string Dir { get; set; }
    public string ImageSource { get; set; }
    public bool IsOpen { get; set; } = false;
    public string Name
    {
        get => name;
        set
        {
            name = value;
        }
    }
    public long Length { get; set; } = 0;
    public string Size { get; set; } = "0";
    public ImageSource FileIcon { get; set; }
    public Image FolderIcon { get; set; } = new Image();
    public string Type { get; set; } = "Directory";

    public NPDirInfo()
    {

    }
    public NPDirInfo(DirectoryInfo di, bool isExpanded = false)
    {
        Date = di.LastWriteTime;
        Path = di.FullName;
        Name = di.Name.TrimEnd('\\');
        dirInfo = di;
        Dir = di.FullName;
        ForeColor = Brushes.Gold;
        FileIcon = ImageSourceFromIcon(DefaultIcons.FolderLarge);
        
    }

    public override string ToString()
    {
        return Name;//.TrimEnd(['\\', ':']);

    }
    public ImageSource ImageSourceFromIcon(Icon icon)
    {
        ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

        return imageSource;
    }
}
