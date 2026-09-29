using SmartClinic.Mobile.Constants;
using SmartClinic.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SmartClinic.Mobile.Services;

public class NotificationService : INotificationService
{
    private readonly HttpClient _httpClient;
    private const string TokenKey = "auth_token";

    public NotificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Notification>> GetNotificationsAsync()
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            // Retrieve all notifications for the authenticated patient.
            var response = await _httpClient.GetAsync(
                ApiConstants.NotificationsEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content
                .ReadFromJsonAsync<List<Notification>>() ?? [];
        }
        catch (HttpRequestException)
        {
            return [];
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
            await AddAuthenticationHeaderAsync();

            // Retrieve notifications that have not yet been read.
            var response = await _httpClient.GetAsync(
                ApiConstants.UnreadNotificationsEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content
                .ReadFromJsonAsync<List<Notification>>() ?? [];
        }
        catch (HttpRequestException)
        {
            return [];
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
            await AddAuthenticationHeaderAsync();

            var endpoint = string.Format(
                ApiConstants.MarkNotificationReadEndpoint,
                notificationId);

            // Update the selected notification's read status.
            var response = await _httpClient.PutAsync(
                endpoint,
                null);

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
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
            await AddAuthenticationHeaderAsync();

            // Mark every notification belonging to the patient as read.
            var response = await _httpClient.PutAsync(
                ApiConstants.MarkAllNotificationsReadEndpoint,
                null);

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task AddAuthenticationHeaderAsync()
    {
        var token = await SecureStorage.Default.GetAsync(TokenKey);

        // Attach the patient's JWT to protected notification requests.
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}