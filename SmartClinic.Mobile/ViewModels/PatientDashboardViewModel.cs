using CommunityToolkit.Mvvm.ComponentModel;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class PatientDashboardViewModel : BaseViewModel
{
    private readonly IPatientService _patientService;

    [ObservableProperty]
    private Patient? patient;

    [ObservableProperty]
    private string welcomeMessage = "Welcome";

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    public PatientDashboardViewModel(IPatientService patientService)
    {
        _patientService = patientService;
        Title = "Home";
    }

    public async Task LoadPatientAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            // Retrieve the profile belonging to the authenticated patient.
            Patient = await _patientService.GetCurrentPatientAsync();

            if (Patient is null)
            {
                ShowError("Unable to load your patient information.");
                return;
            }

            WelcomeMessage = $"Welcome, {Patient.FirstName}";
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
