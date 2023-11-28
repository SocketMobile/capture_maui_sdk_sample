namespace capture_maui_sdk_sample;

public partial class App : Application
{
#if __ANDROID__
    MainPage rootPage;
#endif

    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
#if __ANDROID__
        // (Android only) Instance of MainPage to access ReEnableCom() method
        rootPage = new MainPage(true);
#endif
    }

    protected override void OnResume()
    {
#if __ANDROID__
        // (Android only) Re-enable communication with the Service after comming back from deep sleep mode
        rootPage.ReEnableConnection();
#endif
    }
}
