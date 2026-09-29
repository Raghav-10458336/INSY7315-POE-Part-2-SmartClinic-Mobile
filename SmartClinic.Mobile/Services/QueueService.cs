using SmartClinic.Mobile.Constants;
using SmartClinic.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SmartClinic.Mobile.Services;

public class QueueService : IQueueService
{
    private readonly HttpClient _httpClient;

    private const string TokenKey = "auth_token";

    public QueueService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<QueueStatus?> CheckInAsync(int appointmentId)
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            // Check the authenticated patient into the selected appointment.
            var endpoint = string.Format(
                ApiConstants.CheckInAppointmentEndpoint,
                appointmentId);

            var response = await _httpClient.PostAsync(
                endpoint,
                null);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<QueueStatus>();
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<QueueStatus?> GetQueueStatusAsync(int appointmentId)
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            // Retrieve the latest queue information for the appointment.
            var endpoint = string.Format(
                ApiConstants.QueueStatusEndpoint,
                appointmentId);

            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<QueueStatus>();
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task AddAuthenticationHeaderAsync()
    {
        var token = await SecureStorage.Default.GetAsync(TokenKey);

        // Attach the patient's JWT to authenticated API requests.
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