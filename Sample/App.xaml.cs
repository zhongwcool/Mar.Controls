using System.Windows;
using System.Windows.Media;
using ModernWpf;
using Sample.Data;
using Sample.Helper;

namespace Sample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly AppConfig _config = AppConfig.CreateInstance();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        //加载配置
        DispatcherHelper.RunOnMainThread(() =>
        {
            var theme = _config.GetValue(Section.Theme, "AppTheme");
            ThemeManager.Current.ApplicationTheme = theme switch
            {
                "Dark" => ModernWpf.ApplicationTheme.Dark,
                "Light" => ModernWpf.ApplicationTheme.Light,
                _ => null
            };

            var accent = _config.GetValue(Section.Theme, "AccentColor");
            if (!string.IsNullOrEmpty(accent) && accent.StartsWith("#"))
            {
                ThemeManager.Current.AccentColor = (Color)ColorConverter.ConvertFromString(accent)!;
            }
            else
            {
                ThemeManager.Current.AccentColor = null;
                _config.SetValue(Section.Theme, "AccentColor", "");
            }
        });
    }

    public static bool IsMultiThreaded { get; } = false;
}