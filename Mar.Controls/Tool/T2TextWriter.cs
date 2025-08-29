using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Mar.Controls.Tool;

internal class T2TextWriter : TextWriter
{
    private readonly TextBox _outputTextBox;
    private readonly StringBuilder _buffer = new StringBuilder();
    private readonly DispatcherTimer _updateTimer;
    private readonly object _bufferLock = new object();
    private bool _isDisposed = false;

    // 缓冲区大小阈值，超过此值就触发更新
    private const int BufferThreshold = 1024;
    
    // 最大更新频率（毫秒）
    private const int UpdateInterval = 100;

    public T2TextWriter(TextBox textBox)
    {
        _outputTextBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        
        // 创建定时器用于批量更新UI
        _updateTimer = new DispatcherTimer(DispatcherPriority.Background, _outputTextBox.Dispatcher)
        {
            Interval = TimeSpan.FromMilliseconds(UpdateInterval)
        };
        _updateTimer.Tick += UpdateTimer_Tick;
        _updateTimer.Start();

        // 设置文本框的最大长度，防止内存泄漏
        _outputTextBox.MaxLength = 50000; // 50KB限制
        
        // 监听文本框内容变化，自动滚动到底部
        _outputTextBox.TextChanged += (_, _) =>
        {
            if (_outputTextBox.Text.Length > _outputTextBox.MaxLength)
            {
                var lines = _outputTextBox.Text.Split('\n').ToList();
                if (lines.Count > 1)
                {
                    // 移除前面的行，保留后面的行
                    var keepCount = lines.Count / 2;
                    var keepLines = lines.Skip(lines.Count - keepCount).ToList();
                    _outputTextBox.Text = string.Join("\n", keepLines);
                }
            }
            
            // 滚动到底部
            _outputTextBox.ScrollToEnd();
        };
    }

    private void UpdateTimer_Tick(object? sender, EventArgs e)
    {
        FlushBuffer();
    }

    private void FlushBuffer()
    {
        if (_isDisposed) return;

        string? textToAppend = null;
        
        lock (_bufferLock)
        {
            if (_buffer.Length > 0)
            {
                textToAppend = _buffer.ToString();
                _buffer.Clear();
            }
        }

        if (string.IsNullOrEmpty(textToAppend)) return;
        try
        {
            _outputTextBox.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (!_isDisposed)
                {
                    _outputTextBox.AppendText(textToAppend);
                }
            }));
        }
        catch (Exception ex)
        {
            // 记录错误但不抛出异常
            System.Diagnostics.Debug.WriteLine($"更新TextBox时出错: {ex.Message}");
        }
    }

    public override void Write(char value)
    {
        if (_isDisposed) return;

        lock (_bufferLock)
        {
            _buffer.Append(value);
            
            // 如果缓冲区超过阈值，立即刷新
            if (_buffer.Length >= BufferThreshold)
            {
                FlushBuffer();
            }
        }
    }

    public override void Write(string? value)
    {
        if (_isDisposed || value == null) return;

        lock (_bufferLock)
        {
            _buffer.Append(value);
            
            // 如果缓冲区超过阈值，立即刷新
            if (_buffer.Length >= BufferThreshold)
            {
                FlushBuffer();
            }
        }
    }

    public override void WriteLine(string? value)
    {
        if (_isDisposed) return;

        lock (_bufferLock)
        {
            _buffer.Append(value);
            _buffer.Append(Environment.NewLine);
            
            // 换行时总是刷新缓冲区，确保及时显示
            FlushBuffer();
        }
    }

    public override void WriteLine()
    {
        if (_isDisposed) return;

        lock (_bufferLock)
        {
            _buffer.Append(Environment.NewLine);
            FlushBuffer();
        }
    }

    public override void Flush()
    {
        FlushBuffer();
        base.Flush();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_isDisposed && disposing)
        {
            _isDisposed = true;
            
            // 停止定时器
            _updateTimer?.Stop();
            
            // 最后一次刷新缓冲区
            FlushBuffer();
            
            // 清理缓冲区
            lock (_bufferLock)
            {
                _buffer.Clear();
            }
        }
        
        base.Dispose(disposing);
    }

    public override Encoding Encoding => Encoding.UTF8;
}