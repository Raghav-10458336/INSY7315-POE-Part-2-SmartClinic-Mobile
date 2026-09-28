using SmartClinic.Mobile.ViewModels;
using Microsoft.Extensions.DependencyInjection;

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