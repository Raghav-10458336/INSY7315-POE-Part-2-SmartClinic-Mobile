using SmartClinic.Mobile.Constants;
using SmartClinic.Mobile.DTOs;
using SmartClinic.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace SmartClinic.Mobile.Services;

public class AppointmentService : IAppointmentService
{
    private readonly HttpClient _httpClient;

    private const string TokenKey = "auth_token";

    public AppointmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Appointment>> GetAppointmentsAsync()
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            var response = await _httpClient.GetAsync(
                ApiConstants.AppointmentsEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content.ReadFromJsonAsync<List<Appointment>>()
                ?? [];
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

    public async Task<Appointment?> GetUpcomingAppointmentAsync()
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            var response = await _httpClient.GetAsync(
                ApiConstants.UpcomingAppointmentEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Appointment>();
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

    public async Task<Appointment?> CreateAppointmentAsync(
    CreateAppointmentRequest request)
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            // Submit the selected doctor, appointment time and visit reason.
            var response = await _httpClient.PostAsJsonAsync(
                ApiConstants.AppointmentsEndpoint,
                request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Appointment>();
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
