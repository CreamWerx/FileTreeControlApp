using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using Path = System.IO.Path;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FileTreeControl;
/// <summary>
/// Interaction logic for UserControl1.xaml
/// </summary>
/// 

[ObservableObject]
public partial class FileTree : UserControl
{
    public event EventHandler<string>? ErrorOccurred;
    public event EventHandler<string>? FileOpenRequest;
    public event EventHandler<(string OldPath, string NewPath)>? FileRenameRequest;
    public event EventHandler<(string OldPath, string NewPath)>? FolderRenameRequest;
    [ObservableProperty]
    List<IPathInfo>? _currentContentList;
    //public List<IPathInfo>? CurrentContentList 
    //{ 
    //    get => _currentContentList; 
    //    set => _currentContentList = value; 
    //}
    public IPathInfo CurrentGridSelection { get; set; } = new NPFileInfo();
    public FileTree()
    {
        InitializeComponent();
    }

    private IPathInfo CreateDirItem(string folder)
    {
        var dirItem = new NPDirInfo(new DirectoryInfo(folder));
        dirItem.NameChanged += DirItem_NameChanged;
        return dirItem;
    }
    private IPathInfo CreateFileItem(string file)
    {
        var fileItem = new NPFileInfo(new FileInfo(file));
        fileItem.NameChanged += FileItem_NameChanged;
        return fileItem;
    }
    private NPTreeInfo CreateTreeItem(string path)
    {
        NPTreeInfo treeItem = new NPTreeInfo(new DirectoryInfo(path));
        treeItem.TreeItemClick += TreeItem_TreeItemClick;
        return treeItem;
    }
    private NPTreeInfo? GetTVIByHeader(NPTreeInfo tmpTVI, string headerName)
    {
        foreach (var item in tmpTVI.Items)
        {
            var tvi = (NPTreeInfo)item;
            if ((tvi.Name as string) == headerName)
            {
                return tvi;
            }
        }
        return null;
    }

    // Method overloaded tp accommodate first first call.
    private NPTreeInfo? GetTVIByHeader(System.Windows.Controls.TreeView tmpTV, string headerName)
    {
        foreach (var item in tmpTV.Items)
        {
            var tvi = (NPTreeInfo)item;
            Debug.WriteLine(tvi.Name as string);
            if ((tvi.Name as string) == headerName)
            {
                Debug.WriteLine("found");
                return tvi;
            }
        }
        return null;
    }
    private List<IPathInfo> GridRefresh(IPathInfo e)
    {
        var folderContents = ListContents(e);
        List<IPathInfo> pathInfos = [];

        foreach (IPathInfo item in folderContents)
        {
            pathInfos.Add(item);
        }
        return pathInfos;
    }
    private IEnumerable<IPathInfo> ListContents(IPathInfo pathInfo)
    {
        var folders = ListFolders(pathInfo);
        var files = ListFiles(pathInfo);

        ObservableCollection<IPathInfo> folderContents = [];

        if (folders is not null)
        {
            foreach(string folder in folders)
            {
                folderContents.Add(CreateDirItem(folder));
            }
        }
        if (files is not null)
        {
            foreach(string file in files)
            {
                folderContents.Add(CreateFileItem(file));
            }
        }
        return folderContents;
    }
    private void ListDrives()
    {
        //DriveInfo[] drives = DriveInfo.GetDrives();
        try
        {
            var drives = Directory.GetLogicalDrives();
            foreach (var drive in drives)
            {
                NPTreeInfo tvi = CreateTreeItem(drive);
                tree.Items.Add(tvi);
            }
            //return true;
        }
        catch (Exception ex)
        {
            Log(ex.Message);
            //return false;
        }
    }
    private IEnumerable<string>? ListFiles(IPathInfo pathInfo)
    {
        return Directory.EnumerateFiles(pathInfo.Path, "*", new EnumerationOptions { IgnoreInaccessible = true });
    }
    private IEnumerable<string>? ListFolders(IPathInfo pathInfo)
    {
        return Directory.EnumerateDirectories(pathInfo.Path, "*", new EnumerationOptions {IgnoreInaccessible = true});
    }
    private void Log(string message)
    {
        Debug.WriteLine(message);
    }
    private IEnumerable? TreeViewItemRefresh(NPTreeInfo tvi)
    {
        //NPTreeInfo dirInfo = GeTDirItemFromTreeViewItem(tvi);
        var directories = Directory.EnumerateDirectories(tvi.Path, "*", new EnumerationOptions { IgnoreInaccessible = true });
        if (directories is null || !directories.Any())
        {
            return null;
        }
        tvi.Items.Clear();
        foreach (var directory in directories)
        {
            tvi.Items.Add(CreateTreeItem(directory));
        }
        return directories;
    }
    private NPTreeInfo? TreeViewSeekToItem(string path, bool isHistoryNavigation = true)
    {
        var pathSplit = path.Split(Path.DirectorySeparatorChar);
        if (pathSplit is null)
        {
            return null;
        }
        NPTreeInfo? rootTVI = GetTVIByHeader(tree, pathSplit[0]);
        if (rootTVI is null)
        {
            return null;
        }
        //UpdateTreeViewItem(rootTVI);
        rootTVI.IsExpanded = true;

        // Here we have the first NPTreeViewItem in TreeView
        // Clear list and add a CrumbItem
        //crumbList.Clear();
        //crumbList.Add(new CrumbItem(rootTVI));
        bool success = true;
        for (int i = 1; i < pathSplit.Length; i++)
        {
            NPTreeInfo? treeViewItem = GetTVIByHeader(rootTVI, pathSplit[i]);
            if (treeViewItem is null)
            {
                Log("Searching tree failed");
                success = false;
                break;
            }

            // Here we have the next TViewItem in previous item,
            //  so update the TreeView, and expand.
            //HistoryNavigation = true;
            //treeViewItem = TreeViewItemRefresh(treeViewItem);
            TreeViewItemRefresh(treeViewItem);
            //HistoryNavigation = true;
            treeViewItem.IsExpanded = true;
            // Add new crumb to list
            //crumbList.Add(new CrumbItem(treeViewItem));
            rootTVI = treeViewItem;
        }

        // Perhaps GetTVIByHeader() failed
        if (success)
        {
            NPTreeInfo targetItem = rootTVI;

            // IsSelected will cause targetItem to be updated with UpdateTreeViewItem()
            //  via tv_SelectedItemChanged event
            //HistoryNavigation = true;
            targetItem.IsSelected = true;

            // At this point we have the last element in the path and want to ensure it's visible.
            //  but we want as many of targetItem.Items as possible (if any) to be visible.
            // In my case there is space for ~22 items, so utilize them all if possivle,
            //  while ensuring targetItem is still in view (at the top if necessary).
            int count = targetItem.Items.Count;
            //int tvHeight = (int)tv.ActualHeight;
            //int tvItemHeight = (int)((TViewItem)tv.Items[0]).ActualHeight;
            //int maxVisibleItems = tvHeight / tvItemHeight;
            //targetItem.Focus();
            //if (count > 0)
            //{
            //    targetItem = (TreeViewItem)(targetItem.Items[(count < 22) ? count - 1 : 21]);
            //}
            targetItem.BringIntoView();
            return targetItem;
        }
        return null;
    }
    public void UserNavigate(string path)
    {
        if (!Path.Exists(path))
        {
            ErrorOccurred?.Invoke(this, $"The path: {path} does not exist.");
            return;
        }
        TreeViewSeekToItem(path);
        NPTreeInfo nPTreeInfo = CreateTreeItem(path);
        TreeViewItemRefresh(nPTreeInfo);
        CurrentContentList = GridRefresh(nPTreeInfo);
    }

    private void DirItem_NameChanged(object? sender, IPathInfo e)
    {
        throw new NotImplementedException();
    }
    private void FileItem_NameChanged(object? sender, IPathInfo e)
    {
        throw new NotImplementedException();
    }
    private void gridMain_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        var selectedItem = gridMain.SelectedItem as IPathInfo;
        var index = gridMain.SelectedIndex;
        var oldPath = CurrentGridSelection.Path;
        var newPath = System.IO.Path.Combine(Path.GetDirectoryName(selectedItem?.Path),
            (e.EditingElement as System.Windows.Controls.TextBox)?.Text);

        // Was a change made?
        if (oldPath != newPath)
        {//File/Folder name was changed
            var isFile = File.Exists(oldPath);
            var isFolder = Directory.Exists(newPath);
            
                //Is it a file or folder?
                if (isFile)
                {
                    CurrentContentList[index] = new NPFileInfo(new FileInfo(newPath));
                    FileRenameRequest?.Invoke(this, (oldPath, newPath));
                }
                else if (isFolder) 
                {
                    CurrentContentList[index] = new NPDirInfo(new DirectoryInfo(newPath));
                    FolderRenameRequest?.Invoke(this, (oldPath, newPath));
                }
                // for future revert changes
                //RenamedFiles.Add((oldPath, newPath));
                // reset grid items
                gridMain.ItemsSource = null;
                gridMain.ItemsSource = CurrentContentList;
            }
        }
    private void gridMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left) { return; }

        var selectedItem = gridMain.SelectedValue as IPathInfo;
        if (selectedItem is null) { return; }

        CurrentGridSelection = selectedItem;
        var index = gridMain.SelectedIndex;
        var isFile = File.Exists(CurrentGridSelection.Path);
        
        //Is it a file or folder?
        if (isFile) { FileOpenRequest?.Invoke(this, CurrentGridSelection.Path); return; }
        else 
        {
            TreeViewSeekToItem(CurrentGridSelection.Path);
            CurrentContentList = GridRefresh(CurrentGridSelection); 
        }

        
        // reset grid items
        //gridMain.ItemsSource = null;
        //gridMain.ItemsSource = CurrentContentList;

    }
    private void gridMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {

    }
    private void gridMain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {

    }
    private void gridMain_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        CurrentGridSelection = gridMain.SelectedItem as IPathInfo;
        var index = gridMain.SelectedIndex;

    }
    private void TreeItem_TreeItemClick(object? sender, NPTreeInfo e)
    {
        TreeViewItemRefresh(e);
        CurrentContentList = GridRefresh(e);
    }
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        ListDrives();
    }
}


