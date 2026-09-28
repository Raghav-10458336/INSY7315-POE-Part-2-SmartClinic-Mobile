using System.Net.Http.Headers;
using System.Net.Http.Json;
using SmartClinic.Mobile.Constants;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public class DoctorService : IDoctorService
{
    private readonly HttpClient _httpClient;

    private const string TokenKey = "auth_token";

    public DoctorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Doctor>> GetDoctorsAsync()
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            var response = await _httpClient.GetAsync(
                ApiConstants.DoctorsEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content.ReadFromJsonAsync<List<Doctor>>()
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

    public async Task<List<DoctorAvailability>> GetDoctorAvailabilityAsync(
        int doctorId)
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            var endpoint = string.Format(
                ApiConstants.DoctorAvailabilityEndpoint,
                doctorId);

            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content
                .ReadFromJsonAsync<List<DoctorAvailability>>()
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

    private async Task AddAuthenticationHeaderAsync()
    {
        var token = await SecureStorage.Default.GetAsync(TokenKey);

        // Attach the patient's JWT to authenticated doctor requests.
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