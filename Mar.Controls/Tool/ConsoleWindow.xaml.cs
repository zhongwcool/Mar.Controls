using System.ComponentModel;
using System.IO;
using System.Management;
using System.Windows;

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
        PrintSystemInfo();
    }

    #region Running Environment

    private static void PrintSystemInfo()
    {
        Console.WriteLine($"Windows Version: {Environment.OSVersion}");
        Console.WriteLine($".NET SDK Version: {Environment.Version}");

        ManagementObjectSearcher searcher;

        // Query CPU
        searcher = new ManagementObjectSearcher("select * from Win32_Processor");
        foreach (var o in searcher.Get())
        {
            var share = (ManagementObject)o;
            Console.WriteLine($"CPU: {share["Name"]}");
        }

        // Query Graphics Card
        searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
        foreach (var o in searcher.Get())
        {
            var share = (ManagementObject)o;
            Console.WriteLine("Graphics Card: " + share["Name"]);
        }

        // Query Memory
        searcher = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
        foreach (var o in searcher.Get())
        {
            var share = (ManagementObject)o;
            var capacityBytes = (ulong)share["Capacity"];
            var mem = (double)capacityBytes / 1024 / 1024 / 1024;
            Console.WriteLine("Memory: " + mem + "GB");
        }

        Console.WriteLine();
    }

    #endregion

    #region MyRegion

    public static readonly DependencyProperty CapacityProperty =
        DependencyProperty.Register(
            nameof(Capacity),
            typeof(int),
            typeof(ConsoleWindow),
            new PropertyMetadata(1000));

    public int Capacity
    {
        get => (int)GetValue(CapacityProperty);
        set => SetValue(CapacityProperty, value);
    }

    #endregion
}