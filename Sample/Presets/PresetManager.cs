using System;
using Sample.Common;
using Sample.Data;

namespace Sample.Presets;

public class PresetManager : BindableBase
{
    private readonly AppConfig _config = AppConfig.CreateInstance();

    internal const string DefaultPreset = "Default";

    private PresetManager()
    {
    }

    public static PresetManager Current { get; } = new PresetManager();

    private string _colorPreset = DefaultPreset;

    public string ColorPreset
    {
        get
        {
            var color = _config.GetValue(Section.Theme, "ColorPreset");
            if (color != "Forest" & color != "Lavender" & color != "Nighttime" & color != DefaultPreset)
            {
                color = DefaultPreset;
                _config.SetValue(Section.Theme, "ColorPreset", DefaultPreset);
            }

            if (!string.IsNullOrEmpty(color)) _colorPreset = color;
            return _colorPreset;
        }
        set
        {
            if (_colorPreset != value)
            {
                _colorPreset = value;
                // 存入配置文件
                _config.SetValue(Section.Theme, "ColorPreset", _colorPreset);
                RaisePropertyChanged();
                ColorPresetChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private string _shapePreset = DefaultPreset;

    public string ShapePreset
    {
        get
        {
            var shape = _config.GetValue(Section.Theme, "ShapePreset");
            if (shape != "PreFluent" & shape != DefaultPreset)
            {
                shape = DefaultPreset;
                _config.SetValue(Section.Theme, "ShapePreset", DefaultPreset);
            }

            if (!string.IsNullOrEmpty(shape)) _shapePreset = shape;
            return _shapePreset;
        }
        set
        {
            if (_shapePreset != value)
            {
                _shapePreset = value;
                // 存入配置文件
                _config.SetValue(Section.Theme, "ShapePreset", _shapePreset);
                RaisePropertyChanged();
                ShapePresetChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler ColorPresetChanged;
    public event EventHandler ShapePresetChanged;
}