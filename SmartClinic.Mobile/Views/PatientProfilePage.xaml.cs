using Microsoft.Extensions.DependencyInjection;
using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class PatientProfilePage : ContentPage
{
    private readonly PatientProfileViewModel _viewModel;

    public PatientProfilePage(
        PatientProfileViewModel viewModel)
    {
        InitializeComponent();

        // Connect the profile page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Return to the login flow after the authenticated session ends.
        _viewModel.LogoutSucceeded += OnLogoutSucceeded;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh the patient's profile whenever the page is opened.
        await _viewModel.LoadProfileAsync();
    }

    private async void OnLogoutClicked(
        object? sender,
        EventArgs e)
    {
        var shouldLogout = await DisplayAlertAsync(
            "Sign Out",
            "Are you sure you want to sign out?",
            "Sign Out",
            "Cancel");

        if (!shouldLogout)
        {
            return;
        }

        if (_viewModel.LogoutCommand.CanExecute(null))
        {
            _viewModel.LogoutCommand.Execute(null);
        }
    }

    private void OnLogoutSucceeded(
        object? sender,
        EventArgs e)
    {
        var loginPage = Handler?.MauiContext?.Services
            .GetService<LoginPage>();

        if (loginPage is null ||
            Application.Current?.Windows.Count == 0)
        {
            return;
        }

        // Replace the authenticated navigation stack with a fresh login flow.
        Application.Current.Windows[0].Page =
            new NavigationPage(loginPage);
    }
}