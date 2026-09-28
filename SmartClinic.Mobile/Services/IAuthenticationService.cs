using SmartClinic.Mobile.DTOs;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public interface IAuthenticationService
{
    // Sends the user's credentials to the API for authentication.
    Task<LoginResponse> LoginAsync(LoginRequest request);

    // Sends patient registration information to the API.
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);

    // Ends the local authenticated session and removes stored credentials.
    Task LogoutAsync();

    // Checks whether a valid authentication token is stored on the device.
    Task<bool> IsAuthenticatedAsync();

    // Returns information about the currently authenticated user.
    Task<AuthenticationState> GetAuthenticationStateAsync();
}
