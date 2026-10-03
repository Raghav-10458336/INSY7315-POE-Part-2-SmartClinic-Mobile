using Microsoft.AspNetCore.Identity;
using SQLite;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Data;

public class SmartClinicDatabase
{
    private readonly SQLiteAsyncConnection _database;
    private readonly PasswordHasher<User> _passwordHasher = new();
    private bool _isInitialized;

    public SmartClinicDatabase()
    {
        var databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "smartclinic.db3");

        _database = new SQLiteAsyncConnection(
            databasePath,
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache);
    }

    public async Task InitializeAsync()
    {
        // Prevent unnecessary initialization work during the same app session.
        if (_isInitialized)
        {
            return;
        }

        await CreateTablesAsync();
        await SeedClinicDataAsync();

        _isInitialized = true;
    }

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        // Ensure the schema and baseline clinic data exist before use.
        await InitializeAsync();

        return _database;
    }

    private async Task CreateTablesAsync()
    {
        await _database.CreateTableAsync<User>();
        await _database.CreateTableAsync<Patient>();
        await _database.CreateTableAsync<Doctor>();
        await _database.CreateTableAsync<DoctorAvailability>();
        await _database.CreateTableAsync<Appointment>();
        await _database.CreateTableAsync<Consultation>();
        await _database.CreateTableAsync<Prescription>();
        await _database.CreateTableAsync<Notification>();
        await _database.CreateTableAsync<QueueStatus>();
    }

    private async Task SeedClinicDataAsync()
    {
        var doctorCount = await _database.Table<Doctor>().CountAsync();

        if (doctorCount == 0)
        {
            await SeedDoctorsAsync();
        }

        await EnsureFutureAvailabilityAsync();
        await SeedDemoPatientAsync();
    }

    private async Task SeedDoctorsAsync()
    {
        var doctors = new List<Doctor>
        {
            new()
            {
                FirstName = "Aisha",
                LastName = "Naidoo",
                Email = "aisha.naidoo@smartclinic.local",
                PhoneNumber = "0125550101",
                Specialisation = "General Practitioner",
                RegistrationNumber = "SC-GP-1001",
                Qualification = "MBChB",
                Biography =
                    "General practitioner focused on preventative care, routine consultations and ongoing patient wellness.",
                IsAvailable = true
            },
            new()
            {
                FirstName = "Daniel",
                LastName = "Mokoena",
                Email = "daniel.mokoena@smartclinic.local",
                PhoneNumber = "0125550102",
                Specialisation = "Family Medicine",
                RegistrationNumber = "SC-FM-1002",
                Qualification = "MBChB, MMed",
                Biography =
                    "Family medicine practitioner providing comprehensive primary healthcare for patients across different life stages.",
                IsAvailable = true
            },
            new()
            {
                FirstName = "Priya",
                LastName = "Patel",
                Email = "priya.patel@smartclinic.local",
                PhoneNumber = "0125550103",
                Specialisation = "General Practitioner",
                RegistrationNumber = "SC-GP-1003",
                Qualification = "MBChB",
                Biography =
                    "General practitioner with an interest in routine health screening, acute care and long-term patient support.",
                IsAvailable = true
            },
            new()
            {
                FirstName = "Michael",
                LastName = "Botha",
                Email = "michael.botha@smartclinic.local",
                PhoneNumber = "0125550104",
                Specialisation = "Internal Medicine",
                RegistrationNumber = "SC-IM-1004",
                Qualification = "MBChB, FCP(SA)",
                Biography =
                    "Physician focused on adult medicine, chronic disease management and coordinated long-term care.",
                IsAvailable = true
            }
        };

        await _database.InsertAllAsync(doctors);
    }

    private async Task EnsureFutureAvailabilityAsync()
    {
        var doctors = await _database.Table<Doctor>()
            .Where(doctor => doctor.IsAvailable)
            .ToListAsync();

        foreach (var doctor in doctors)
        {
            var futureSlotCount = await _database.Table<DoctorAvailability>()
                .Where(slot =>
                    slot.DoctorId == doctor.Id &&
                    slot.StartDateTime > DateTime.Now)
                .CountAsync();

            if (futureSlotCount > 0)
            {
                continue;
            }

            var slots = CreateAvailabilitySlots(doctor);

            if (slots.Count > 0)
            {
                await _database.InsertAllAsync(slots);
            }
        }
    }

    private static List<DoctorAvailability> CreateAvailabilitySlots(
        Doctor doctor)
    {
        var slots = new List<DoctorAvailability>();
        var date = DateTime.Today.AddDays(1);
        var workingDaysAdded = 0;

        // Generate availability across the next five working days.
        while (workingDaysAdded < 5)
        {
            if (date.DayOfWeek is not DayOfWeek.Saturday
                and not DayOfWeek.Sunday)
            {
                AddDailySlots(slots, doctor, date);
                workingDaysAdded++;
            }

            date = date.AddDays(1);
        }

        return slots;
    }

    private static void AddDailySlots(
        List<DoctorAvailability> slots,
        Doctor doctor,
        DateTime date)
    {
        var appointmentTimes = new[]
        {
            new TimeSpan(8, 30, 0),
            new TimeSpan(9, 30, 0),
            new TimeSpan(10, 30, 0),
            new TimeSpan(11, 30, 0),
            new TimeSpan(14, 0, 0),
            new TimeSpan(15, 0, 0)
        };

        foreach (var time in appointmentTimes)
        {
            var startDateTime = date.Date.Add(time);

            slots.Add(new DoctorAvailability
            {
                DoctorId = doctor.Id,
                StartDateTime = startDateTime,
                EndDateTime = startDateTime.AddMinutes(30),
                IsAvailable = true,
                DoctorName = doctor.FullName,
                Specialisation = doctor.Specialisation
            });
        }
    }

    private async Task SeedDemoPatientAsync()
    {
        const string demoEmail = "demo.patient@smartclinic.local";

        var existingUser = await _database.Table<User>()
            .Where(user => user.Email == demoEmail)
            .FirstOrDefaultAsync();

        // Never duplicate or overwrite the demo account.
        if (existingUser is not null)
        {
            return;
        }

        var user = new User
        {
            FirstName = "Thabo",
            LastName = "Mokoena",
            Email = demoEmail,
            Role = UserRole.Patient,
            IsActive = true
        };

        // Only the generated password hash is stored in SQLite.
        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            "Demo@12345");

        await _database.InsertAsync(user);

        var patient = new Patient
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = "0825550147",
            DateOfBirth = new DateTime(1998, 6, 14),
            Gender = "Male",
            Address = "Pretoria, Gauteng",
            EmergencyContactName = "Naledi Mokoena",
            EmergencyContactNumber = "0825550188",
            MedicalAidProvider = "Demo Health Medical Scheme",
            MedicalAidNumber = "DEM0012458"
        };

        await _database.InsertAsync(patient);

        await SeedDemoClinicalHistoryAsync(user, patient);
        await SeedDemoFutureAppointmentAsync(user, patient);
    }

    private async Task SeedDemoClinicalHistoryAsync(
        User user,
        Patient patient)
    {
        var doctors = await _database.Table<Doctor>()
            .OrderBy(doctor => doctor.Id)
            .ToListAsync();

        if (doctors.Count < 2)
        {
            return;
        }

        var firstDoctor = doctors[0];
        var secondDoctor = doctors[1];

        var firstAppointmentDate = DateTime.Today
            .AddDays(-45)
            .AddHours(9);

        var secondAppointmentDate = DateTime.Today
            .AddDays(-18)
            .AddHours(14);

        var firstAppointment = new Appointment
        {
            PatientId = patient.Id,
            DoctorId = firstDoctor.Id,
            AppointmentDateTime = firstAppointmentDate,
            DurationMinutes = 30,
            Status = AppointmentStatus.Completed,
            ReasonForVisit = "Persistent seasonal allergy symptoms",
            Notes = "Routine outpatient consultation completed.",
            CreatedAt = firstAppointmentDate.AddDays(-5),
            UpdatedAt = firstAppointmentDate,
            DoctorName = firstDoctor.FullName,
            DoctorSpecialisation = firstDoctor.Specialisation,
            PatientName = patient.FullName
        };

        var secondAppointment = new Appointment
        {
            PatientId = patient.Id,
            DoctorId = secondDoctor.Id,
            AppointmentDateTime = secondAppointmentDate,
            DurationMinutes = 30,
            Status = AppointmentStatus.Completed,
            ReasonForVisit = "Follow-up consultation",
            Notes = "Symptoms reviewed and follow-up advice provided.",
            CreatedAt = secondAppointmentDate.AddDays(-4),
            UpdatedAt = secondAppointmentDate,
            DoctorName = secondDoctor.FullName,
            DoctorSpecialisation = secondDoctor.Specialisation,
            PatientName = patient.FullName
        };

        await _database.InsertAsync(firstAppointment);
        await _database.InsertAsync(secondAppointment);

        var firstConsultation = new Consultation
        {
            AppointmentId = firstAppointment.Id,
            PatientId = patient.Id,
            DoctorId = firstDoctor.Id,
            ConsultationDate = firstAppointmentDate,
            DoctorName = firstDoctor.FullName,
            Diagnosis = "Seasonal allergic rhinitis",
            Treatment =
                "Symptom management and avoidance of known environmental triggers.",
            Summary =
                "Patient presented with recurring seasonal allergy symptoms. Clinical assessment was consistent with allergic rhinitis.",
            FollowUpInstructions =
                "Continue treatment as directed and return if symptoms worsen or do not improve.",
            FollowUpDate = firstAppointmentDate.AddDays(30)
        };

        var secondConsultation = new Consultation
        {
            AppointmentId = secondAppointment.Id,
            PatientId = patient.Id,
            DoctorId = secondDoctor.Id,
            ConsultationDate = secondAppointmentDate,
            DoctorName = secondDoctor.FullName,
            Diagnosis = "Follow-up review",
            Treatment =
                "Continue current symptom management plan.",
            Summary =
                "Follow-up consultation completed. Patient reported improvement since the previous visit.",
            FollowUpInstructions =
                "Continue monitoring symptoms and schedule another consultation if required."
        };

        await _database.InsertAsync(firstConsultation);
        await _database.InsertAsync(secondConsultation);

        var prescriptions = new List<Prescription>
        {
            new()
            {
                ConsultationId = firstConsultation.Id,
                PatientId = patient.Id,
                DoctorId = firstDoctor.Id,
                DoctorName = firstDoctor.FullName,
                MedicationName = "Cetirizine",
                Dosage = "10 mg",
                Frequency = "Once daily",
                Duration = "14 days",
                Instructions =
                    "Take as directed for allergy symptom relief.",
                IssuedDate = firstAppointmentDate,
                ExpiryDate = firstAppointmentDate.AddMonths(1)
            },
            new()
            {
                ConsultationId = firstConsultation.Id,
                PatientId = patient.Id,
                DoctorId = firstDoctor.Id,
                DoctorName = firstDoctor.FullName,
                MedicationName = "Saline Nasal Spray",
                Dosage = "As directed",
                Frequency = "Up to twice daily",
                Duration = "14 days",
                Instructions =
                    "Use as directed to assist with nasal symptoms.",
                IssuedDate = firstAppointmentDate,
                ExpiryDate = firstAppointmentDate.AddMonths(1)
            }
        };

        await _database.InsertAllAsync(prescriptions);

        var notifications = new List<Notification>
        {
            new()
            {
                UserId = user.Id,
                Title = "Consultation summary available",
                Message =
                    $"Your consultation summary with {secondDoctor.FullName} is available.",
                Type = "Consultation",
                CreatedAt = secondAppointmentDate.AddHours(1),
                IsRead = false,
                AppointmentId = secondAppointment.Id
            },
            new()
            {
                UserId = user.Id,
                Title = "Prescription available",
                Message =
                    "A digital prescription from your recent consultation is available.",
                Type = "Prescription",
                CreatedAt = firstAppointmentDate.AddHours(1),
                IsRead = true,
                AppointmentId = firstAppointment.Id
            },
            new()
            {
                UserId = user.Id,
                Title = "Follow-up reminder",
                Message =
                    "Remember to review your symptoms and arrange a follow-up consultation if required.",
                Type = "FollowUp",
                CreatedAt = DateTime.Today.AddDays(-2).AddHours(9),
                IsRead = false,
                AppointmentId = firstAppointment.Id
            }
        };

        await _database.InsertAllAsync(notifications);
    }

    private async Task SeedDemoFutureAppointmentAsync(
        User user,
        Patient patient)
    {
        var doctor = await _database.Table<Doctor>()
            .Where(existing => existing.IsAvailable)
            .OrderBy(existing => existing.Id)
            .FirstOrDefaultAsync();

        if (doctor is null)
        {
            return;
        }

        var slot = await _database.Table<DoctorAvailability>()
            .Where(availability =>
                availability.DoctorId == doctor.Id &&
                availability.IsAvailable &&
                availability.StartDateTime > DateTime.Now)
            .OrderBy(availability => availability.StartDateTime)
            .FirstOrDefaultAsync();

        if (slot is null)
        {
            return;
        }

        var appointment = new Appointment
        {
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            AppointmentDateTime = slot.StartDateTime,
            DurationMinutes = 30,
            Status = AppointmentStatus.Scheduled,
            ReasonForVisit = "Routine wellness consultation",
            CreatedAt = DateTime.Now,
            DoctorName = doctor.FullName,
            DoctorSpecialisation = doctor.Specialisation,
            PatientName = patient.FullName
        };

        await _database.InsertAsync(appointment);

        // Reserve the slot used by the demo patient's appointment.
        slot.IsAvailable = false;
        await _database.UpdateAsync(slot);

        var notification = new Notification
        {
            UserId = user.Id,
            Title = "Appointment confirmed",
            Message =
                $"Your appointment with {doctor.FullName} has been scheduled.",
            Type = "AppointmentConfirmation",
            CreatedAt = DateTime.Now,
            IsRead = false,
            AppointmentId = appointment.Id
        };

        await _database.InsertAsync(notification);
    }
}