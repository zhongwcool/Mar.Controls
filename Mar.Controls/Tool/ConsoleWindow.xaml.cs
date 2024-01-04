using System.ComponentModel;
using System.IO;
using System.Windows;
using Mar.Cheese;

namespace Mar.Controls.Tool;

public partial class ConsoleWindow : Window
{
    // 保存默认的控制台输出流
    private readonly TextWriter _defaultWriter = Console.Out;

    /// <summary>
    ///     Console Window
    /// <param name="owner">subscribe owner's closed event</param>
    /// </summary>
    public ConsoleWindow(Window owner)
    {
        InitializeComponent();
        owner.Closed += Owner_WindowClosed;

        _defaultWriter = Console.Out;
        var customWriter = new T2TextWriter(BlockConsole); // 替换为你的界面控件
        Console.SetOut(customWriter);

        BlockConsole.TextChanged += (_, _) =>
        {
            if (!IsMouseOver) ScrollViewer.ScrollToBottom();
        };
    }

    public ConsoleWindow()
    {
        InitializeComponent();

        var customWriter = new T2TextWriter(BlockConsole); // 替换为你的界面控件
        Console.SetOut(customWriter);
    }

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