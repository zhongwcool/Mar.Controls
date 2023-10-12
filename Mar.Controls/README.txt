# Mar.Controls

![](https://raw.githubusercontent.com/zhongwcool/Mar.Controls/main/Assets/sf9QtpmJUj.gif)

Thanks for installing the Mar.Controls NuGet package!

### CircularProgressBar

Don't forget to add a percent value resources in ViewModel:

	<Grid>
        ...
        <mar:CircularProgressBar Value="{Binding Percent}" />
        ...
    </Grid>
    
    
### ConsoleWindow   
 
Don't forget to configure Serilog in App.cs:

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
