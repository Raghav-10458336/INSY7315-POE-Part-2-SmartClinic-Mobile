using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClinic.Mobile.DTOs;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authenticationService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    // Allows the page to respond when authentication succeeds.
    public event EventHandler? LoginSucceeded;

    public LoginViewModel(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        Title = "Sign In";
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        // Prevent multiple login requests from running at the same time.
        if (IsBusy)
        {
            return;
        }

        ClearError();

        if (!ValidateInput())
        {
            return;
        }

        try
        {
            IsBusy = true;

            var request = new LoginRequest
            {
                Email = Email.Trim(),
                Password = Password
            };

            var response = await _authenticationService.LoginAsync(request);

            if (!response.IsSuccess)
            {
                ShowError(response.Message);
                return;
            }

            // Notify the page that authenticated navigation can begin.
            LoginSucceeded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception)
        {
            ShowError(
                "An unexpected error occurred while signing in. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool ValidateInput()
    {
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

        if (string.IsNullOrWhiteSpace(Password))
        {
            ShowError("Please enter your password.");
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
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }
}