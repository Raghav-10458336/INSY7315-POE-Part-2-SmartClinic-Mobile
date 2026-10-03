using SmartClinic.Mobile.Data;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public class DoctorService : IDoctorService
{
    private readonly SmartClinicDatabase _smartClinicDatabase;
    private readonly IAuthenticationService _authenticationService;

    public DoctorService(
        SmartClinicDatabase smartClinicDatabase,
        IAuthenticationService authenticationService)
    {
        _smartClinicDatabase = smartClinicDatabase;
        _authenticationService = authenticationService;
    }

    public async Task<List<Doctor>> GetDoctorsAsync()
    {
        try
        {
            if (!await _authenticationService.IsAuthenticatedAsync())
            {
                return [];
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            // Only doctors currently accepting appointments are shown.
            return await database.Table<Doctor>()
                .Where(doctor => doctor.IsAvailable)
                .OrderBy(doctor => doctor.LastName)
                .ThenBy(doctor => doctor.FirstName)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<List<DoctorAvailability>> GetDoctorAvailabilityAsync(
        int doctorId)
    {
        try
        {
            if (!await _authenticationService.IsAuthenticatedAsync() ||
                doctorId <= 0)
            {
                return [];
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            var doctor = await database.FindAsync<Doctor>(doctorId);

            if (doctor is null || !doctor.IsAvailable)
            {
                return [];
            }

            var now = DateTime.Now;

            // Only future slots that are still available can be booked.
            var slots = await database.Table<DoctorAvailability>()
                .Where(slot =>
                    slot.DoctorId == doctorId &&
                    slot.IsAvailable &&
                    slot.StartDateTime > now)
                .OrderBy(slot => slot.StartDateTime)
                .ToListAsync();

            // Populate display information from the authoritative doctor record.
            foreach (var slot in slots)
            {
                slot.DoctorName = doctor.FullName;
                slot.Specialisation = doctor.Specialisation;
            }

            return slots;
        }
        catch (Exception)
        {
            return [];
        }
    }
}