using Microsoft.Extensions.Logging;
using SmartClinic.Mobile.Constants;
using SmartClinic.Mobile.Services;
using SmartClinic.Mobile.ViewModels;
using SmartClinic.Mobile.Views;

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

            // Register patient services used for authenticated profile operations.
            builder.Services.AddSingleton<IPatientService, PatientService>();
            builder.Services.AddSingleton<IAppointmentService, AppointmentService>();
            builder.Services.AddSingleton<IDoctorService, DoctorService>();
            builder.Services.AddSingleton<IQueueService, QueueService>();
            builder.Services.AddSingleton<IClinicalService, ClinicalService>();

            // Register authentication ViewModels used by the mobile interface.
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<PatientDashboardViewModel>();
            builder.Services.AddTransient<AppointmentsViewModel>();
            builder.Services.AddTransient<BookAppointmentViewModel>();
            builder.Services.AddTransient<RescheduleAppointmentViewModel>();
            builder.Services.AddTransient<CheckInQueueViewModel>();
            builder.Services.AddTransient<ConsultationHistoryViewModel>();
            builder.Services.AddTransient<ConsultationDetailsViewModel>();

            // Register authentication pages for dependency injection.
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<PatientDashboardPage>();
            builder.Services.AddTransient<AppointmentsPage>();
            builder.Services.AddTransient<BookAppointmentPage>();
            builder.Services.AddTransient<RescheduleAppointmentPage>();
            builder.Services.AddTransient<CheckInQueuePage>();
            builder.Services.AddTransient<ConsultationHistoryPage>();
            builder.Services.AddTransient<ConsultationDetailsPage>();



            return builder.Build();
        }
    }
}
