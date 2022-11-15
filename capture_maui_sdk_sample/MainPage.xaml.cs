using Microsoft.Maui;
using SocketMobile.Capture;
using System.Text.RegularExpressions;

namespace capture_maui_sdk_sample;

public partial class MainPage : ContentPage
{
	int count = 0;
	CaptureHelper capture = new CaptureHelper();

	public MainPage()
	{
		InitializeComponent();

        var appId = "";
        var developerId = "";
        var appKey = "";

        if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            appId = "ios:com.socketmobile.capturemauisdksample";
            developerId = "ec9e1b16-c5ae-ec11-983e-000d3a5bbc61";
            appKey = "MC0CFQCmP6oKJtSK5RXgIpPBUhXsjg63rwIUbS8tmz3f2JpdoNjfhHWGmjv5+AI=";

        }
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            appId = "android:com.socketmobile.multiplatformtest.android";
            developerId = "ec9e1b16-c5ae-ec11-983e-000d3a5bbc61";
            appKey = "MCwCFBOpqbd3TEZivPYJ2oKHKASgPD+yAhRVLLyX/+yQpa3LXAMC+MpgiwdXsA==";
        }
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            appId = "windows:com.socketmobile.multiplatformtest.windows";
            developerId = "ec9e1b16-c5ae-ec11-983e-000d3a5bbc61";
            appKey = "MCwCFAc20rtTcO+lDlodlK/+LVvx+plcAhQpDPwo3SyqMWdm4NbUBWAd5IRv/A==";
        }

        capture.OpenAsync(appId, developerId, appKey)
        .ContinueWith(result => {
            System.Diagnostics.Debug.Print("Open Capture returns {0}", result.Result);
            if (SktErrors.SKTSUCCESS(result.Result))
            {
                capture.DeviceArrival += Capture_DeviceArrival;
                capture.DeviceRemoval += Capture_DeviceRemoval; ;
                capture.DecodedData += Capture_DecodedData;
            }
        });

    }

    private void Capture_DecodedData(object sender, CaptureHelper.DecodedDataArgs e)
    {
    }

    private void Capture_DeviceRemoval(object sender, CaptureHelper.DeviceArgs e)
    {
    }

    private void Capture_DeviceArrival(object sender, CaptureHelper.DeviceArgs e)
    {
    }

    private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}

