using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class CheckInQueuePage : ContentPage
{
    private readonly CheckInQueueViewModel _viewModel;

    public CheckInQueuePage(CheckInQueueViewModel viewModel)
    {
        InitializeComponent();

        // Connect the check-in page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public async Task InitialiseAsync(Appointment appointment)
    {
        // Load the selected appointment and any existing queue information.
        await _viewModel.InitialiseAsync(appointment);
    }

    private async void OnCheckInClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy)
        {
            return;
        }

        // Require confirmation before adding the patient to the clinic queue.
        var confirmed = await DisplayAlertAsync(
            "Check In",
            "Confirm that you have arrived at the clinic and want to check in for this appointment.",
            "Check In",
            "Not Yet");

        if (!confirmed)
        {
            return;
        }

        await _viewModel.CheckInCommand.ExecuteAsync(null);
    }
}