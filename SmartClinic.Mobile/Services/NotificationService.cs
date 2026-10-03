using SmartClinic.Mobile.Data;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public class NotificationService : INotificationService
{
    private readonly SmartClinicDatabase _smartClinicDatabase;
    private readonly IAuthenticationService _authenticationService;

    public NotificationService(
        SmartClinicDatabase smartClinicDatabase,
        IAuthenticationService authenticationService)
    {
        _smartClinicDatabase = smartClinicDatabase;
        _authenticationService = authenticationService;
    }

    public async Task<List<Notification>> GetNotificationsAsync()
    {
        try
        {
            var userId = await GetCurrentUserIdAsync();

            if (userId is null)
            {
                return [];
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            // Return the signed-in user's notifications newest first.
            return await database.Table<Notification>()
                .Where(notification => notification.UserId == userId.Value)
                .OrderByDescending(notification => notification.CreatedAt)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<List<Notification>> GetUnreadNotificationsAsync()
    {
        try
        {
            var userId = await GetCurrentUserIdAsync();

            if (userId is null)
            {
                return [];
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            // Only unread notifications belonging to the signed-in user are returned.
            return await database.Table<Notification>()
                .Where(notification =>
                    notification.UserId == userId.Value &&
                    !notification.IsRead)
                .OrderByDescending(notification => notification.CreatedAt)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        try
        {
            if (notificationId <= 0)
            {
                return false;
            }

            var userId = await GetCurrentUserIdAsync();

            if (userId is null)
            {
                return false;
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            // Ownership is verified before the notification can be changed.
            var notification = await database.Table<Notification>()
                .Where(existing =>
                    existing.Id == notificationId &&
                    existing.UserId == userId.Value)
                .FirstOrDefaultAsync();

            if (notification is null)
            {
                return false;
            }

            if (notification.IsRead)
            {
                return true;
            }

            notification.IsRead = true;

            return await database.UpdateAsync(notification) > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> MarkAllAsReadAsync()
    {
        try
        {
            var userId = await GetCurrentUserIdAsync();

            if (userId is null)
            {
                return false;
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            var unreadNotifications = await database.Table<Notification>()
                .Where(notification =>
                    notification.UserId == userId.Value &&
                    !notification.IsRead)
                .ToListAsync();

            // Nothing to update is still a successful operation.
            if (unreadNotifications.Count == 0)
            {
                return true;
            }

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            var updatedRows =
                await database.UpdateAllAsync(unreadNotifications);

            return updatedRows == unreadNotifications.Count;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task<int?> GetCurrentUserIdAsync()
    {
        var authenticationState =
            await _authenticationService.GetAuthenticationStateAsync();

        if (!authenticationState.IsAuthenticated ||
            authenticationState.UserId is null)
        {
            return null;
        }

        return authenticationState.UserId.Value;
    }
}