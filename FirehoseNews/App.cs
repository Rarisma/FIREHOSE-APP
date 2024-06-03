using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseNews.Preferences;
using FirehoseNews.UI;
using FirehoseNews.Viewmodels;

namespace FirehoseNews;

public class App : Application
{
    protected Window? MainWindow { get; private set; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
        MainWindow = new Window();
#else
        MainWindow = Microsoft.UI.Xaml.Window.Current;
#endif

#if DEBUG
        MainWindow.EnableHotReload();
#endif
#if !WINDOWS

#endif
        PreferencesModel.Load();

        configureIOC();
        App.Current.UnhandledException += Current_UnhandledException;

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not Frame rootFrame)
        {
            // Place the frame in the current Window
            MainWindow.Content = Glob.Frame;
            Glob.Frame.NavigationFailed += OnNavigationFailed;
        }

        Glob.Publications = await new API().GetPublications();

        if (Glob.Frame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            Glob.NaviStack.Push(new ArticleList());
            Glob.DoNavi();

        }

        
        // Ensure the current window is active
        MainWindow.Activate();

    }
    
    void configureIOC()
    {
        var Services = new ServiceCollection()
            .AddSingleton<ShellVM>()
            .AddSingleton<ThemeVM>()
            .BuildServiceProvider();
        
        Ioc.Default.ConfigureServices(Services);
    }

    private void Current_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Console.WriteLine(e.Message);
    }

    /// <summary>
    /// Invoked when Navigation to a page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }
}
