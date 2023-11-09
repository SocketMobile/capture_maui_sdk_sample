using Microsoft.Maui;
using SocketMobile.Capture;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using capture_maui_sdk_sample.Model;

namespace capture_maui_sdk_sample;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    public CaptureHelper capture = new CaptureHelper();

    private static ObservableCollection<StoredDevice> _deviceList = new ObservableCollection<StoredDevice>();

    private static CaptureHelperDevice _selectedDevice;

    private string _selectedDeviceText = $"Selected Device for Trigger Scan Button: ";
    public string SelectedDeviceText
    {
        get => _selectedDeviceText;
        set
        {
            if (_selectedDeviceText != value)
            {
                _selectedDeviceText = value;
                OnPropertyChanged(nameof(SelectedDeviceText));
            }
        }
    }

    private string _displayText = $"Decoded Data: ";
    public string DisplayText
    {
        get => _displayText;
        set
        {
            if (_displayText != value)
            {
                _displayText = value;
                OnPropertyChanged(nameof(DisplayText));
            }
        }
    }

    private string _deviceEventText = $"Device Event: ";
    public string DeviceEventText
    {
        get => _deviceEventText;
        set
        {
            if (_deviceEventText != value)
            {
                _deviceEventText = value;
                OnPropertyChanged(nameof(DeviceEventText));
            }
        }
    }

    private bool _isVisibleAndroid;
    public bool IsVisibleAndroid
    {
        get => _isVisibleAndroid;
        set
        {
            _isVisibleAndroid = value;
            OnPropertyChanged(nameof(IsVisibleAndroid));
        }
    }

    private bool _isSocketCamEnable;
    public bool IsSocketCamEnable
    {
        get => _isSocketCamEnable;
        set
        {
            _isSocketCamEnable = value;
            OnPropertyChanged(nameof(IsSocketCamEnable));
        }
    }

    private bool _isSocketCamSwitchEnable;
    public bool IsSocketCamSwitchEnable
    {
        get => _isSocketCamSwitchEnable;
        set
        {
            _isSocketCamSwitchEnable = value;
            OnPropertyChanged(nameof(IsSocketCamSwitchEnable));
        }
    }

    public MainPage()
    {
        InitializeComponent();

        string appId = "";
        string developerId = "";
        string appKey = "";

        switchLabel.BindingContext = this;
        selectedDeviceText.BindingContext = this;
        deviceList.BindingContext = this;
        deviceList.ItemsSource = _deviceList;
        deviceArrivalRemoval.BindingContext = this;
        deviceDecodedData.BindingContext = this;
        socketCamSwitch.BindingContext = this;

        switch (Device.RuntimePlatform)
        {
            case Device.iOS:
                appId = "<User App ID>";
                developerId = "<User Developer ID>";
                appKey = "<User App Key>";
                break;

            case Device.Android:
                appId = "<User App ID>";
                developerId = "<User Developer ID>";
                appKey = "<User App Key>";
                break;

            case Device.UWP:
                appId = "<User App ID>";
                developerId = "<User Developer ID>";
                appKey = "<User App Key>";
                break;

        }

        IsVisibleAndroid = Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.iOS;

        capture.OpenAsync(appId, developerId, appKey)
        .ContinueWith(result => {
            System.Diagnostics.Debug.Print("Open Capture returns {0}", result.Result);
            if (SktErrors.SKTSUCCESS(result.Result))
            {
                capture.DeviceArrival += Capture_DeviceArrival;
                capture.DeviceRemoval += Capture_DeviceRemoval; ;
                capture.DecodedData += Capture_DecodedData;

                // (Android-iOS) Check if SocketCam is enabled to set the Switch
                getSocketCamStatusInit();
            }
        });
    }

    // Device Events--
    private void Capture_DeviceRemoval(object sender, CaptureHelper.DeviceArgs e)
    {
        DeviceEventText = string.Format("Device Removal: {0}", e.CaptureDevice.GetDeviceInfo().Name);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                int i = 0;
                foreach (var device in _deviceList)
                {
                    if (device.DeviceName == e.CaptureDevice.GetDeviceInfo().Name)
                    {
                        _deviceList.RemoveAt(i);
                        break;
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error removing device from list: " + ex.ToString());
            }

        });

        SelectedDeviceText = "Selected Device: ";
    }

    private void Capture_DeviceArrival(object sender, CaptureHelper.DeviceArgs e)
    {
        DeviceEventText = string.Format("Device Arrival: {0}", e.CaptureDevice.GetDeviceInfo().Name);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            _deviceList.Add(new StoredDevice() { DeviceObj = e.CaptureDevice, DeviceName = e.CaptureDevice.GetDeviceInfo().Name });
        });

        SelectedDeviceText = string.Format("Selected Device for Trigger Scan Button:\n{0}", e.CaptureDevice.GetDeviceInfo().Name);

        // Last device arrival is the new selected device
        _selectedDevice = e.CaptureDevice;

        // Set SocketCam Overlay to display camera
#if (__IOS__ || __ANDROID__)
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var getStatus = await capture.GetSocketCamStatusAsync();
            if (getStatus.Status == CaptureHelper.SocketCamStatus.Enable) _selectedDevice.SetSocketCamOverlay();
        });
#endif
    }

    private void Capture_DecodedData(object sender, CaptureHelper.DecodedDataArgs e)
    {
        DisplayText = string.Format("Decoded Data: {0}", e.DecodedData.DataToUTF8String);
    }
    // --Device

    private void DeviceList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (deviceList.SelectedIndex != -1)
        {
            _selectedDevice = _deviceList[deviceList.SelectedIndex].DeviceObj;
            SelectedDeviceText = string.Format("Selected Device for Trigger Scan Button:\n{0}", _selectedDevice.GetDeviceInfo().Name);
        }
    }

    private void DeviceList_Focused(object sender, FocusEventArgs e)
    {
        int i = 0;
        int index = -1;

        foreach (var item in _deviceList)
        {
            if (item.DeviceName == _selectedDevice.GetDeviceInfo().Name)
            {
                index = i;
                break;
            }

            i++;
        }

        deviceList.SelectedIndex = index;
    }

    private void DeviceList_Unfocused(object sender, FocusEventArgs e)
    {
        deviceList.SelectedIndex = -1;
    }

    private void Button_TriggerScan(object sender, EventArgs e)
    {
        _selectedDevice?.SetTriggerStartAsync();
    }

    // (Android-iOS) Check if SocketCam is enabled to set the Switch
    private async void getSocketCamStatusInit()
    {
        var getStatus = await capture.GetSocketCamStatusAsync();
        if (getStatus.Status != CaptureHelper.SocketCamStatus.NotSupported)
        {
            IsSocketCamSwitchEnable = true;
        }

        switch (getStatus.Status)
        {
            case CaptureHelper.SocketCamStatus.Enable:
                IsSocketCamEnable = true;
                break;

            case CaptureHelper.SocketCamStatus.Disable:
                IsSocketCamEnable = false;
                break;

        }
    }

    // (Android-iOS) 
    private async void Switch_SocketCamStatus(object sender, ToggledEventArgs e)
    {
        await capture.SetSocketCamStatusAsync(_isSocketCamEnable ? CaptureHelper.SocketCamStatus.Enable : CaptureHelper.SocketCamStatus.Disable);
    }
}

