using System.ComponentModel;
using System.IO;
using System.Windows;
using Mar.Cheese;

namespace Mar.Controls.Tool;

public partial class ConsoleWindow : Window
{
    // 保存默认的控制台输出流
    private readonly TextWriter _defaultWriter = Console.Out;

    // 这个变量表示窗体2是否正在跟随窗体1
    private static bool _shouldFollow = true;
    private readonly Window _owner;

    /// <summary>
    ///     Console Window
    /// <param name="owner">subscribe owner's closed event</param>
    /// </summary>
    public ConsoleWindow(Window owner)
    {
        InitializeComponent();
        _owner = owner;
        _owner.Closed += Owner_WindowClosed;
        _owner.LocationChanged += Owner_LocationChanged;

        _defaultWriter = Console.Out;
        var customWriter = new T2TextWriter(BlockConsole); // 替换为你的界面控件
        Console.SetOut(customWriter);

        BlockConsole.TextChanged += (_, _) =>
        {
            if (!IsMouseOver) ScrollViewer.ScrollToBottom();
        };

        LocationChanged += Self_OnLocationChanged;
    }

    private void Self_OnLocationChanged(object sender, EventArgs e)
    {
        // 判断Window2的位置与Window1是否足够近，来确定是否“粘连”
        const double distanceThreshold = 50.0; // 为距离阈值，小于这个距离将粘连

        // 如果拖动Window2并靠近Window1，则粘连到Window1的旁边
        if (Math.Abs(Left - (_owner.Left + _owner.ActualWidth)) < distanceThreshold &&
            Math.Abs(Top - _owner.Top) < distanceThreshold)
        {
            UpdatePosition(_owner.Left + _owner.ActualWidth, _owner.Top);
            _shouldFollow = true; // 重置为跟随状态
        }
        else
        {
            _shouldFollow = false;
        }
    }

    private void Owner_LocationChanged(object sender, EventArgs e)
    {
        // 只有当Window2允许跟随的时候，才更新Window2的位置
        if (_shouldFollow)
        {
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

    private void Owner_WindowClosed(object sender, EventArgs e)
    {
        Close();
    }

    /// <inheritdoc cref="Window.OnContentRendered" />
    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        if (PrintHello) SystemUtil.PrintSystemInfo();
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