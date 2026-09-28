using System.Net.Http.Json;
using SmartClinic.Mobile.Constants;
using SmartClinic.Mobile.DTOs;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;

    private const string TokenKey = "auth_token";
    private const string UserIdKey = "user_id";
    private const string FirstNameKey = "first_name";
    private const string LastNameKey = "last_name";
    private const string EmailKey = "email";
    private const string RoleKey = "user_role";

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                ApiConstants.LoginEndpoint,
                request);

            // Attempt to read the API response even when login is rejected.
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (!response.IsSuccessStatusCode || result is null)
            {
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = result?.Message ?? "Unable to sign in. Please try again."
                };
            }

            if (!result.IsSuccess || string.IsNullOrWhiteSpace(result.Token))
            {
                return result;
            }

            // Store sensitive authentication information using device secure storage.
            await SecureStorage.Default.SetAsync(TokenKey, result.Token);

            await SecureStorage.Default.SetAsync(UserIdKey, result.UserId.ToString());
            await SecureStorage.Default.SetAsync(FirstNameKey, result.FirstName);
            await SecureStorage.Default.SetAsync(LastNameKey, result.LastName);
            await SecureStorage.Default.SetAsync(EmailKey, result.Email);
            await SecureStorage.Default.SetAsync(RoleKey, result.Role.ToString());

            return result;
        }
        catch (HttpRequestException)
        {
            return new LoginResponse
            {
                IsSuccess = false,
                Message = "Unable to connect to the clinic server."
            };
        }
        catch (Exception)
        {
            return new LoginResponse
            {
                IsSuccess = false,
                Message = "An unexpected error occurred while signing in."
            };
        }
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                ApiConstants.RegisterEndpoint,
                request);

            var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();

            if (result is not null)
            {
                return result;
            }

            return new RegisterResponse
            {
                IsSuccess = false,
                Message = "Unable to create the account. Please try again."
            };
        }
        catch (HttpRequestException)
        {
            return new RegisterResponse
            {
                IsSuccess = false,
                Message = "Unable to connect to the clinic server."
            };
        }
        catch (Exception)
        {
            return new RegisterResponse
            {
                IsSuccess = false,
                Message = "An unexpected error occurred while creating the account."
            };
        }
    }

    public Task LogoutAsync()
    {
        // Remove all locally stored authentication information.
        SecureStorage.Default.Remove(TokenKey);
        SecureStorage.Default.Remove(UserIdKey);
        SecureStorage.Default.Remove(FirstNameKey);
        SecureStorage.Default.Remove(LastNameKey);
        SecureStorage.Default.Remove(EmailKey);
        SecureStorage.Default.Remove(RoleKey);

        return Task.CompletedTask;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(TokenKey);

            return !string.IsNullOrWhiteSpace(token);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(TokenKey);

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState();
            }

            var userIdValue = await SecureStorage.Default.GetAsync(UserIdKey);
            var firstName = await SecureStorage.Default.GetAsync(FirstNameKey);
            var lastName = await SecureStorage.Default.GetAsync(LastNameKey);
            var email = await SecureStorage.Default.GetAsync(EmailKey);
            var roleValue = await SecureStorage.Default.GetAsync(RoleKey);

            int.TryParse(userIdValue, out var userId);
            Enum.TryParse<UserRole>(roleValue, out var role);

            return new AuthenticationState
            {
                IsAuthenticated = true,
                UserId = userId,
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                Email = email ?? string.Empty,
                Role = role
            };
        }
        catch (Exception)
        {
            return new AuthenticationState();
        }
    }
}