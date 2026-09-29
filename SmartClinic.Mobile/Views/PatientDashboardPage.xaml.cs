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

    private async void OnAppointmentsTapped(
        object? sender,
        TappedEventArgs e)
    {
        // Resolve the appointments page through dependency injection.
        var appointmentsPage = Handler?.MauiContext?.Services
            .GetService<AppointmentsPage>();

        if (appointmentsPage is not null)
        {
            await Navigation.PushAsync(appointmentsPage);
        }
    }

    private async void OnBookAppointmentClicked(
        object? sender,
        EventArgs e)
    {
        // Resolve the booking page through dependency injection.
        var bookingPage = Handler?.MauiContext?.Services
            .GetService<BookAppointmentPage>();

        if (bookingPage is not null)
        {
            await Navigation.PushAsync(bookingPage);
        }
    }

    private async void OnPrescriptionsTapped(
        object? sender,
        TappedEventArgs e)
    {
        // Open the authenticated patient's prescription history.
        var prescriptionsPage = Handler?.MauiContext?.Services
            .GetService<PrescriptionsPage>();

        if (prescriptionsPage is not null)
        {
            await Navigation.PushAsync(prescriptionsPage);
        }
    }

    private async void OnConsultationHistoryTapped(
        object? sender,
        TappedEventArgs e)
    {
        // Open the authenticated patient's consultation history.
        var historyPage = Handler?.MauiContext?.Services
            .GetService<ConsultationHistoryPage>();

        if (historyPage is not null)
        {
            await Navigation.PushAsync(historyPage);
        }
    }

    private async void OnNotificationsTapped(
        object? sender,
        TappedEventArgs e)
    {
        // Open the authenticated patient's notifications and reminders.
        var notificationsPage = Handler?.MauiContext?.Services
            .GetService<NotificationsPage>();

        if (notificationsPage is not null)
        {
            await Navigation.PushAsync(notificationsPage);
        }
    }
}