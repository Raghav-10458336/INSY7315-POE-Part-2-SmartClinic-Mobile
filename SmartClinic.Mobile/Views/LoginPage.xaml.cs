using Microsoft.Extensions.DependencyInjection;
using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();

        // Connect the page to its ViewModel for data binding and commands.
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Navigate only after authentication has completed successfully.
        _viewModel.LoginSucceeded += OnLoginSucceeded;
    }

    private async void OnLoginSucceeded(object? sender, EventArgs e)
    {
        var dashboardPage = Handler?.MauiContext?.Services
            .GetService<PatientDashboardPage>();

        if (dashboardPage is null)
        {
            return;
        }

        // Replace the login flow with the authenticated patient navigation stack.
        Application.Current!.Windows[0].Page =
            new NavigationPage(dashboardPage);

        await Task.CompletedTask;
    }

    private async void OnCreateAccountTapped(
        object? sender,
        TappedEventArgs e)
    {
        // Resolve the registration page through dependency injection.
        var registerPage = Handler?.MauiContext?.Services
            .GetService<RegisterPage>();

        if (registerPage is not null)
        {
            await Navigation.PushAsync(registerPage);
        }
    }
}