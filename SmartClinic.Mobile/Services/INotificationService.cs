using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public interface INotificationService
{
    // Retrieves all notifications for the authenticated patient.
    Task<List<Notification>> GetNotificationsAsync();

    // Retrieves notifications the patient has not yet read.
    Task<List<Notification>> GetUnreadNotificationsAsync();

    // Marks a selected notification as read.
    Task<bool> MarkAsReadAsync(int notificationId);

    // Marks all notifications belonging to the patient as read.
    Task<bool> MarkAllAsReadAsync();
}