using Android.App;
using Android.Content.PM;
using Android.OS;
using SocketMobile.Capture;

namespace capture_maui_sdk_sample;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        Platform.Init(this, savedInstanceState);

        // Set to use Android Service Methods
        CaptureHelper.SetAndroidServiceContext(this);

        base.OnCreate(savedInstanceState);
    }
}
