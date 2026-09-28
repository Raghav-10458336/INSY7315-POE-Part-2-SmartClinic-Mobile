using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public interface IDoctorService
{
    // Retrieves doctors available for patient appointment booking.
    Task<List<Doctor>> GetDoctorsAsync();

    // Retrieves available appointment slots for a selected doctor.
    Task<List<DoctorAvailability>> GetDoctorAvailabilityAsync(int doctorId);
}