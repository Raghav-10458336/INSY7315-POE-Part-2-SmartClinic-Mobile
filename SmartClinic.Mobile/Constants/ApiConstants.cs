namespace SmartClinic.Mobile.Constants;

public static class ApiConstants
{
    // Base address used by the mobile app when communicating with the API.
    // This will be replaced with the hosted API address before deployment.
    public const string BaseUrl = "https://localhost:7001";

    // Authentication endpoints.
    public const string LoginEndpoint = "/api/auth/login";
    public const string RegisterEndpoint = "/api/auth/register";
}