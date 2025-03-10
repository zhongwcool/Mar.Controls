﻿using System.ComponentModel;
using System.IO;
using System.Windows;
using Mar.Cheese;

namespace Mar.Controls.Tool;

/// <inheritdoc cref="System.Windows.Window" />
public partial class ConsoleWindow : Window
{
    private static ConsoleWindow _instance;

    // 静态方法，返回唯一的实例
    public static ConsoleWindow GetInstance(Window owner)
    {
        if (_instance != null) return _instance;
        _instance = new ConsoleWindow(owner);
        _instance.Closed += (s, e) => _instance = null;
        return _instance;
    }

    // 保存默认的控制台输出流
    private readonly TextWriter _defaultWriter = Console.Out;

    // 这个变量表示窗体2是否正在跟随窗体1
    private static bool _shouldFollow = true;

    private readonly Window _owner;

    // 添加变量跟踪粘连位置
    private bool _isLeftSide = false;

    /// <summary>
    ///     Console Window
    /// <param name="owner">subscribe owner's closed event</param>
    /// </summary>
    private ConsoleWindow(Window owner)
    {
        InitializeComponent();
        _owner = owner;
        _owner.Closed += Owner_WindowClosed;
        _owner.LocationChanged += Owner_LocationChanged;

        // 默认在右侧显示
        _isLeftSide = false;
        UpdatePosition(_owner.Left + _owner.ActualWidth, _owner.Top);

        _defaultWriter = Console.Out;
        var customWriter = new T2TextWriter(BlockConsole); // 替换为你的界面控件
        Console.SetOut(customWriter);

        BlockConsole.TextChanged += (_, _) => { ScrollViewer.ScrollToBottom(); };

        LocationChanged += Self_OnLocationChanged;
    }

    private void Self_OnLocationChanged(object sender, EventArgs e)
    {
        // 判断Window2的位置与Window1是否足够近，来确定是否"粘连"
        const double distanceThreshold = 50.0; // 为距离阈值，小于这个距离将粘连

        // 检查是否应该粘连到右侧
        if (Math.Abs(Left - (_owner.Left + _owner.ActualWidth)) < distanceThreshold &&
            Math.Abs(Top - _owner.Top) < distanceThreshold)
        {
            UpdatePosition(_owner.Left + _owner.ActualWidth, _owner.Top);
            _shouldFollow = true;
            _isLeftSide = false; // 设置为右侧粘连
        }
        // 检查是否应该粘连到左侧
        else if (Math.Abs(Left + ActualWidth - _owner.Left) < distanceThreshold &&
                 Math.Abs(Top - _owner.Top) < distanceThreshold)
        {
            UpdatePosition(_owner.Left - ActualWidth, _owner.Top);
            _shouldFollow = true;
            _isLeftSide = true; // 设置为左侧粘连
        }
        else
        {
            _shouldFollow = false;
        }
    }

    private void Owner_LocationChanged(object sender, EventArgs e)
    {
        // 只有当Window2允许跟随的时候，才更新Window2的位置
        if (!_shouldFollow) return;
        if (_isLeftSide)
        {
            // 左侧粘连
            UpdatePosition(_owner.Left - ActualWidth, _owner.Top);
        }
        else
        {
            // 右侧粘连
            UpdatePosition(_owner.Left + _owner.ActualWidth, _owner.Top);
        }
    }

    /// <summary>
    ///    update position of Console Window
    /// </summary>
    /// <param name="newLeft"></param>
    /// <param name="newTop"></param>
    private void UpdatePosition(double newLeft, double newTop)
    {
        Left = newLeft;
        Top = newTop;
    }

    /// <inheritdoc />
    public ConsoleWindow()
    {
        InitializeComponent();

        var customWriter = new T2TextWriter(BlockConsole); // 替换为你的界面控件
        Console.SetOut(customWriter);
    }

    /// <inheritdoc />
    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        // 恢复输出到系统控制台
        Console.SetOut(_defaultWriter);
    }

    private void ClearTextBlock_Click(object sender, RoutedEventArgs e)
    {
        // 清除TextBox的内容
        BlockConsole.Clear();
    }

    private void Owner_WindowClosed(object sender, EventArgs e)
    {
        Close();
    }

    /// <inheritdoc cref="Window.OnContentRendered" />
    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        if (PrintHello) SystemUtil.PrintSystemInfoAsync();
    }

    #region MyRegion

    private static readonly DependencyProperty CapacityProperty =
        DependencyProperty.Register(nameof(Capacity), typeof(int), typeof(ConsoleWindow), new PropertyMetadata(1000));

    /// <summary>
    ///     Max content size of console
    /// </summary>
    public int Capacity
    {
        get => (int)GetValue(CapacityProperty);
        set => SetValue(CapacityProperty, value);
    }

    private static readonly DependencyProperty PrintHelloProperty =
        DependencyProperty.Register(nameof(PrintHello), typeof(bool), typeof(ConsoleWindow),
            new PropertyMetadata(true));

    /// <summary>
    ///     Print system info when window loaded
    /// </summary>
    public bool PrintHello
    {
        get => (bool)GetValue(PrintHelloProperty);
        set => SetValue(PrintHelloProperty, value);
    }

    #endregion
}