using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using SmartClinic.Mobile.Data;
using SmartClinic.Mobile.DTOs;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly SmartClinicDatabase _smartClinicDatabase;
    private readonly PasswordHasher<User> _passwordHasher = new();

    private const string SessionKey = "authenticated_session";
    private const string UserIdKey = "user_id";
    private const string FirstNameKey = "first_name";
    private const string LastNameKey = "last_name";
    private const string EmailKey = "email";
    private const string RoleKey = "user_role";

    public AuthenticationService(SmartClinicDatabase smartClinicDatabase)
    {
        _smartClinicDatabase = smartClinicDatabase;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "Email and password are required."
                };
            }

            var email = NormalizeEmail(request.Email);
            var database = await _smartClinicDatabase.GetConnectionAsync();

            var user = await database.Table<User>()
                .Where(existingUser => existingUser.Email == email)
                .FirstOrDefaultAsync();

            if (user is null)
            {
                return InvalidCredentials();
            }

            if (!user.IsActive)
            {
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "This account is currently inactive."
                };
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return InvalidCredentials();
            }

            // Refresh older password hashes when the framework recommends it.
            if (verificationResult ==
                PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(
                    user,
                    request.Password);

                await database.UpdateAsync(user);
            }

            await SaveSessionAsync(user);

            return new LoginResponse
            {
                IsSuccess = true,
                Message = "Sign in successful.",
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
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
            var validationMessage = ValidateRegistration(request);

            if (validationMessage is not null)
            {
                return new RegisterResponse
                {
                    IsSuccess = false,
                    Message = validationMessage
                };
            }

            var email = NormalizeEmail(request.Email);
            var database = await _smartClinicDatabase.GetConnectionAsync();

            var existingUser = await database.Table<User>()
                .Where(user => user.Email == email)
                .FirstOrDefaultAsync();

            if (existingUser is not null)
            {
                return new RegisterResponse
                {
                    IsSuccess = false,
                    Message =
                        "An account with this email address already exists."
                };
            }

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                Role = UserRole.Patient,
                IsActive = true
            };

            // Only the generated password hash is persisted.
            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                request.Password);

            await database.InsertAsync(user);

            try
            {
                var patient = new Patient
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = request.PhoneNumber.Trim(),
                    DateOfBirth = request.DateOfBirth.Date,
                    Gender = request.Gender.Trim()
                };

                await database.InsertAsync(patient);
            }
            catch
            {
                // Prevent a partial account if patient creation fails.
                await database.DeleteAsync(user);
                throw;
            }

            return new RegisterResponse
            {
                IsSuccess = true,
                Message = "Account created successfully.",
                UserId = user.Id
            };
        }
        catch (Exception)
        {
            return new RegisterResponse
            {
                IsSuccess = false,
                Message =
                    "An unexpected error occurred while creating the account."
            };
        }
    }

    public Task LogoutAsync()
    {
        ClearSession();

        return Task.CompletedTask;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var session = await SecureStorage.Default.GetAsync(SessionKey);
            var userIdValue = await SecureStorage.Default.GetAsync(UserIdKey);

            if (session != "active" ||
                !int.TryParse(userIdValue, out var userId) ||
                userId <= 0)
            {
                return false;
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();
            var user = await database.FindAsync<User>(userId);

            return user is not null && user.IsActive;
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
            if (!await IsAuthenticatedAsync())
            {
                return new AuthenticationState();
            }

            var userIdValue =
                await SecureStorage.Default.GetAsync(UserIdKey);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return new AuthenticationState();
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();
            var user = await database.FindAsync<User>(userId);

            if (user is null || !user.IsActive)
            {
                ClearSession();
                return new AuthenticationState();
            }

            return new AuthenticationState
            {
                IsAuthenticated = true,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            };
        }
        catch (Exception)
        {
            return new AuthenticationState();
        }
    }

    private static string? ValidateRegistration(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName))
        {
            return "First name and last name are required.";
        }

        if (request.FirstName.Trim().Length < 2 ||
            request.LastName.Trim().Length < 2)
        {
            return "Please enter a valid first name and last name.";
        }

        if (string.IsNullOrWhiteSpace(request.Email) ||
            !IsValidEmail(request.Email))
        {
            return "Please enter a valid email address.";
        }

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            return "Phone number is required.";
        }

        var phoneNumber = request.PhoneNumber.Trim();

        if (phoneNumber.Length < 7 || phoneNumber.Length > 20)
        {
            return "Please enter a valid phone number.";
        }

        if (request.DateOfBirth == default ||
            request.DateOfBirth.Date > DateTime.Today)
        {
            return "Please enter a valid date of birth.";
        }

        if (request.DateOfBirth.Date >
            DateTime.Today.AddYears(-13))
        {
            return "Patients must be at least 13 years old to register.";
        }

        if (string.IsNullOrWhiteSpace(request.Gender))
        {
            return "Gender is required.";
        }

        if (string.IsNullOrWhiteSpace(request.Password) ||
            request.Password.Length < 8)
        {
            return "Password must contain at least 8 characters.";
        }

        if (!request.Password.Any(char.IsUpper) ||
            !request.Password.Any(char.IsLower) ||
            !request.Password.Any(char.IsDigit))
        {
            return
                "Password must include an uppercase letter, lowercase letter and number.";
        }

        if (request.Password != request.ConfirmPassword)
        {
            return "Passwords do not match.";
        }

        return null;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var trimmedEmail = email.Trim();
            var address = new MailAddress(trimmedEmail);

            return string.Equals(
                address.Address,
                trimmedEmail,
                StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    private async Task SaveSessionAsync(User user)
    {
        // SecureStorage holds session information, never the user's password.
        await SecureStorage.Default.SetAsync(SessionKey, "active");
        await SecureStorage.Default.SetAsync(
            UserIdKey,
            user.Id.ToString());

        await SecureStorage.Default.SetAsync(
            FirstNameKey,
            user.FirstName);

        await SecureStorage.Default.SetAsync(
            LastNameKey,
            user.LastName);

        await SecureStorage.Default.SetAsync(
            EmailKey,
            user.Email);

        await SecureStorage.Default.SetAsync(
            RoleKey,
            user.Role.ToString());
    }

    private static void ClearSession()
    {
        SecureStorage.Default.Remove(SessionKey);
        SecureStorage.Default.Remove(UserIdKey);
        SecureStorage.Default.Remove(FirstNameKey);
        SecureStorage.Default.Remove(LastNameKey);
        SecureStorage.Default.Remove(EmailKey);
        SecureStorage.Default.Remove(RoleKey);
    }

    private static LoginResponse InvalidCredentials()
    {
        // Avoid revealing whether a specific email account exists.
        return new LoginResponse
        {
            IsSuccess = false,
            Message = "Invalid email or password."
        };
    }
}