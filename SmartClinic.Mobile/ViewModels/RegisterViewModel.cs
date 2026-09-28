using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClinic.Mobile.DTOs;
using SmartClinic.Mobile.Services;
using System.Reflection;

namespace SmartClinic.Mobile.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authenticationService;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    [ObservableProperty]
    private DateTime dateOfBirth = DateTime.Today.AddYears(-18);

    [ObservableProperty]
    private string gender = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string successMessage = string.Empty;

    [ObservableProperty]
    private bool hasSuccess;

    public RegisterViewModel(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        Title = "Create Account";
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        // Prevent duplicate registration requests.
        if (IsBusy)
        {
            return;
        }

        ClearMessages();

        if (!ValidateInput())
        {
            return;
        }

        try
        {
            IsBusy = true;

            var request = new RegisterRequest
            {
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim(),
                Email = Email.Trim(),
                PhoneNumber = PhoneNumber.Trim(),
                DateOfBirth = DateOfBirth,
                Gender = Gender,
                Password = Password,
                ConfirmPassword = ConfirmPassword
            };

            var response = await _authenticationService.RegisterAsync(request);

            if (!response.IsSuccess)
            {
                ShowError(response.Message);
                return;
            }

            HasSuccess = true;
            SuccessMessage = response.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            ShowError("Please enter your first name.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            ShowError("Please enter your last name.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            ShowError("Please enter your email address.");
            return false;
        }

        if (!IsValidEmail(Email))
        {
            ShowError("Please enter a valid email address.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            ShowError("Please enter your phone number.");
            return false;
        }

        if (DateOfBirth > DateTime.Today)
        {
            ShowError("Date of birth cannot be in the future.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Gender))
        {
            ShowError("Please select your gender.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ShowError("Please enter a password.");
            return false;
        }

        if (Password.Length < 8)
        {
            ShowError("Password must be at least 8 characters long.");
            return false;
        }

        if (Password != ConfirmPassword)
        {
            ShowError("Passwords do not match.");
            return false;
        }

        return true;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var address = new System.Net.Mail.MailAddress(email);

            return address.Address.Equals(
                email.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
        HasSuccess = false;
    }

    private void ClearMessages()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        HasError = false;
        HasSuccess = false;
    }
}
