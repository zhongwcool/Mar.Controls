﻿using System;
using System.Windows;
using System.Windows.Threading;

namespace Sample.Helper;

public static class DispatcherHelper
{
    public static void RunOnMainThread(Action action)
    {
        RunOnUIThread(Application.Current, action);
    }

    public static void RunOnUIThread(this DispatcherObject d, Action action)
    {
        var dispatcher = d.Dispatcher;
        if (dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            dispatcher.BeginInvoke(action);
        }
    }
}