using SQLite;

namespace SmartClinic.Mobile.Models;

public class Notification
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Links the notification to the user who should receive it.
    [Indexed]
    public int UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    // Distinguishes reminders, confirmations and other notification types.
    [Indexed]
    public string Type { get; set; } = string.Empty;

    // Indexed so notifications can be displayed newest first.
    [Indexed]
    public DateTime CreatedAt { get; set; }

    // Tracks whether the patient has already viewed the notification.
    [Indexed]
    public bool IsRead { get; set; }

    // Optionally links the notification to a related appointment.
    [Indexed]
    public int? AppointmentId { get; set; }
}