using System.Windows;
using System.Windows.Media;

namespace Mar.Controls.Control;

/// <summary>
///     Display a circular progress bar with a percentage
/// </summary>
public partial class CircularProgressBar
{
    /// <summary>
    ///     Display a circular progress bar with a percentage
    /// </summary>
    public CircularProgressBar()
    {
        InitializeComponent();
        DrawBackgroundBar();
    }

    #region MagicStroke

    /// <summary>
    /// set accent color of CircularProgressBar,it doesn't follow system if MagicStroke has been set 
    /// </summary>
    public Brush Stroke
    {
        get => (Brush)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    private static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
        nameof(Stroke),
        typeof(Brush),
        typeof(CircularProgressBar),
        new PropertyMetadata(new SolidColorBrush(Colors.Black)));

    /// <summary>
    /// set Opacity of CircularProgressBar to reduce the brightness
    /// </summary>
    public float Magic
    {
        get => (float)GetValue(MagicProperty);
        set
        {
            switch (value)
            {
                case < 0.4f:
                    SetValue(MagicProperty, 0.3f);
                    break;
                case > 1.0f:
                    SetValue(MagicProperty, 1.0f);
                    break;
                default:
                    SetValue(MagicProperty, value);
                    break;
            }
        }
    }

    private static readonly DependencyProperty MagicProperty = DependencyProperty.Register(
        nameof(Magic),
        typeof(float),
        typeof(CircularProgressBar),
        new PropertyMetadata(1.0f, OnMagicChanged));

    private static void OnMagicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        //输出变化信息
    }

    #endregion

    #region CurrentValue

    /// <summary>
    /// set value of CircularProgressBar
    /// </summary>
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set
        {
            SetValue(ValueProperty, value);
            DrawValue(value);
        }
    }

    private static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(int),
        typeof(CircularProgressBar),
        new PropertyMetadata(-1, OnPropertyChanged));

    private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (args.Property == ValueProperty)
        {
            var bar = sender as CircularProgressBar;
            if (bar?.Value != null)
            {
                bar.DrawValue(bar.Value);
            }
        }
    }

    /// <summary>
    /// 设置百分百，输入整数，自动除100
    /// </summary>
    /// <param name="percentValue"></param>
    private void DrawValue(double percentValue)
    {
        // 限制百分比范围
        percentValue = Math.Max(0, Math.Min(100, percentValue));
        
        // 更新文本显示
        PART_Text.Content = percentValue.ToString("0") + "%";

        // 计算角度（从-90度开始，顺时针方向）
        var angle = (percentValue / 100.0) * 360.0;
        
        // 创建简化的圆弧路径
        var pathData = CreateArcPath(angle);
        PART_Bar.Data = Geometry.Parse(pathData);
    }

    /// <summary>
    /// 创建圆弧路径数据
    /// </summary>
    /// <param name="angle">角度（0-360）</param>
    /// <returns>Path数据字符串</returns>
    private string CreateArcPath(double angle)
    {
        if (angle <= 0)
        {
            return "M17,3 L17,3"; // 空路径
        }
        
        if (angle >= 360)
        {
            // 完整圆 - 使用两个半圆组成
            return "M17,3 A14,14 0 1 1 17,31 A14,14 0 1 1 17,3 Z";
        }

        // 计算结束点
        var endAngle = angle - 90; // 从-90度开始
        var endAngleRad = endAngle * Math.PI / 180.0;
        var endX = 17 + 14 * Math.Cos(endAngleRad);
        var endY = 17 + 14 * Math.Sin(endAngleRad);

        // 判断是否为大弧（大于180度）
        var isLargeArc = angle > 180;

        return $"M17,3 A14,14 0 {(isLargeArc ? "1" : "0")} 1 {endX:F2},{endY:F2}";
    }

    private void DrawBackgroundBar()
    {
        // 背景圆环 - 使用完整的圆
        PART_Back.Data = Geometry.Parse("M17,3 A14,14 0 1 1 17,31 A14,14 0 1 1 17,3 Z");
    }

    #endregion
}