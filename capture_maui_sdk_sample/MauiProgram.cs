namespace capture_maui_sdk_sample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSansRegular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSansSemibold.ttf", "OpenSansSemibold");
			});

		return builder.Build();
	}
}
