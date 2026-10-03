using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.Tests;

public class RegistrationRulesTests
{
    [Theory]
    [InlineData("Test", true)]
    [InlineData("Jo", true)]
    [InlineData("A", false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void IsValidName_ReturnsExpectedResult(
        string? name,
        bool expected)
    {
        Assert.Equal(expected, RegistrationRules.IsValidName(name));
    }

    [Theory]
    [InlineData("patient@example.com", true)]
    [InlineData("test.patient@smartclinic.local", true)]
    [InlineData("invalid-email", false)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void IsValidEmail_ReturnsExpectedResult(
        string? email,
        bool expected)
    {
        Assert.Equal(expected, RegistrationRules.IsValidEmail(email));
    }

    [Theory]
    [InlineData("0821234567", true)]
    [InlineData("1234567", true)]
    [InlineData("123456", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidPhoneNumber_ReturnsExpectedResult(
        string? phoneNumber,
        bool expected)
    {
        Assert.Equal(
            expected,
            RegistrationRules.IsValidPhoneNumber(phoneNumber));
    }

    [Fact]
    public void IsValidDateOfBirth_FutureDate_ReturnsFalse()
    {
        var today = new DateTime(2026, 10, 3);
        var futureDate = new DateTime(2026, 10, 4);

        Assert.False(
            RegistrationRules.IsValidDateOfBirth(futureDate, today));
    }

    [Fact]
    public void IsValidDateOfBirth_Today_ReturnsTrue()
    {
        var today = new DateTime(2026, 10, 3);

        Assert.True(
            RegistrationRules.IsValidDateOfBirth(today, today));
    }

    [Theory]
    [InlineData(2000, 10, 3, true)]
    [InlineData(2013, 10, 3, true)]
    [InlineData(2013, 10, 4, false)]
    [InlineData(2014, 1, 1, false)]
    public void MeetsMinimumAge_ReturnsExpectedResult(
        int year,
        int month,
        int day,
        bool expected)
    {
        var today = new DateTime(2026, 10, 3);
        var dateOfBirth = new DateTime(year, month, day);

        Assert.Equal(
            expected,
            RegistrationRules.MeetsMinimumAge(dateOfBirth, today));
    }

    [Theory]
    [InlineData("Male", true)]
    [InlineData("Female", true)]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData(null, false)]
    public void IsValidGender_ReturnsExpectedResult(
        string? gender,
        bool expected)
    {
        Assert.Equal(
            expected,
            RegistrationRules.IsValidGender(gender));
    }

    [Theory]
    [InlineData("Password1", true)]
    [InlineData("Secure123", true)]
    [InlineData("password1", false)]
    [InlineData("PASSWORD1", false)]
    [InlineData("Password", false)]
    [InlineData("Pass1", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidPassword_ReturnsExpectedResult(
        string? password,
        bool expected)
    {
        Assert.Equal(
            expected,
            RegistrationRules.IsValidPassword(password));
    }

    [Theory]
    [InlineData("Password1", "Password1", true)]
    [InlineData("Password1", "Password2", false)]
    [InlineData("", "", false)]
    [InlineData(null, null, false)]
    public void PasswordsMatch_ReturnsExpectedResult(
        string? password,
        string? confirmPassword,
        bool expected)
    {
        Assert.Equal(
            expected,
            RegistrationRules.PasswordsMatch(
                password,
                confirmPassword));
    }
}