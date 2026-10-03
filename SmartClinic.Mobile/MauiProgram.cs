using Microsoft.Extensions.Logging;
using SmartClinic.Mobile.Data;
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

            // Register the local SQLite database used by mobile services.
            builder.Services.AddSingleton<SmartClinicDatabase>();

            // Register application services.
            builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<IPatientService, PatientService>();
            builder.Services.AddSingleton<IAppointmentService, AppointmentService>();
            builder.Services.AddSingleton<IDoctorService, DoctorService>();
            builder.Services.AddSingleton<IQueueService, QueueService>();
            builder.Services.AddSingleton<IClinicalService, ClinicalService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();

            // Register ViewModels.
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<PatientDashboardViewModel>();
            builder.Services.AddTransient<AppointmentsViewModel>();
            builder.Services.AddTransient<BookAppointmentViewModel>();
            builder.Services.AddTransient<RescheduleAppointmentViewModel>();
            builder.Services.AddTransient<CheckInQueueViewModel>();
            builder.Services.AddTransient<ConsultationHistoryViewModel>();
            builder.Services.AddTransient<ConsultationDetailsViewModel>();
            builder.Services.AddTransient<PrescriptionsViewModel>();
            builder.Services.AddTransient<NotificationsViewModel>();
            builder.Services.AddTransient<PatientProfileViewModel>();

            // Register application pages.
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<PatientDashboardPage>();
            builder.Services.AddTransient<AppointmentsPage>();
            builder.Services.AddTransient<BookAppointmentPage>();
            builder.Services.AddTransient<RescheduleAppointmentPage>();
            builder.Services.AddTransient<CheckInQueuePage>();
            builder.Services.AddTransient<ConsultationHistoryPage>();
            builder.Services.AddTransient<ConsultationDetailsPage>();
            builder.Services.AddTransient<PrescriptionsPage>();
            builder.Services.AddTransient<NotificationsPage>();
            builder.Services.AddTransient<PatientProfilePage>();

            return builder.Build();
        }
    }
}