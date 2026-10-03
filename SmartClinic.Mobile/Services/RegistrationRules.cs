using System.Net.Mail;

namespace SmartClinic.Mobile.Services;

// Contains reusable registration validation rules.
public static class RegistrationRules
{
    public static bool IsValidName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) &&
               name.Trim().Length >= 2;
    }

    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var address = new MailAddress(email.Trim());
            return address.Address.Equals(
                email.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return false;
        }

        var trimmedPhone = phoneNumber.Trim();

        return trimmedPhone.Length >= 7 &&
               trimmedPhone.Length <= 20;
    }

    public static bool IsValidDateOfBirth(DateTime dateOfBirth, DateTime today)
    {
        return dateOfBirth.Date <= today.Date;
    }

    public static bool MeetsMinimumAge(
        DateTime dateOfBirth,
        DateTime today,
        int minimumAge = 13)
    {
        if (!IsValidDateOfBirth(dateOfBirth, today))
        {
            return false;
        }

        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }

        return age >= minimumAge;
    }

    public static bool IsValidGender(string? gender)
    {
        return !string.IsNullOrWhiteSpace(gender);
    }

    public static bool IsValidPassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            password.Length < 8)
        {
            return false;
        }

        return password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit);
    }

    public static bool PasswordsMatch(
        string? password,
        string? confirmPassword)
    {
        return !string.IsNullOrEmpty(password) &&
               password == confirmPassword;
    }
}