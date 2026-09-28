using System.Net.Http.Headers;
using System.Net.Http.Json;
using SmartClinic.Mobile.Constants;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public class PatientService : IPatientService
{
    private readonly HttpClient _httpClient;

    private const string TokenKey = "auth_token";

    public PatientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Patient?> GetCurrentPatientAsync()
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            var response = await _httpClient.GetAsync(
                ApiConstants.CurrentPatientEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Patient>();
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

    public async Task<bool> UpdatePatientAsync(Patient patient)
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            var response = await _httpClient.PutAsJsonAsync(
                ApiConstants.UpdatePatientEndpoint,
                patient);

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

        // Attach the JWT only when an authenticated token is available.
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
