using Microsoft.Extensions.Logging;
using SmartClinic.Mobile.Constants;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });



#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Configure the HTTP client used for communication with the Smart Clinic API.
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(ApiConstants.BaseUrl)
            });

            // Register authentication services for dependency injection.
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();


            return builder.Build();
        }
    }
}
