using System.Management;
using System.Windows;
using System.Windows.Input;

namespace Mar.Controls.Tool;

public partial class ConsoleWindow : Window
{
    public ConsoleWindow()
    {
        InitializeComponent();
        var customWriter = new T2TextWriter(BlockConsole); // 替换为你的界面控件
        Console.SetOut(customWriter);
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        PrintSystemInfo();
    }

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

    private void UIElement_OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        ScrollViewer.ScrollToBottom();
    }
}