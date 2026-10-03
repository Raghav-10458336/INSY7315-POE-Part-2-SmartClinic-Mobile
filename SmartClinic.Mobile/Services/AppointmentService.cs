using SmartClinic.Mobile.Data;
using SmartClinic.Mobile.DTOs;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public class AppointmentService : IAppointmentService
{
    private readonly SmartClinicDatabase _smartClinicDatabase;
    private readonly IAuthenticationService _authenticationService;

    public AppointmentService(
        SmartClinicDatabase smartClinicDatabase,
        IAuthenticationService authenticationService)
    {
        _smartClinicDatabase = smartClinicDatabase;
        _authenticationService = authenticationService;
    }

    public async Task<List<Appointment>> GetAppointmentsAsync()
    {
        try
        {
            var patient = await GetCurrentPatientAsync();

            if (patient is null)
            {
                return [];
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            // Return only appointments belonging to the authenticated patient.
            return await database.Table<Appointment>()
                .Where(appointment => appointment.PatientId == patient.Id)
                .OrderByDescending(appointment => appointment.AppointmentDateTime)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<Appointment?> GetUpcomingAppointmentAsync()
    {
        try
        {
            var patient = await GetCurrentPatientAsync();

            if (patient is null)
            {
                return null;
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();
            var now = DateTime.Now;

            // Find the patient's next active future appointment.
            return await database.Table<Appointment>()
                .Where(appointment =>
                    appointment.PatientId == patient.Id &&
                    appointment.AppointmentDateTime > now &&
                    appointment.Status == AppointmentStatus.Scheduled)
                .OrderBy(appointment => appointment.AppointmentDateTime)
                .FirstOrDefaultAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Appointment?> CreateAppointmentAsync(
        CreateAppointmentRequest request)
    {
        try
        {
            var patient = await GetCurrentPatientAsync();

            if (patient is null ||
                request.DoctorId <= 0 ||
                request.AppointmentDateTime <= DateTime.Now ||
                string.IsNullOrWhiteSpace(request.ReasonForVisit))
            {
                return null;
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            var doctor = await database.FindAsync<Doctor>(request.DoctorId);

            if (doctor is null || !doctor.IsAvailable)
            {
                return null;
            }

            // The requested time must match an available slot for this doctor.
            var slot = await database.Table<DoctorAvailability>()
                .Where(availability =>
                    availability.DoctorId == request.DoctorId &&
                    availability.StartDateTime == request.AppointmentDateTime &&
                    availability.IsAvailable)
                .FirstOrDefaultAsync();

            if (slot is null)
            {
                return null;
            }

            // Prevent another active appointment from using the same doctor and time.
            var doctorConflict = await database.Table<Appointment>()
                .Where(appointment =>
                    appointment.DoctorId == request.DoctorId &&
                    appointment.AppointmentDateTime == request.AppointmentDateTime &&
                    appointment.Status == AppointmentStatus.Scheduled)
                .FirstOrDefaultAsync();

            if (doctorConflict is not null)
            {
                return null;
            }

            // Prevent the patient from booking two appointments at the same time.
            var patientConflict = await database.Table<Appointment>()
                .Where(appointment =>
                    appointment.PatientId == patient.Id &&
                    appointment.AppointmentDateTime == request.AppointmentDateTime &&
                    appointment.Status == AppointmentStatus.Scheduled)
                .FirstOrDefaultAsync();

            if (patientConflict is not null)
            {
                return null;
            }

            var appointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                AppointmentDateTime = request.AppointmentDateTime,
                DurationMinutes = 30,
                Status = AppointmentStatus.Scheduled,
                ReasonForVisit = request.ReasonForVisit.Trim(),
                CreatedAt = DateTime.Now,
                DoctorName = doctor.FullName,
                DoctorSpecialisation = doctor.Specialisation,
                PatientName = patient.FullName
            };

            // Save the appointment before reserving its availability slot.
            await database.InsertAsync(appointment);

            slot.IsAvailable = false;

            var slotUpdated = await database.UpdateAsync(slot);

            if (slotUpdated <= 0)
            {
                // Remove the appointment if its slot could not be reserved.
                await database.DeleteAsync(appointment);
                return null;
            }

            return appointment;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> CancelAppointmentAsync(int appointmentId)
    {
        try
        {
            if (appointmentId <= 0)
            {
                return false;
            }

            var patient = await GetCurrentPatientAsync();

            if (patient is null)
            {
                return false;
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            var appointment = await database.Table<Appointment>()
                .Where(existing =>
                    existing.Id == appointmentId &&
                    existing.PatientId == patient.Id)
                .FirstOrDefaultAsync();

            if (appointment is null ||
                appointment.Status != AppointmentStatus.Scheduled ||
                appointment.AppointmentDateTime <= DateTime.Now)
            {
                return false;
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.Now;

            var updatedRows = await database.UpdateAsync(appointment);

            if (updatedRows <= 0)
            {
                return false;
            }

            // Release the original slot so another patient can book it.
            var slot = await FindAvailabilitySlotAsync(
                appointment.DoctorId,
                appointment.AppointmentDateTime);

            if (slot is not null)
            {
                slot.IsAvailable = true;
                await database.UpdateAsync(slot);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<Appointment?> RescheduleAppointmentAsync(
        int appointmentId,
        RescheduleAppointmentRequest request)
    {
        try
        {
            if (appointmentId <= 0 ||
                request.AppointmentDateTime <= DateTime.Now)
            {
                return null;
            }

            var patient = await GetCurrentPatientAsync();

            if (patient is null)
            {
                return null;
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            var appointment = await database.Table<Appointment>()
                .Where(existing =>
                    existing.Id == appointmentId &&
                    existing.PatientId == patient.Id)
                .FirstOrDefaultAsync();

            if (appointment is null ||
                appointment.Status != AppointmentStatus.Scheduled ||
                appointment.AppointmentDateTime <= DateTime.Now)
            {
                return null;
            }

            if (appointment.AppointmentDateTime == request.AppointmentDateTime)
            {
                return appointment;
            }

            // The new time must be a free slot belonging to the same doctor.
            var newSlot = await database.Table<DoctorAvailability>()
                .Where(slot =>
                    slot.DoctorId == appointment.DoctorId &&
                    slot.StartDateTime == request.AppointmentDateTime &&
                    slot.IsAvailable)
                .FirstOrDefaultAsync();

            if (newSlot is null)
            {
                return null;
            }

            var doctorConflict = await database.Table<Appointment>()
                .Where(existing =>
                    existing.Id != appointment.Id &&
                    existing.DoctorId == appointment.DoctorId &&
                    existing.AppointmentDateTime == request.AppointmentDateTime &&
                    existing.Status == AppointmentStatus.Scheduled)
                .FirstOrDefaultAsync();

            if (doctorConflict is not null)
            {
                return null;
            }

            var patientConflict = await database.Table<Appointment>()
                .Where(existing =>
                    existing.Id != appointment.Id &&
                    existing.PatientId == patient.Id &&
                    existing.AppointmentDateTime == request.AppointmentDateTime &&
                    existing.Status == AppointmentStatus.Scheduled)
                .FirstOrDefaultAsync();

            if (patientConflict is not null)
            {
                return null;
            }

            var previousDateTime = appointment.AppointmentDateTime;

            // Reserve the new slot before releasing the patient's old slot.
            newSlot.IsAvailable = false;

            var newSlotUpdated = await database.UpdateAsync(newSlot);

            if (newSlotUpdated <= 0)
            {
                return null;
            }

            appointment.AppointmentDateTime = request.AppointmentDateTime;
            appointment.UpdatedAt = DateTime.Now;

            var appointmentUpdated = await database.UpdateAsync(appointment);

            if (appointmentUpdated <= 0)
            {
                // Restore the new slot if the appointment update fails.
                newSlot.IsAvailable = true;
                await database.UpdateAsync(newSlot);

                return null;
            }

            // Release the old appointment slot after rescheduling succeeds.
            var previousSlot = await FindAvailabilitySlotAsync(
                appointment.DoctorId,
                previousDateTime);

            if (previousSlot is not null)
            {
                previousSlot.IsAvailable = true;
                await database.UpdateAsync(previousSlot);
            }

            return appointment;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<Patient?> GetCurrentPatientAsync()
    {
        var authenticationState =
            await _authenticationService.GetAuthenticationStateAsync();

        if (!authenticationState.IsAuthenticated ||
            authenticationState.UserId is null)
        {
            return null;
        }

        var database = await _smartClinicDatabase.GetConnectionAsync();
        var userId = authenticationState.UserId.Value;

        return await database.Table<Patient>()
            .Where(patient => patient.UserId == userId)
            .FirstOrDefaultAsync();
    }

    private async Task<DoctorAvailability?> FindAvailabilitySlotAsync(
        int doctorId,
        DateTime appointmentDateTime)
    {
        var database = await _smartClinicDatabase.GetConnectionAsync();

        return await database.Table<DoctorAvailability>()
            .Where(slot =>
                slot.DoctorId == doctorId &&
                slot.StartDateTime == appointmentDateTime)
            .FirstOrDefaultAsync();
    }
}