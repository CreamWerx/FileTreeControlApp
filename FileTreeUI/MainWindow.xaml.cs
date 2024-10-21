using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace FileTreeUI;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
/// 
[ObservableObject]
public partial class MainWindow : Window
{
    [ObservableProperty]
    private string folderPath = @"C:\Users";

    //public string FolderPath { get => folderPath; set => folderPath = value; }
    public MainWindow()
    {
        InitializeComponent();
    }

    private void FolderPathChanged(string newPath = "")
    {
        fileTree.UserNavigate(newPath == ""? FolderPath : newPath);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        fileTree.ErrorOccurred += FileTree_ErrorOccurred;
        fileTree.PathChanged += FileTree_PathChanged;
    }

    private void FileTree_PathChanged(object? sender, string e)
    {
        FolderPath = e;
    }

    private void FileTree_ErrorOccurred(object? sender, string e)
    {
        MessageBox.Show(e, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void tbPath_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            FolderPathChanged(tbPath.Text);
        }
    }
}