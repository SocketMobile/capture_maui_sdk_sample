using Microsoft.Maui;
using SocketMobile.Capture;
using System.ComponentModel;
using System.Data;
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
            appKey = "MCwCFEkb64A52C9k5eFixm5cxROCIAvuAhQEdRLDDwWm0118ZQ2Wanm3+mTsWw==";
        }
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            appId = "android:com.socketmobile.capturemauisdksample";
            developerId = "ec9e1b16-c5ae-ec11-983e-000d3a5bbc61";
            appKey = "MC0CFAHlouPuU92TWdlHoQR6Fouwo/ldAhUA7JTa66UIqUfCS3NFFbBhd6VyMQ4=";
        }
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            appId = "windows:com.socketmobile.capturemauisdksample";
            developerId = "ec9e1b16-c5ae-ec11-983e-000d3a5bbc61";
            appKey = "MCwCFHujtHDn4yJI2YV0qeOjDhQ1D7cdAhR9JoCEtiO56BX8DeL6tcZwQW2j0w==";
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
        MainThread.BeginInvokeOnMainThread(() =>
        {
            DataLabel.Text += "\n" + $"[{DateTime.Now}] " + "Decoded Data: " + System.Text.Encoding.UTF8.GetString(e.DecodedData.Data);
        });
    }

    private void Capture_DeviceRemoval(object sender, CaptureHelper.DeviceArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            DataLabel.Text += "\n" + $"[{DateTime.Now}]" + " Device Removal" + "\n";
        });
    }

    private async void Capture_DeviceArrival(object sender, CaptureHelper.DeviceArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            DataLabel.Text += $"[{DateTime.Now}] " + "Device Arrival";
            var name = await e.CaptureDevice.GetFriendlyNameAsync();
            DataLabel.Text += "\n" + $"[{DateTime.Now}] " + "Friendly Name: " + name.FriendlyName;
        });
    }
}

