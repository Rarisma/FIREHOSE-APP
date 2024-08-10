using Windows.ApplicationModel.DataTransfer;
using CommunityToolkit.Mvvm.DependencyInjection;
using FirehoseApp.Preferences;
using FirehoseApp.UI.Dialogs;
using FirehoseApp.Viewmodels;
using HYDRANT.Definitions;
using WinRT.Interop;

namespace FirehoseApp.UI.Controls;
public sealed partial class CommonNavigationBar : Grid
{
    PreferencesModel Pref = Ioc.Default.GetRequiredService<PreferencesModel>();

    /// <summary>
    /// Set this to the Article Object you want to display publisher information for
    /// </summary>
    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register(
            "ItemSource", // Name of the property
            typeof(Article), // Type of the property
            typeof(CommonNavigationBar), // Owner of the property (this control)
            new PropertyMetadata(default(string)));
    
    /// <summary>
    /// Publication information for given article.
    /// </summary>
    public Article ItemSource
    {
        get { return (Article)GetValue(ItemSourceProperty); }
        set { SetValue(ItemSourceProperty, value); }
    }

    public CommonNavigationBar()
    {
        InitializeComponent();
    }
    
    private void ReportSummary(object sender, RoutedEventArgs e) =>
        AIFeedbackDialog.ReportSummary(ItemSource);
    
    /// <summary>
    /// Opens article in the users browser
    /// </summary>
    private async void OpenBrowser(object sender, RoutedEventArgs e) => await Windows.System.Launcher.LaunchUriAsync(new Uri(ItemSource.Url));
    
    
    /// <summary>
    /// Opens the reader mode UI, which should show the users 
    /// a no nonsense view of the articles text.
    /// </summary>
    private async void OpenReader(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(ItemSource.Text))
        {
            ItemSource.Text = await Ioc.Default.GetRequiredService<ShellVM>()
                .Hallon.GetArticleText(ItemSource.Url);
        }
        Glob.DoNavi(new ReaderMode(ItemSource));
    } 
    
    /// <summary>
    /// Return to article view.
    /// </summary>
    private void GoBack(object sender, RoutedEventArgs e) { Glob.GoBack(); }
    
    /// <summary>
    /// Ran when loaded, checks the users bookmarks for if the Article is bookmarked.
    /// </summary>
    public void SetBookmarkIcon(object? sender = null, RoutedEventArgs? e = null)
    {
        //Handle bookmarked status
        if (Pref.BookmarkedArticles.Count(article => article.Url == ItemSource.Url) != 0)
        {
            Glyphy.Glyph = "\xE735"; // Filled Star
        }
        else { Glyphy.Glyph = "\xE734"; } // Empty star
    }
    
    private void BookmarkClick(object sender, RoutedEventArgs e)
    {
        PreferencesModel M = Ioc.Default.GetRequiredService<PreferencesModel>();
        // Add or Remove article from bookmarked articles.
        if (M.BookmarkedArticles.Contains(ItemSource))
        {
            M.BookmarkedArticles.Remove(ItemSource);
        }
        else
        {
            M.BookmarkedArticles.Add(ItemSource);
        }
        
        // update icon state and save bookmarks
        SetBookmarkIcon();
        PreferencesModel.Save();
    }
    
    private void ReportClickbait(object sender, RoutedEventArgs e)
    {
        Ioc.Default.GetRequiredService<ShellVM>().Hallon.VoteClickbait(ItemSource);

        //Show visual feedback
        ClickbaitButton.Text = "Already reported";
        ClickbaitButton.IsEnabled = false; //Prevent multiple reports.
    }
    
    private async void Share(object sender, RoutedEventArgs e)
    {
        if (DataTransferManager.IsSupported())
        {
#if WINDOWS //Sharing workarounds
            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WindowNative.GetWindowHandle(App.MainWindow);

            //Do Interop magic
            IDataTransferManagerInterop interop = DataTransferManager.As<IDataTransferManagerInterop>();
            IntPtr result = interop.GetForWindow(hWnd, _dtm_iid);

            //Create DataManager and show UI
            var dataTransferManager = WinRT.MarshalInterface<DataTransferManager>.FromAbi(result);
            dataTransferManager.DataRequested += ShareStarted;
            interop.ShowShareUIForWindow(hWnd);
#else //Non-windows platforms.
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += ShareStarted;
            DataTransferManager.ShowShareUI();
#endif
        }
        else
        {
            //This shouldn't happen.
            await Glob.OpenContentDialog(new()
            {
               Title = "Your system doesn't support sharing.",
               Content = "Sorry, but your system somehow doesn't support sharing." +
                         "\nThis shouldn't happen but if you see this, let us know",
               PrimaryButtonText = "Close"
            });
        }
    }
    
    /// <summary>
    /// This event is called when the device sharing menu is opened
    /// </summary>
    private void ShareStarted(DataTransferManager manager, DataRequestedEventArgs args)
    {
        args.Request.Data.Properties.Title = $"Sharing {ItemSource.Title}";
        args.Request.Data.Properties.Description = "Summary Text";
        args.Request.Data.SetText(ItemSource.Summary);
        args.Request.Data.SetWebLink(new Uri(ItemSource.Url));
    }
    
    //Required windows workarounds.
#if WINDOWS
    [System.Runtime.InteropServices.ComImport]
    [System.Runtime.InteropServices.Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8")]
    [System.Runtime.InteropServices.InterfaceType(
        System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
    interface IDataTransferManagerInterop
    {
        IntPtr GetForWindow([System.Runtime.InteropServices.In] IntPtr appWindow,
            [System.Runtime.InteropServices.In] ref Guid riid);
        void ShowShareUIForWindow(IntPtr appWindow);
    }
    
    static readonly Guid _dtm_iid =
        new Guid(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c);

#endif

    /// <summary>
    /// Opens about this source dialog
    /// </summary>
    private async void OpenSourceInfo(object sender, RoutedEventArgs e)
    {
        await Glob.OpenContentDialog(new ContentDialog
        {
            Title = "About " + Glob.Publications[ItemSource.PublisherID].Name,
            PrimaryButtonText = "Close",
            Content = new AboutSource(ItemSource.PublisherID)
        });
    }
}
