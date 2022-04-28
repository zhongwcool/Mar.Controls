using System.Windows;

namespace Sample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static bool IsMultiThreaded { get; } = false;
}