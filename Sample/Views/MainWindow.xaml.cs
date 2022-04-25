using System.Windows;
using ModernWpf;
using Sample.Helper;
using Sample.ViewModels;

namespace Sample.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = MainViewModel.CreateInstance();
    }

    private void OnThemeButtonClick(object sender, RoutedEventArgs e)
    {
        DispatcherHelper.RunOnMainThread(() =>
        {
            if (ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
            }
        });
    }
}