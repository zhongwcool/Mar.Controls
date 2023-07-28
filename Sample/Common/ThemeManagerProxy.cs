using System.ComponentModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using ModernWpf;
using Sample.Data;
using Sample.Helper;

namespace Sample.Common;

public class ThemeManagerProxy : ObservableObject
{
    private ThemeManagerProxy()
    {
        DispatcherHelper.RunOnMainThread(() =>
        {
            DependencyPropertyDescriptor.FromProperty(ThemeManager.ApplicationThemeProperty, typeof(ThemeManager))
                .AddValueChanged(ThemeManager.Current, delegate { UpdateApplicationTheme(); });

            DependencyPropertyDescriptor.FromProperty(ThemeManager.ActualApplicationThemeProperty, typeof(ThemeManager))
                .AddValueChanged(ThemeManager.Current, delegate { UpdateActualApplicationTheme(); });

            DependencyPropertyDescriptor.FromProperty(ThemeManager.AccentColorProperty, typeof(ThemeManager))
                .AddValueChanged(ThemeManager.Current, delegate { UpdateAccentColor(); });

            DependencyPropertyDescriptor.FromProperty(ThemeManager.ActualAccentColorProperty, typeof(ThemeManager))
                .AddValueChanged(ThemeManager.Current, delegate { UpdateActualAccentColor(); });

            UpdateApplicationTheme();
            UpdateActualApplicationTheme();
            UpdateAccentColor();
            UpdateActualAccentColor();
        });
    }

    private readonly AppConfig _config = AppConfig.CreateInstance();
    public static ThemeManagerProxy Current { get; } = new ThemeManagerProxy();

    #region ApplicationTheme

    public ApplicationTheme? ApplicationTheme
    {
        get
        {
            var theme = _config.GetValue(Section.Theme, "AppTheme");
            _applicationTheme = theme switch
            {
                "Dark" => ModernWpf.ApplicationTheme.Dark,
                "Light" => ModernWpf.ApplicationTheme.Light,
                _ => null
            };

            return _applicationTheme;
        }
        set
        {
            SetProperty(ref _applicationTheme, value);

            if (!_updatingApplicationTheme)
            {
                DispatcherHelper.RunOnMainThread(() => { ThemeManager.Current.ApplicationTheme = _applicationTheme; });
            }

            // 存入配置
            var theme = ThemeManager.Current.ApplicationTheme;
            if (null == theme)
            {
                _config.SetValue(Section.Theme, "AppTheme", "");
                return;
            }

            _config.SetValue(Section.Theme, "AppTheme", theme.ToString());
        }
    }

    private void UpdateApplicationTheme()
    {
        _updatingApplicationTheme = true;
        ApplicationTheme = ThemeManager.Current.ApplicationTheme;
        _updatingApplicationTheme = false;
    }

    private ApplicationTheme? _applicationTheme;
    private bool _updatingApplicationTheme;

    #endregion

    #region ActualApplicationTheme

    public ApplicationTheme ActualApplicationTheme
    {
        get => _actualApplicationTheme;
        private set => SetProperty(ref _actualApplicationTheme, value);
    }

    private void UpdateActualApplicationTheme()
    {
        ActualApplicationTheme = ThemeManager.Current.ActualApplicationTheme;
    }

    private ApplicationTheme _actualApplicationTheme;

    #endregion

    #region AccentColor

    private Color? _accentColor;
    private bool _updatingAccentColor;

    public Color? AccentColor
    {
        get
        {
            var accent = _config.GetValue(Section.Theme, "AccentColor");
            if (!string.IsNullOrEmpty(accent) && accent.StartsWith("#"))
            {
                _accentColor = (Color)ColorConverter.ConvertFromString(accent)!;
            }
            else
            {
                _accentColor = null;
                _config.SetValue(Section.Theme, "AccentColor", "");
            }

            return _accentColor;
        }
        set
        {
            if (_accentColor != value)
            {
                // 存入配置文件
                if (null != value)
                {
                    _config.SetValue(Section.Theme, "AccentColor", value.ToString());
                }
                else
                {
                    _config.SetValue(Section.Theme, "AccentColor", "");
                }

                SetProperty(ref _accentColor, value);

                if (!_updatingAccentColor)
                {
                    DispatcherHelper.RunOnMainThread(() => { ThemeManager.Current.AccentColor = _accentColor; });
                }
            }
        }
    }

    private void UpdateAccentColor()
    {
        _updatingAccentColor = true;
        AccentColor = ThemeManager.Current.AccentColor;
        _updatingAccentColor = false;
    }

    #endregion

    #region ActualAccentColor

    public Color ActualAccentColor
    {
        get => _actualAccentColor;
        private set => SetProperty(ref _actualAccentColor, value);
    }

    private void UpdateActualAccentColor()
    {
        ActualAccentColor = ThemeManager.Current.ActualAccentColor;
    }

    private Color _actualAccentColor;

    #endregion
}