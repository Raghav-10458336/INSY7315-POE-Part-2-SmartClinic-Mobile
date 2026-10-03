using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.Tests;

public class AppointmentLifecycleTests
{
    [Theory]
    [InlineData(AppointmentStatus.Scheduled, true)]
    [InlineData(AppointmentStatus.Confirmed, true)]
    [InlineData(AppointmentStatus.CheckedIn, false)]
    [InlineData(AppointmentStatus.InQueue, false)]
    [InlineData(AppointmentStatus.InConsultation, false)]
    [InlineData(AppointmentStatus.Completed, false)]
    [InlineData(AppointmentStatus.Cancelled, false)]
    [InlineData(AppointmentStatus.NoShow, false)]
    public void CanManage_ReturnsExpectedResult(
        AppointmentStatus status,
        bool expected)
    {
        Assert.Equal(expected, AppointmentRules.CanManage(status));
    }

    [Theory]
    [InlineData(AppointmentStatus.Scheduled, true)]
    [InlineData(AppointmentStatus.CheckedIn, true)]
    [InlineData(AppointmentStatus.InQueue, true)]
    [InlineData(AppointmentStatus.Completed, false)]
    [InlineData(AppointmentStatus.Cancelled, false)]
    public void CanAccessCheckIn_ReturnsExpectedResult(
        AppointmentStatus status,
        bool expected)
    {
        Assert.Equal(expected, AppointmentRules.CanAccessCheckIn(status));
    }

    [Theory]
    [InlineData(AppointmentStatus.CheckedIn, true)]
    [InlineData(AppointmentStatus.InQueue, true)]
    [InlineData(AppointmentStatus.Scheduled, false)]
    [InlineData(AppointmentStatus.Completed, false)]
    [InlineData(AppointmentStatus.Cancelled, false)]
    public void IsQueueActive_ReturnsExpectedResult(
        AppointmentStatus status,
        bool expected)
    {
        Assert.Equal(expected, AppointmentRules.IsQueueActive(status));
    }

    [Theory]
    [InlineData(AppointmentStatus.Scheduled, "Check In & View Queue")]
    [InlineData(AppointmentStatus.CheckedIn, "View Queue Status")]
    [InlineData(AppointmentStatus.InQueue, "View Queue Status")]
    public void GetQueueActionText_ReturnsExpectedText(
        AppointmentStatus status,
        string expected)
    {
        Assert.Equal(expected, AppointmentRules.GetQueueActionText(status));
    }
}