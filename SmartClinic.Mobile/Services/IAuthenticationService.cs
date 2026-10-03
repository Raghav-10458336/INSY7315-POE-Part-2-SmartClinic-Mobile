using SmartClinic.Mobile.DTOs;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public interface IAuthenticationService
{
    // Authenticates a user against the local account database.
    Task<LoginResponse> LoginAsync(LoginRequest request);

    // Creates a new patient account and profile.
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);

    // Ends the current authenticated session.
    Task LogoutAsync();

    // Checks whether a user currently has an authenticated session.
    Task<bool> IsAuthenticatedAsync();

    // Returns information about the currently authenticated user.
    Task<AuthenticationState> GetAuthenticationStateAsync();
}