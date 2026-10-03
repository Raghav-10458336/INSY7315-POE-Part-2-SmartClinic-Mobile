using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class PatientProfileViewModel : BaseViewModel
{
    private readonly IPatientService _patientService;
    private readonly IAuthenticationService _authenticationService;

    [ObservableProperty]
    private Patient? patient;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string successMessage = string.Empty;

    [ObservableProperty]
    private bool hasSuccess;

    // Allows the page to return to the login flow after sign out.
    public event EventHandler? LogoutSucceeded;

    public PatientProfileViewModel(
        IPatientService patientService,
        IAuthenticationService authenticationService)
    {
        _patientService = patientService;
        _authenticationService = authenticationService;
        Title = "My Profile";
    }

    public async Task LoadProfileAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearMessages();

            // Retrieve the authenticated patient's current profile.
            Patient = await _patientService.GetCurrentPatientAsync();

            if (Patient is null)
            {
                ShowError("Unable to load your profile information.");
            }
        }
        catch (Exception)
        {
            ShowError("Unable to load your profile information.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void EditProfile()
    {
        if (Patient is null)
        {
            return;
        }

        ClearMessages();
        IsEditing = true;
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        if (Patient is null || IsBusy)
        {
            return;
        }

        ClearMessages();

        if (!ValidateProfile())
        {
            return;
        }

        try
        {
            IsBusy = true;

            // Persist the patient's updated personal information.
            var wasUpdated =
                await _patientService.UpdatePatientAsync(Patient);

            if (!wasUpdated)
            {
                ShowError("Unable to save your profile changes.");
                return;
            }

            IsEditing = false;
            ShowSuccess("Your profile has been updated successfully.");
        }
        catch (Exception)
        {
            ShowError("Unable to save your profile changes.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelEditAsync()
    {
        IsEditing = false;
        ClearMessages();

        // Reload the saved profile to discard unsaved changes.
        await LoadProfileAsync();
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearMessages();

            // Remove the authenticated session from secure device storage.
            await _authenticationService.LogoutAsync();

            LogoutSucceeded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception)
        {
            ShowError("Unable to sign out. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool ValidateProfile()
    {
        if (string.IsNullOrWhiteSpace(Patient!.FirstName) ||
            string.IsNullOrWhiteSpace(Patient.LastName))
        {
            ShowError("Please enter your first and last name.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Patient.Email))
        {
            ShowError("Please enter your email address.");
            return false;
        }

        if (!Patient.Email.Contains('@') ||
            !Patient.Email.Contains('.'))
        {
            ShowError("Please enter a valid email address.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Patient.PhoneNumber))
        {
            ShowError("Please enter your phone number.");
            return false;
        }

        return true;
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
        HasSuccess = false;
    }

    private void ShowSuccess(string message)
    {
        SuccessMessage = message;
        HasSuccess = true;
        HasError = false;
    }

    private void ClearMessages()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        HasError = false;
        HasSuccess = false;
    }
}