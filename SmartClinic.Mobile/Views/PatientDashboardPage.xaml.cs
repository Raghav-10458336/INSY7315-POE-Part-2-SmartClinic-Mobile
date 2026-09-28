using Microsoft.Extensions.DependencyInjection;
using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class PatientDashboardPage : ContentPage
{
    private readonly PatientDashboardViewModel _viewModel;

    public PatientDashboardPage(PatientDashboardViewModel viewModel)
    {
        InitializeComponent();

        // Connect the dashboard to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh dashboard information whenever the page appears.
        await _viewModel.LoadDashboardAsync();
    }

    private async void OnAppointmentsTapped(object? sender, TappedEventArgs e)
    {
        // Resolve the appointments page through dependency injection.
        var appointmentsPage = Handler?.MauiContext?.Services
            .GetService<AppointmentsPage>();

        if (appointmentsPage is not null)
        {
            await Navigation.PushAsync(appointmentsPage);
        }
    }

    private async void OnBookAppointmentClicked(object? sender, EventArgs e)
    {
        // Resolve the booking page through dependency injection.
        var bookingPage = Handler?.MauiContext?.Services
            .GetService<BookAppointmentPage>();

        if (bookingPage is not null)
        {
            await Navigation.PushAsync(bookingPage);
        }
    }
}