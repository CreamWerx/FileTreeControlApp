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

namespace FileTreeUI;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    public string FolderPath { get; set; } = @"C:\Users";
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnFolderPathChanged(string newPath = "")
    {
        fileTree.UserNavigate(newPath == ""? FolderPath : newPath);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        //fileTree.Init();
        //OnFolderPathChanged();
    }

    private void tbPath_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            OnFolderPathChanged(tbPath.Text);
        }
    }

    
}