namespace SmartClinic.Mobile.Models;

public class DoctorAvailability
{
    public int Id { get; set; }

    public int DoctorId { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    // Indicates whether this slot can currently be selected for a booking.
    public bool IsAvailable { get; set; } = true;

    // Optional display information returned with availability data.
    public string DoctorName { get; set; } = string.Empty;

    public string Specialisation { get; set; } = string.Empty;
}