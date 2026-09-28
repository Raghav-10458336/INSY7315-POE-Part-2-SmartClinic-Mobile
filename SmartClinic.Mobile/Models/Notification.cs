namespace SmartClinic.Mobile.Models;

public class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    // Helps the app distinguish reminders, confirmations and other alerts.
    public string Type { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }

    // Allows a notification to reference a related appointment when required.
    public int? AppointmentId { get; set; }
}