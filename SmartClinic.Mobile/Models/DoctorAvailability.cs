using SQLite;

namespace SmartClinic.Mobile.Models;

public class DoctorAvailability
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Links the availability slot to the doctor who owns it.
    [Indexed]
    public int DoctorId { get; set; }

    // Defines the beginning and end of the appointment slot.
    [Indexed]
    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    // Booked or unavailable slots cannot be selected by patients.
    public bool IsAvailable { get; set; } = true;

    // Display values are populated from the related doctor when required.
    public string DoctorName { get; set; } = string.Empty;

    public string Specialisation { get; set; } = string.Empty;
}