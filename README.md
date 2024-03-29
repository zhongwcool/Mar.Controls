# [Mar.Controls](https://github.com/zhongwcool/Mar.Controls)

[![latest version](https://img.shields.io/nuget/v/Mar.Controls)](https://www.nuget.org/packages/Mar.Controls) [![downloads](https://img.shields.io/nuget/dt/Mar.Controls)](https://www.nuget.org/packages/Mar.Controls)

## 演示各种控件效果

### 水印

![](https://raw.githubusercontent.com/zhongwcool/Mar.Controls/main/Assets/s487h6X0fA.gif)

在App.xaml中引用资源字典：

```xaml
<Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                
                <!-- Other merged dictionaries here -->
                <ResourceDictionary Source="pack://application:,,,/Mar.Controls;component/Themes/Generic.xaml" />
            
            </ResourceDictionary.MergedDictionaries>
            <!-- Other app resources here -->

        </ResourceDictionary>
    </Application.Resources>
```

在布局文件中使用：

```xaml
    <Grid>
        ...
        <mar:Watermark Mark="Mar.Controls" FontSize="30" Angle="45" MarkMargin="10" MarkBrush="{DynamicResource SystemAccentColorLight3Brush}">
            <Border BorderThickness="1" CornerRadius="4" />
        </mar:Watermark>
        ...
    </Grid>
```

### 环形进度

![](https://raw.githubusercontent.com/zhongwcool/Mar.Controls/main/Assets/sf9QtpmJUj.gif)

Don't forget to add a percent value resources in ViewModel:

```xaml
    <Grid>
        ...
        <mar:CircularProgressBar Value="{Binding Percent}" />
        ...
    </Grid>
```

### 调试窗口

将调试信息输出到窗口中，方便调试。

![](https://raw.githubusercontent.com/zhongwcool/Mar.Controls/main/Assets/imf5dl4qxE.gif)

You should reference the following packages in your project:

```xaml
    <ItemGroup>
        ...
        <PackageReference Include="Serilog" Version="3.0.1"/>
        <PackageReference Include="Serilog.Sinks.Console" Version="4.1.0"/>
        <PackageReference Include="Serilog.Sinks.Debug" Version="2.0.0"/>
        <PackageReference Include="Serilog.Sinks.File" Version="5.0.0"/>
    </ItemGroup>
```

Don't forget to config Serilog in App.cs:

```csharp
private readonly string _file = Path.Combine("Log", "log.txt");

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