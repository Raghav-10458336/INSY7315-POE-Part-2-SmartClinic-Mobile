using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class AppointmentsViewModel : BaseViewModel
{
    private readonly IAppointmentService _appointmentService;

    public ObservableCollection<Appointment> Appointments { get; } = [];

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string successMessage = string.Empty;

    [ObservableProperty]
    private bool hasSuccess;

    [ObservableProperty]
    private bool hasAppointments;

    [ObservableProperty]
    private bool hasNoAppointments = true;

    public AppointmentsViewModel(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
        Title = "Appointments";
    }

    public async Task LoadAppointmentsAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            Appointments.Clear();

            // Retrieve appointments belonging to the authenticated patient.
            var appointments = await _appointmentService.GetAppointmentsAsync();

            foreach (var appointment in appointments
                         .OrderBy(a => a.AppointmentDateTime))
            {
                Appointments.Add(appointment);
            }

            UpdateAppointmentState();
        }
        catch (Exception)
        {
            ShowError("Unable to load your appointments.");

            HasAppointments = false;
            HasNoAppointments = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAppointmentAsync(Appointment? appointment)
    {
        if (appointment is null || IsBusy)
        {
            return;
        }

        ClearError();
        ClearSuccess();

        // Completed or already cancelled appointments cannot be cancelled.
        if (appointment.Status == AppointmentStatus.Completed ||
            appointment.Status == AppointmentStatus.Cancelled)
        {
            ShowError("This appointment cannot be cancelled.");
            return;
        }

        try
        {
            IsBusy = true;

            // Request cancellation of the selected appointment.
            var wasCancelled =
                await _appointmentService.CancelAppointmentAsync(appointment.Id);

            if (!wasCancelled)
            {
                ShowError("Unable to cancel the appointment. Please try again.");
                return;
            }

            // Remove the cancelled appointment from the active list.
            Appointments.Remove(appointment);
            UpdateAppointmentState();

            SuccessMessage = "Your appointment has been cancelled successfully.";
            HasSuccess = true;
        }
        catch (Exception)
        {
            ShowError("Unable to cancel the appointment. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateAppointmentState()
    {
        HasAppointments = Appointments.Count > 0;
        HasNoAppointments = !HasAppointments;
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    private void ClearSuccess()
    {
        SuccessMessage = string.Empty;
        HasSuccess = false;
    }
}