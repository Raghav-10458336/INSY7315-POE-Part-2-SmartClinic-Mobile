using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.DTOs;

namespace SmartClinic.Mobile.Services;

public interface IAppointmentService
{
    // Retrieves all appointments belonging to the authenticated patient.
    Task<List<Appointment>> GetAppointmentsAsync();

    // Retrieves the patient's next upcoming appointment.
    Task<Appointment?> GetUpcomingAppointmentAsync();

    // Creates a new appointment for the authenticated patient.
    Task<Appointment?> CreateAppointmentAsync(CreateAppointmentRequest request);

    // Cancels an appointment belonging to the authenticated patient.
    Task<bool> CancelAppointmentAsync(int appointmentId);

    // Reschedules an existing appointment to a new available time.
    Task<Appointment?> RescheduleAppointmentAsync(
        int appointmentId,
        RescheduleAppointmentRequest request);
}