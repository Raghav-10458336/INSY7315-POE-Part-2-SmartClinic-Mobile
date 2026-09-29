using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClinic.Mobile.DTOs;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class RescheduleAppointmentViewModel : BaseViewModel
{
    private readonly IDoctorService _doctorService;
    private readonly IAppointmentService _appointmentService;

    public ObservableCollection<DoctorAvailability> AvailableSlots { get; } = [];

    [ObservableProperty]
    private Appointment? appointment;

    [ObservableProperty]
    private DoctorAvailability? selectedSlot;

    [ObservableProperty]
    private bool hasAvailableSlots;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string successMessage = string.Empty;

    [ObservableProperty]
    private bool hasSuccess;

    public RescheduleAppointmentViewModel(
        IDoctorService doctorService,
        IAppointmentService appointmentService)
    {
        _doctorService = doctorService;
        _appointmentService = appointmentService;

        Title = "Reschedule Appointment";
    }

    public async Task InitialiseAsync(Appointment appointment)
    {
        Appointment = appointment;

        AvailableSlots.Clear();
        SelectedSlot = null;
        HasAvailableSlots = false;

        ClearError();
        ClearSuccess();

        await LoadAvailabilityAsync();
    }

    private async Task LoadAvailabilityAsync()
    {
        if (Appointment is null || IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            // Retrieve available times for the appointment's existing doctor.
            var availability =
                await _doctorService.GetDoctorAvailabilityAsync(
                    Appointment.DoctorId);

            foreach (var slot in availability
                         .Where(a => a.IsAvailable)
                         .OrderBy(a => a.StartDateTime))
            {
                AvailableSlots.Add(slot);
            }

            HasAvailableSlots = AvailableSlots.Count > 0;

            if (!HasAvailableSlots)
            {
                ShowError(
                    "No alternative appointment times are currently available.");
            }
        }
        catch (Exception)
        {
            ShowError(
                "Unable to load alternative appointment times.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RescheduleAppointmentAsync()
    {
        if (Appointment is null || IsBusy)
        {
            return;
        }

        ClearError();
        ClearSuccess();

        if (SelectedSlot is null)
        {
            ShowError("Please select a new appointment time.");
            return;
        }

        if (SelectedSlot.StartDateTime ==
            Appointment.AppointmentDateTime)
        {
            ShowError(
                "Please select a different appointment time.");
            return;
        }

        try
        {
            IsBusy = true;

            var request = new RescheduleAppointmentRequest
            {
                AppointmentDateTime = SelectedSlot.StartDateTime
            };

            // Submit the newly selected time for the existing appointment.
            var updatedAppointment =
                await _appointmentService.RescheduleAppointmentAsync(
                    Appointment.Id,
                    request);

            if (updatedAppointment is null)
            {
                ShowError(
                    "Unable to reschedule the appointment. Please try again.");
                return;
            }

            Appointment = updatedAppointment;

            SuccessMessage =
                "Your appointment has been rescheduled successfully.";

            HasSuccess = true;
        }
        catch (Exception)
        {
            ShowError(
                "Unable to reschedule the appointment. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
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