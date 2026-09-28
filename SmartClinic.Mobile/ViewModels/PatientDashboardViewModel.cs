using CommunityToolkit.Mvvm.ComponentModel;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class PatientDashboardViewModel : BaseViewModel
{
    private readonly IPatientService _patientService;
    private readonly IAppointmentService _appointmentService;

    [ObservableProperty]
    private Patient? patient;

    [ObservableProperty]
    private Appointment? upcomingAppointment;

    [ObservableProperty]
    private string welcomeMessage = "Welcome";

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private bool hasUpcomingAppointment;

    [ObservableProperty]
    private bool hasNoUpcomingAppointment = true;

    public PatientDashboardViewModel(
        IPatientService patientService,
        IAppointmentService appointmentService)
    {
        _patientService = patientService;
        _appointmentService = appointmentService;

        Title = "Home";
    }

    public async Task LoadDashboardAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            // Load the patient profile and upcoming appointment.
            Patient = await _patientService.GetCurrentPatientAsync();
            UpcomingAppointment =
                await _appointmentService.GetUpcomingAppointmentAsync();

            if (Patient is null)
            {
                ShowError("Unable to load your patient information.");
            }
            else
            {
                WelcomeMessage = $"Welcome, {Patient.FirstName}";
            }

            // Update the dashboard state based on appointment availability.
            HasUpcomingAppointment = UpcomingAppointment is not null;
            HasNoUpcomingAppointment = UpcomingAppointment is null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }
}
