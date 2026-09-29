using Microsoft.Extensions.DependencyInjection;
using SmartClinic.Mobile.Models;
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

    private async void OnCancelAppointmentClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button ||
            button.BindingContext is not Appointment appointment)
        {
            return;
        }

        // Require confirmation before submitting a cancellation.
        var confirmed = await DisplayAlertAsync(
            "Cancel Appointment",
            $"Are you sure you want to cancel your appointment with {appointment.DoctorName}?",
            "Cancel Appointment",
            "Keep Appointment");

        if (!confirmed)
        {
            return;
        }

        await _viewModel.CancelAppointmentCommand.ExecuteAsync(appointment);
    }

    private async void OnRescheduleAppointmentClicked(
    object? sender,
    EventArgs e)
    {
        if (sender is not Button button ||
            button.BindingContext is not Appointment appointment)
        {
            return;
        }

        // Resolve the reschedule page through dependency injection.
        var reschedulePage = Handler?.MauiContext?.Services
            .GetService<RescheduleAppointmentPage>();

        if (reschedulePage is null)
        {
            return;
        }

        // Pass the selected appointment to the reschedule workflow.
        await reschedulePage.InitialiseAsync(appointment);

        await Navigation.PushAsync(reschedulePage);
    }

    private async void OnCheckInAppointmentClicked(
    object? sender,
    EventArgs e)
    {
        if (sender is not Button button ||
            button.BindingContext is not Appointment appointment)
        {
            return;
        }

        // Resolve the check-in page through dependency injection.
        var checkInPage = Handler?.MauiContext?.Services
            .GetService<CheckInQueuePage>();

        if (checkInPage is null)
        {
            return;
        }

        // Pass the selected appointment into the check-in workflow.
        await checkInPage.InitialiseAsync(appointment);

        await Navigation.PushAsync(checkInPage);
    }
}