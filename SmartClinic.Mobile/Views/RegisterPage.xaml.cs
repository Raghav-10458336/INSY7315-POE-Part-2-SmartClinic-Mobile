using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();

        // Connect the registration page to its ViewModel.
        BindingContext = viewModel;
    }

    private async void OnSignInTapped(object? sender, TappedEventArgs e)
    {
        // Return to the existing login page in the navigation stack.
        await Navigation.PopAsync();
    }
}