# Mar.Controls

Hope Mar.Controls can help you :-)

### 1. CircularProgressBar

![](https://raw.githubusercontent.com/zhongwcool/Mar.Controls/main/Assets/sf9QtpmJUj.gif)

Don't forget to add a percent value resources in ViewModel:

	<Grid>
        ...
        <mar:CircularProgressBar Value="{Binding Percent}" />
        ...
    </Grid>

### 2. 水印

在App.xaml中引用资源字典：

<Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                
                <!-- Other merged dictionaries here -->
                <ResourceDictionary Source="pack://application:,,,/Mar.Controls;component/Themes/Generic.xaml" />
            
            </ResourceDictionary.MergedDictionaries>
            <!-- Other app resources here -->

        </ResourceDictionary>
    </Application.Resources>

在布局文件中使用：

    <Grid>
        ...
        <mar:Watermark Mark="Mar.Controls" FontSize="30" Angle="45" MarkMargin="10" MarkBrush="{DynamicResource SystemAccentColorLight3Brush}">
            <Border BorderThickness="1" CornerRadius="4" />
        </mar:Watermark>
        ...
    </Grid>    
    
### 3. ConsoleWindow

![](https://raw.githubusercontent.com/zhongwcool/Mar.Controls/main/Assets/imf5dl4qxE.gif)

You should reference the following packages in your project:

    <ItemGroup>
        ...
        <PackageReference Include="Serilog" Version="3.0.1"/>
        <PackageReference Include="Serilog.Sinks.Console" Version="4.1.0"/>
        <PackageReference Include="Serilog.Sinks.Debug" Version="2.0.0"/>
        <PackageReference Include="Serilog.Sinks.File" Version="5.0.0"/>
    </ItemGroup>
 
Don't forget to configure Serilog in App.cs:

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

See https://github.com/zhongwcool/Mar.Controls for more information.
