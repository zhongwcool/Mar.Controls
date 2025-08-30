using System.ComponentModel;
using System.IO;
using System.Windows;
using Mar.Cheese;

namespace Mar.Controls.Tool;

/// <inheritdoc cref="System.Windows.Window" />
public partial class ConsoleWindow : Window
{
    // 使用字典来管理多个实例，避免内存泄漏
    private static readonly Dictionary<Window, ConsoleWindow> Instances = new();
    private static readonly object LockObject = new object();

    // 静态方法，返回唯一的实例
    public static ConsoleWindow GetInstance(Window owner, bool showOnLeft = false, double spacing = 0.0)
    {
        if (owner == null)
            throw new ArgumentNullException(nameof(owner));

        lock (LockObject)
        {
            if (Instances.TryGetValue(owner, out var existingInstance))
            {
                // 检查实例是否仍然有效
                if (!existingInstance.IsDisposed)
                {
                    return existingInstance;
                }
                else
                {
                    // 清理无效实例
                    Instances.Remove(owner);
                }
            }

            // 创建新实例
            var newInstance = new ConsoleWindow(owner, showOnLeft, spacing);
            Instances[owner] = newInstance;
            return newInstance;
        }
    }

    // 保存默认的控制台输出流
    private readonly TextWriter _defaultWriter = Console.Out;

    // 这个变量表示窗体2是否正在跟随窗体1
    private static bool _shouldFollow = true;

    private readonly Window _owner = null!;

    // 添加变量跟踪粘连位置
    private bool _isLeftSide = false;
    
    // 添加间距参数
    private double _spacing = 0.0;

    // 添加标志位来跟踪是否已释放

    /// <summary>
    /// 检查实例是否已释放
    /// </summary>
    private bool IsDisposed { get; set; } = false;

    /// <summary>
    ///     Console Window
    /// <param name="owner">subscribe owner's closed event</param>
    /// <param name="showOnLeft">是否在左侧显示</param>
    /// <param name="spacing">窗口粘连的间距</param>
    /// </summary>
    private ConsoleWindow(Window owner, bool showOnLeft = false, double spacing = 0.0)
    {
        InitializeComponent();
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _spacing = spacing;
        
        // 订阅owner的事件
        _owner.Closed += Owner_WindowClosed;
        _owner.LocationChanged += Owner_LocationChanged;

        // 设置显示位置
        _isLeftSide = showOnLeft;
        UpdateInitialPosition();

        _defaultWriter = Console.Out;
        var customWriter = new T2TextWriter(BlockConsole); // 替换为你的界面控件
        Console.SetOut(customWriter);

        BlockConsole.TextChanged += (_, _) => { ScrollViewer.ScrollToBottom(); };

        LocationChanged += Self_OnLocationChanged;
        
        // 订阅自己的关闭事件
        Closed += Self_Closed;
    }

    /// <summary>根据左右位置和间距更新初始位置</summary>
    private void UpdateInitialPosition()
    {
        var verticalBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
        if (_isLeftSide)
            UpdatePosition(_owner.Left - ActualWidth - _spacing - (_spacing == 0.0 ? verticalBorderWidth : 0.0), _owner.Top);
        else
            UpdatePosition(_owner.Left + _owner.ActualWidth + _spacing - (_spacing == 0.0 ? verticalBorderWidth : 0.0), _owner.Top);
    }

    private void Self_Closed(object? sender, EventArgs e)
    {
        CleanupInstance();
    }

    private void CleanupInstance()
    {
        if (IsDisposed) return;
        
        IsDisposed = true;
        
        // 取消事件订阅
        _owner.Closed -= Owner_WindowClosed;
        _owner.LocationChanged -= Owner_LocationChanged;

        // 从静态字典中移除
        lock (LockObject)
        {
            Instances.Remove(_owner);
        }
        
        // 恢复控制台输出
        try
        {
            Console.SetOut(_defaultWriter);
        }
        catch (Exception ex)
        {
            // 记录错误但不抛出异常
            System.Diagnostics.Debug.WriteLine($"恢复控制台输出时出错: {ex.Message}");
        }
    }

    /// <summary>
    /// 清理所有实例（用于测试或特殊情况）
    /// </summary>
    public static void CleanupAllInstances()
    {
        lock (LockObject)
        {
            var instancesToClose = Instances.Values.ToList();
            foreach (var instance in instancesToClose)
            {
                try
                {
                    if (!instance.IsDisposed)
                    {
                        instance.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"关闭ConsoleWindow实例时出错: {ex.Message}");
                }
            }
            Instances.Clear();
        }
    }

    private void Self_OnLocationChanged(object? sender, EventArgs e)
    {
        var verticalBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
        var num = _spacing - (_spacing == 0.0 ? verticalBorderWidth : 0.0);
        
        // 检查是否应该粘连到右侧
        if (Math.Abs(Left - (_owner.Left + _owner.ActualWidth + num)) < 50.0 &&
            Math.Abs(Top - _owner.Top) < 50.0)
        {
            UpdatePosition(_owner.Left + _owner.ActualWidth + num, _owner.Top);
            _shouldFollow = true;
            _isLeftSide = false; // 设置为右侧粘连
        }
        // 检查是否应该粘连到左侧
        else if (Math.Abs(Left + ActualWidth + num - _owner.Left) < 50.0 &&
                 Math.Abs(Top - _owner.Top) < 50.0)
        {
            UpdatePosition(_owner.Left - ActualWidth - num, _owner.Top);
            _shouldFollow = true;
            _isLeftSide = true; // 设置为左侧粘连
        }
        else
        {
            _shouldFollow = false;
        }
    }

    private void Owner_LocationChanged(object? sender, EventArgs? e)
    {
        // 只有当Window2允许跟随的时候，才更新Window2的位置
        if (!_shouldFollow) return;
        
        var verticalBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
        var num = _spacing - (_spacing == 0.0 ? verticalBorderWidth : 0.0);
        
        if (_isLeftSide)
        {
            // 左侧粘连
            UpdatePosition(_owner.Left - ActualWidth - num, _owner.Top);
        }
        else
        {
            // 右侧粘连
            UpdatePosition(_owner.Left + _owner.ActualWidth + num, _owner.Top);
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
        CleanupInstance();
    }

    private void ClearTextBlock_Click(object sender, RoutedEventArgs e)
    {
        // 清除TextBox的内容
        BlockConsole.Clear();
    }

    private void Owner_WindowClosed(object? sender, EventArgs? e)
    {
        CleanupInstance();
        Close();
    }

    /// <inheritdoc cref="Window.OnContentRendered" />
    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        UpdateInitialPosition();
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