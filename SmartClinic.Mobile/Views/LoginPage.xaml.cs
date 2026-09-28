using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();

        // Connect the page to its ViewModel for data binding and commands.
        BindingContext = viewModel;
    }

    private async void OnCreateAccountTapped(object? sender, TappedEventArgs e)
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