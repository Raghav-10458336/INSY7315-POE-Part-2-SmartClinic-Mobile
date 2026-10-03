using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;

    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();

        // Connect the registration page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnCreateAccountClicked(object? sender, EventArgs e)
    {
        // Run the registration workflow through the ViewModel.
        await _viewModel.RegisterCommand.ExecuteAsync(null);

        // Remain on the page when validation or registration fails.
        if (!_viewModel.HasSuccess)
        {
            return;
        }

        // Return to login after the account has been created successfully.
        await DisplayAlertAsync(
            "Account Created",
            "Your SmartClinic account has been created successfully. You can now sign in.",
            "Continue");

        await Navigation.PopAsync();
    }

    private async void OnSignInTapped(object? sender, TappedEventArgs e)
    {
        // Return to the existing login page in the navigation stack.
        await Navigation.PopAsync();
    }
}