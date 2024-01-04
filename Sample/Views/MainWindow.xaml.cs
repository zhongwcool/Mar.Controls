using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Mar.Controls.Tool;
using ModernWpf;
using Sample.Helper;
using Sample.ViewModels;
using Serilog;

namespace Sample.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = MainViewModel.CreateInstance();

        Task.Delay(500).ContinueWith(_ =>
        {
            Dispatcher.Invoke(() =>
            {
                var console = new ConsoleWindow(this)
                {
                    Capacity = 2000,
                    PrintHello = true
                };
                console.Show();
            });
        });
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var sb = Resources["CloseMenu"] as Storyboard;
        sb?.Begin(RightMenu);

        Log.Debug("Close Menu");
    }

    private void BtnTest_OnClick(object sender, RoutedEventArgs e)
    {
        var sb = Resources["OpenMenu"] as Storyboard;
        sb?.Begin(RightMenu);

        Log.Debug("Open Menu");
    }

    private void OnThemeButtonClick(object sender, RoutedEventArgs e)
    {
        DispatcherHelper.RunOnMainThread(() =>
        {
            if (ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark)
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                Log.Debug("Theme Change to Light");
            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                Log.Debug("Theme Change to Dark");
            }
        });
    }
}