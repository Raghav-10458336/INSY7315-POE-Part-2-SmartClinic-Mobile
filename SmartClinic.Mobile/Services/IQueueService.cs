using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public interface IQueueService
{
    // Checks the authenticated patient into a scheduled appointment.
    Task<QueueStatus?> CheckInAsync(int appointmentId);

    // Retrieves the patient's current queue information.
    Task<QueueStatus?> GetQueueStatusAsync(int appointmentId);
}
