using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class NotificationsViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;

    public ObservableCollection<Notification> Notifications { get; } = [];

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string successMessage = string.Empty;

    [ObservableProperty]
    private bool hasSuccess;

    [ObservableProperty]
    private bool hasNotifications;

    [ObservableProperty]
    private bool hasNoNotifications = true;

    [ObservableProperty]
    private bool hasUnreadNotifications;

    [ObservableProperty]
    private int unreadCount;

    public NotificationsViewModel(
        INotificationService notificationService)
    {
        _notificationService = notificationService;
        Title = "Notifications";
    }

    public async Task LoadNotificationsAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearMessages();

            Notifications.Clear();

            // Retrieve the patient's notifications with the newest first.
            var notifications =
                await _notificationService.GetNotificationsAsync();

            foreach (var notification in notifications
                .OrderByDescending(n => n.CreatedAt))
            {
                Notifications.Add(notification);
            }

            UpdateNotificationState();
        }
        catch (Exception)
        {
            ShowError("Unable to load your notifications.");
            UpdateNotificationState();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task MarkAsReadAsync(Notification? notification)
    {
        if (notification is null ||
            notification.IsRead ||
            IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearMessages();

            var wasUpdated =
                await _notificationService.MarkAsReadAsync(
                    notification.Id);

            if (!wasUpdated)
            {
                ShowError(
                    "Unable to update this notification.");

                return;
            }

            notification.IsRead = true;

            // Refresh the collection so the updated read state is displayed.
            var index = Notifications.IndexOf(notification);

            if (index >= 0)
            {
                Notifications[index] = notification;
            }

            UpdateNotificationState();
        }
        catch (Exception)
        {
            ShowError(
                "Unable to update this notification.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task MarkAllAsReadAsync()
    {
        if (IsBusy || !HasUnreadNotifications)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearMessages();

            var wasUpdated =
                await _notificationService.MarkAllAsReadAsync();

            if (!wasUpdated)
            {
                ShowError(
                    "Unable to mark all notifications as read.");

                return;
            }

            foreach (var notification in Notifications)
            {
                notification.IsRead = true;
            }

            UpdateNotificationState();

            ShowSuccess(
                "All notifications have been marked as read.");
        }
        catch (Exception)
        {
            ShowError(
                "Unable to mark all notifications as read.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateNotificationState()
    {
        HasNotifications = Notifications.Count > 0;
        HasNoNotifications = !HasNotifications;

        UnreadCount =
            Notifications.Count(n => !n.IsRead);

        HasUnreadNotifications = UnreadCount > 0;
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
        HasSuccess = false;
    }

    private void ShowSuccess(string message)
    {
        SuccessMessage = message;
        HasSuccess = true;
        HasError = false;
    }

    private void ClearMessages()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
        HasError = false;
        HasSuccess = false;
    }
}