using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Services;
using FirehoseApp.Views.Models;
using Serilog;
using Serilog.Core;
using Uno.Resizetizer;

namespace FirehoseApp;
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)

                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(LogLevel.Warning);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layout specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// DevServer and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

                }, enableUnoLogging: true)
                .UseSerilog(consoleLoggingEnabled: true, fileLoggingEnabled: true)
                //.ConfigureServices((context, services) =>
                //{
                //    services.AddSingleton<AppState>();
                //    services.AddSingleton<ShellVM>();
                //})
            );

        UnhandledException += App_UnhandledException;
        Log.Information($"Firehose News {AppState.Version} started");
        configureIOC();
        Log.Information("IOC built");
        MainWindow = builder.Window;

#if DEBUG
        Log.Information("Hot reload enabled.");
        MainWindow.EnableHotReload();
#endif
        MainWindow.SetWindowIcon();
        Log.Information("Icon set");

        Host = builder.Build();

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;
            Log.Information("Window initalised.");
        }

        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(MainPage), args.Arguments);
            Log.Information("window frame has no content, navigating to mainpage.");
        }

        // Ensure the current window is active
        Log.Information("Displaying window.");
        MainWindow.Activate();
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Log.Logger.Error(e.Exception.Message);
    }

    void configureIOC()
    {
        var Services = new ServiceCollection()
            .AddSingleton<AppState>()
            .AddSingleton<ShellVM>()
            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(Services);
    }
}
