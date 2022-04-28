using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
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

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var sb = Resources["CloseMenu"] as Storyboard;
        sb?.Begin(RightMenu);
    }

    private void BtnTest_OnClick(object sender, RoutedEventArgs e)
    {
        var sb = Resources["OpenMenu"] as Storyboard;
        sb?.Begin(RightMenu);
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