using SmartClinic.Mobile.Constants;
using SmartClinic.Mobile.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SmartClinic.Mobile.Services;

public class ClinicalService : IClinicalService
{
    private readonly HttpClient _httpClient;

    private const string TokenKey = "auth_token";

    public ClinicalService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Consultation>> GetConsultationsAsync()
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            // Retrieve consultation history for the authenticated patient.
            var response = await _httpClient.GetAsync(
                ApiConstants.ConsultationsEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content
                .ReadFromJsonAsync<List<Consultation>>() ?? [];
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

    public async Task<Consultation?> GetConsultationAsync(
        int consultationId)
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            // Retrieve the full record for the selected consultation.
            var endpoint = string.Format(
                ApiConstants.ConsultationDetailsEndpoint,
                consultationId);

            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<Consultation>();
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

    public async Task<List<Prescription>> GetPrescriptionsAsync()
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            // Retrieve all prescriptions belonging to the authenticated patient.
            var response = await _httpClient.GetAsync(
                ApiConstants.PrescriptionsEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content
                .ReadFromJsonAsync<List<Prescription>>() ?? [];
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

    public async Task<List<Prescription>> GetConsultationPrescriptionsAsync(
        int consultationId)
    {
        try
        {
            await AddAuthenticationHeaderAsync();

            // Retrieve prescriptions issued during the selected consultation.
            var endpoint = string.Format(
                ApiConstants.ConsultationPrescriptionsEndpoint,
                consultationId);

            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                return [];
            }

            return await response.Content
                .ReadFromJsonAsync<List<Prescription>>() ?? [];
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

        // Attach the patient's JWT to protected clinical API requests.
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