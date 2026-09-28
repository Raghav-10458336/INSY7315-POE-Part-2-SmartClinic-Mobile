using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public interface IAppointmentService
{
    // Retrieves all appointments belonging to the authenticated patient.
    Task<List<Appointment>> GetAppointmentsAsync();

    // Retrieves the patient's next upcoming appointment.
    Task<Appointment?> GetUpcomingAppointmentAsync();
}