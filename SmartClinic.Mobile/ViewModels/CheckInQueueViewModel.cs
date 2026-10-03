using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class CheckInQueueViewModel : BaseViewModel
{
    private readonly IQueueService _queueService;

    [ObservableProperty]
    private Appointment? appointment;

    [ObservableProperty]
    private QueueStatus? queueStatus;

    [ObservableProperty]
    private bool hasQueueStatus;

    [ObservableProperty]
    private bool canCheckIn;

    [ObservableProperty]
    private bool isCheckInUnavailable;

    [ObservableProperty]
    private string checkInAvailabilityMessage = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string successMessage = string.Empty;

    [ObservableProperty]
    private bool hasSuccess;

    public CheckInQueueViewModel(IQueueService queueService)
    {
        _queueService = queueService;
        Title = "Check In";
    }

    public async Task InitialiseAsync(Appointment appointment)
    {
        Appointment = appointment;

        QueueStatus = null;
        HasQueueStatus = false;
        CanCheckIn = false;
        IsCheckInUnavailable = false;
        CheckInAvailabilityMessage = string.Empty;

        ClearError();
        ClearSuccess();

        // Restore existing queue information if the patient is already checked in.
        await LoadQueueStatusAsync();

        // If no queue exists yet, determine whether check-in is currently available.
        if (!HasQueueStatus)
        {
            UpdateCheckInAvailability();
        }
    }

    [RelayCommand]
    private async Task CheckInAsync()
    {
        if (Appointment is null || IsBusy || !CanCheckIn)
        {
            return;
        }

        ClearError();
        ClearSuccess();

        try
        {
            IsBusy = true;

            // Check the patient into the appointment through the local queue service.
            var status = await _queueService.CheckInAsync(Appointment.Id);

            if (status is null)
            {
                UpdateCheckInAvailability();

                ShowError(
                    "Unable to check in. Please confirm that your appointment is within the check-in window.");

                return;
            }

            QueueStatus = status;
            HasQueueStatus = true;
            CanCheckIn = false;
            IsCheckInUnavailable = false;

            // Keep the in-memory appointment consistent with the persisted lifecycle.
            Appointment.Status = AppointmentStatus.CheckedIn;

            SuccessMessage =
                "You have checked in successfully. Your queue status is now available.";

            HasSuccess = true;
        }
        catch (Exception)
        {
            ShowError(
                "Unable to check in at the moment. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshQueueAsync()
    {
        await LoadQueueStatusAsync(showError: true);
    }

    private async Task LoadQueueStatusAsync(bool showError = false)
    {
        if (Appointment is null || IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;

            if (showError)
            {
                ClearError();
            }

            // Retrieve the patient's current local queue information.
            var status =
                await _queueService.GetQueueStatusAsync(Appointment.Id);

            if (status is null)
            {
                if (showError)
                {
                    ShowError(
                        "Unable to retrieve the latest queue status.");
                }

                return;
            }

            QueueStatus = status;
            HasQueueStatus = true;
            CanCheckIn = false;
            IsCheckInUnavailable = false;

            // An existing checked-in queue record represents an active check-in.
            if (status.IsCheckedIn)
            {
                Appointment.Status = AppointmentStatus.CheckedIn;
            }
        }
        catch (Exception)
        {
            if (showError)
            {
                ShowError(
                    "Unable to retrieve the latest queue status.");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateCheckInAvailability()
    {
        if (Appointment is null)
        {
            CanCheckIn = false;
            IsCheckInUnavailable = false;
            CheckInAvailabilityMessage = string.Empty;
            return;
        }

        if (Appointment.Status != AppointmentStatus.Scheduled)
        {
            CanCheckIn = false;
            IsCheckInUnavailable = false;
            CheckInAvailabilityMessage = string.Empty;
            return;
        }

        var now = DateTime.Now;
        var checkInOpens =
            Appointment.AppointmentDateTime.AddMinutes(-60);

        var checkInCloses =
            Appointment.AppointmentDateTime.AddMinutes(30);

        if (now < checkInOpens)
        {
            CanCheckIn = false;
            IsCheckInUnavailable = true;

            CheckInAvailabilityMessage =
                $"Check-in opens at {checkInOpens:HH:mm} on {checkInOpens:dd MMM yyyy}.";

            return;
        }

        if (now > checkInCloses)
        {
            CanCheckIn = false;
            IsCheckInUnavailable = true;

            CheckInAvailabilityMessage =
                "The check-in window for this appointment has closed.";

            return;
        }

        CanCheckIn = true;
        IsCheckInUnavailable = false;
        CheckInAvailabilityMessage = string.Empty;
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