# Mar.Controls

[![latest version](https://img.shields.io/nuget/v/Mar.Controls)](https://www.nuget.org/packages/Mar.Controls) [![downloads](https://img.shields.io/nuget/dt/Mar.Controls)](https://www.nuget.org/packages/Mar.Controls)

主色跟随响应颜色选择变化依赖优秀的三方库: [ModernWpfUI](https://www.nuget.org/packages/ModernWpfUI)。

## 演示各种控件效果

### 1. 环形进度

![](https://raw.githubusercontent.com/zhongwcool/Mar.Controls/main/Assets/sf9QtpmJUj.gif)

Don't forget to add a percent value resources in ViewModel:

```xaml
    <Grid>
        ...
        <mar:CircularProgressBar Value="{Binding Percent}" />
        ...
    </Grid>
```

### 2. 调试窗口

将调试信息输出到窗口中，方便调试。

![](https://raw.githubusercontent.com/zhongwcool/Mar.Controls/main/Assets/imf5dl4qxE.gif)

Don't forget to config Serilog in App.cs:

```csharp
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Debug(outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}{Exception}")
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}{Exception}")
                .WriteTo.File(_file,
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}{Exception}")
                .CreateLogger();
```