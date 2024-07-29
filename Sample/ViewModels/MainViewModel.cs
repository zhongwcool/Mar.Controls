using System;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Sample.ViewModels;

public class MainViewModel : ObservableObject
{
    private int _mPercent;
    private readonly DispatcherTimer _mTimer = new();

    private static MainViewModel _instance;

    public static MainViewModel CreateInstance()
    {
        _instance ??= new MainViewModel();
        return _instance;
    }

    private MainViewModel()
    {
        _mTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        _mTimer.Tick += MTimerTick;

        CommandStart = new RelayCommand(() =>
        {
            Log.Debug("开始演示");
            _mPercent = 0;
            if (IsBusy)
            {
                _mTimer.Stop();
                IsBusy = false;
            }
            else
            {
                Percent = 0;
                _mTimer.Start();
                IsBusy = true;
            }
        }, () => !IsBusy);
    }

    private void MTimerTick(object sender, EventArgs e)
    {
        _mPercent += 1;
        if (_mPercent > 100)
        {
            _mPercent = 100;
            Log.Debug("演示完成");
            _mTimer.Stop();
            IsBusy = false;
            // reset dispatcher timer
            _mTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }

        Percent = _mPercent;
    }

    private int _percent = 80;

    public int Percent
    {
        get => _percent;
        set
        {
            SetProperty(ref _percent, value);
            Log.Debug("{Percent}", _percent);
            CommandStart?.NotifyCanExecuteChanged();
        }
    }

    private bool _isBusy;

    private bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public IRelayCommand CommandStart { get; }
}