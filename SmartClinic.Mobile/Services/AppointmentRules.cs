using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

// Contains pure appointment lifecycle rules that can be tested independently.
public static class AppointmentRules
{
    public static bool CanManage(AppointmentStatus status)
    {
        return status == AppointmentStatus.Scheduled ||
               status == AppointmentStatus.Confirmed;
    }

    public static bool CanAccessCheckIn(AppointmentStatus status)
    {
        return status == AppointmentStatus.Scheduled ||
               status == AppointmentStatus.CheckedIn ||
               status == AppointmentStatus.InQueue;
    }

    public static bool IsQueueActive(AppointmentStatus status)
    {
        return status == AppointmentStatus.CheckedIn ||
               status == AppointmentStatus.InQueue;
    }

    public static string GetQueueActionText(AppointmentStatus status)
    {
        return IsQueueActive(status)
            ? "View Queue Status"
            : "Check In & View Queue";
    }
}