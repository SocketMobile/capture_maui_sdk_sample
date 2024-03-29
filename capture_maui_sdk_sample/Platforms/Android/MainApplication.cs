﻿using Android.App;
using Android.Runtime;
using SocketMobile.Capture;

namespace capture_maui_sdk_sample;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
    }

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
