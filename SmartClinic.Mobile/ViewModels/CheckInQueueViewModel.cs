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
    private bool canCheckIn = true;

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
        CanCheckIn = true;

        ClearError();
        ClearSuccess();

        // Restore existing queue information if the patient is already checked in.
        await LoadQueueStatusAsync();
    }

    [RelayCommand]
    private async Task CheckInAsync()
    {
        if (Appointment is null || IsBusy)
        {
            return;
        }

        ClearError();
        ClearSuccess();

        try
        {
            IsBusy = true;

            // Ask the backend to check the patient into this appointment.
            var status = await _queueService.CheckInAsync(Appointment.Id);

            if (status is null)
            {
                ShowError(
                    "Unable to check in. Please confirm that your appointment is eligible for check-in.");
                return;
            }

            QueueStatus = status;
            HasQueueStatus = true;
            CanCheckIn = false;

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

            // Retrieve the latest queue position and status from the backend.
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
            CanCheckIn = !status.IsCheckedIn;
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