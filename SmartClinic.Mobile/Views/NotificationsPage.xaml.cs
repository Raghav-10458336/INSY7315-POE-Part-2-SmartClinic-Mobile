using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class NotificationsPage : ContentPage
{
    private readonly NotificationsViewModel _viewModel;

    public NotificationsPage(
        NotificationsViewModel viewModel)
    {
        InitializeComponent();

        // Connect the notifications page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh notifications whenever the patient opens this page.
        await _viewModel.LoadNotificationsAsync();
    }
}