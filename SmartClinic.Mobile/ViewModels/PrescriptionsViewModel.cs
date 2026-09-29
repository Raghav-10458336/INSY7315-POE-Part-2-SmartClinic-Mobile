using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class PrescriptionsViewModel : BaseViewModel
{
    private readonly IClinicalService _clinicalService;

    public ObservableCollection<Prescription> Prescriptions { get; } = [];

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private bool hasPrescriptions;

    [ObservableProperty]
    private bool hasNoPrescriptions = true;

    public PrescriptionsViewModel(
        IClinicalService clinicalService)
    {
        _clinicalService = clinicalService;
        Title = "My Prescriptions";
    }

    public async Task LoadPrescriptionsAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            Prescriptions.Clear();

            // Retrieve all prescriptions belonging to the authenticated patient.
            var prescriptions =
                await _clinicalService.GetPrescriptionsAsync();

            foreach (var prescription in prescriptions
                .OrderByDescending(p => p.IssuedDate))
            {
                Prescriptions.Add(prescription);
            }

            UpdatePrescriptionState();
        }
        catch (Exception)
        {
            ShowError(
                "Unable to load your prescriptions.");

            HasPrescriptions = false;
            HasNoPrescriptions = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdatePrescriptionState()
    {
        HasPrescriptions = Prescriptions.Count > 0;
        HasNoPrescriptions = !HasPrescriptions;
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