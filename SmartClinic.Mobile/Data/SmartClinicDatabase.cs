using SQLite;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Data;

public class SmartClinicDatabase
{
    private readonly SQLiteAsyncConnection _database;

    public SmartClinicDatabase()
    {
        var databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "smartclinic.db3");

        _database = new SQLiteAsyncConnection(databasePath);
    }

    public async Task InitializeAsync()
    {
        // Create the local database tables if they do not already exist.
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
}