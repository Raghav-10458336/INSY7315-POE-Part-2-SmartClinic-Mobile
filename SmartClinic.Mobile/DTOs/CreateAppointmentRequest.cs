namespace SmartClinic.Mobile.DTOs;

public class CreateAppointmentRequest
{
    public int DoctorId { get; set; }

    public DateTime AppointmentDateTime { get; set; }

    public string ReasonForVisit { get; set; } = string.Empty;
}