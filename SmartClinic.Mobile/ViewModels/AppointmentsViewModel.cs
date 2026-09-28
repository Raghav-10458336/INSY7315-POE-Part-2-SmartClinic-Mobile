using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class AppointmentsViewModel : BaseViewModel
{
    private readonly IAppointmentService _appointmentService;

    public ObservableCollection<Appointment> Appointments { get; } = [];

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private bool hasAppointments;

    [ObservableProperty]
    private bool hasNoAppointments = true;

    public AppointmentsViewModel(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
        Title = "Appointments";
    }

    public async Task LoadAppointmentsAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            Appointments.Clear();

            // Retrieve appointments belonging to the authenticated patient.
            var appointments = await _appointmentService.GetAppointmentsAsync();

            foreach (var appointment in appointments
                         .OrderBy(a => a.AppointmentDateTime))
            {
                Appointments.Add(appointment);
            }

            HasAppointments = Appointments.Count > 0;
            HasNoAppointments = !HasAppointments;
        }
        catch (Exception)
        {
            ShowError("Unable to load your appointments.");

            HasAppointments = false;
            HasNoAppointments = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }
}