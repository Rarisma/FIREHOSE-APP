using System.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using FirehoseApp.UI;
using FirehoseApp.Viewmodels;
using NLog;
using Uno.Resizetizer;
using LogLevel = NLog.LogLevel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;

namespace FirehoseApp;
public partial class App : Application
{
    Logger logger = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
        logger.Log(LogLevel.Info, "FHN Initalised");
    }

    public static Window MainWindow { get; private set; }
    public static Frame UI { get; private set; }
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
#if __ANDROID__
    logger.Log(LogLevel.Info, "Running on android.");
    Uno.UI.FeatureConfiguration.Style.ConfigureNativeFrameNavigation();
    Uno.UI.FeatureConfiguration.NativeFramePresenter.AndroidUnloadInactivePages = false;
#endif
        MainWindow = new();
        UI = new();
#if DEBUG
        logger.Log(LogLevel.Info, "Running as Debug.");
        MainWindow.EnableHotReload();
#endif
        configureIOC();

        Current.UnhandledException += Current_UnhandledException;

        if (MainWindow.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            MainWindow.Content = UI;
            UI.Navigate(typeof(ArticleList));
        }
        
        //Set window info and show it.
        MainWindow.SetWindowIcon();

#if __ANDROID__ || __IOS__ || __MACCATALYST__ || __MACOS__
                MainWindow.Title = $"Firehose News {Package.Current.Id.Version.Major}." +
               $"{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";
#else
        MainWindow.Title = "Firehose News";
#endif


        if (Debugger.IsAttached)
        {
            MainWindow.Title += " (DEV)";
        }
        MainWindow.Activate();
    }
    
    void configureIOC()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();


        var Services = new ServiceCollection()
            .AddSingleton<ShellVM>()
            .AddSingleton<ThemeVM>()
            .AddSingleton(PreferencesModel.Load())
            .AddLogging(loggingBuilder =>
            {
                // configure Logging with NLog
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                loggingBuilder.AddNLog(config);
            })
            .BuildServiceProvider();
        
        Ioc.Default.ConfigureServices(Services);
        logger.Log(LogLevel.Info, "IOC initalised");
    }
    
    private void Current_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        logger.Fatal(e.Exception, $"Unhandled crash: {e.Message}");
    }
}
