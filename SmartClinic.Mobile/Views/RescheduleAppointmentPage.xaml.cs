using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class RescheduleAppointmentPage : ContentPage
{
    private readonly RescheduleAppointmentViewModel _viewModel;

    public RescheduleAppointmentPage(
        RescheduleAppointmentViewModel viewModel)
    {
        InitializeComponent();

        // Connect the reschedule page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Return to the appointments list after a successful reschedule.
        _viewModel.RescheduleCompleted += OnRescheduleCompleted;
    }

    public async Task InitialiseAsync(Appointment appointment)
    {
        // Load the selected appointment and its available replacement times.
        await _viewModel.InitialiseAsync(appointment);
    }

    private async void OnRescheduleCompleted(object? sender, EventArgs e)
    {
        // Return to My Appointments, which refreshes when it becomes visible.
        await Navigation.PopAsync();
    }
}