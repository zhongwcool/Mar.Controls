using System.IO;
using System.Windows;
using System.Windows.Media;
using ModernWpf;
using Sample.Data;
using Sample.Helper;
using Serilog;

namespace Sample;

public partial class App : Application
{
    private readonly AppConfig _config = AppConfig.CreateInstance();
    private readonly string _file = Path.Combine("Log", "log.txt");

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        //加载配置
        DispatcherHelper.RunOnMainThread(() =>
        {
            var theme = _config.GetValue(Section.Theme, "AppTheme");
            ThemeManager.Current.ApplicationTheme = theme switch
            {
                "Dark" => ApplicationTheme.Dark,
                "Light" => ApplicationTheme.Light,
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

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug(outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}{Exception}")
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}{Exception}")
            .WriteTo.File(_file,
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}{Exception}")
            .CreateLogger();
        Log.Debug("hello serilog");
    }

    public static bool IsMultiThreaded { get; } = false;
}