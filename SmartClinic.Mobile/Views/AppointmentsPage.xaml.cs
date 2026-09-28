using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class AppointmentsPage : ContentPage
{
    private readonly AppointmentsViewModel _viewModel;

    public AppointmentsPage(AppointmentsViewModel viewModel)
    {
        InitializeComponent();

        // Connect the appointments page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh the patient's appointments whenever the page appears.
        await _viewModel.LoadAppointmentsAsync();
    }
}