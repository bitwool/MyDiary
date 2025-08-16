using Avalonia;
using System;
using System.IO;
using Serilog;

namespace MyDiary.UI;

sealed class Program
{

    [STAThread]
    public static void Main(string[] args)
    {
        var logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "MyDiary",
            "log.txt"
        );

        // 配置 Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug() // 记录所有级别的日志
            .WriteTo.Console()    // 同时输出到控制台（开发时有用）
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day) // 每天生成一个新的日志文件
            .CreateLogger();

        try
        {
            Log.Information("==============================================");
            Log.Information("Application Starting Up...");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            // 这是捕获启动过程中致命错误的关键！
            Log.Fatal(ex, "Application failed to start.");
        }
        finally
        {
            Log.Information("Application Shutting Down.");
            Log.CloseAndFlush(); // 确保所有日志都被写入文件
        }
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}