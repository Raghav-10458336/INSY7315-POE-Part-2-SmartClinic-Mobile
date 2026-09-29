using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class ConsultationDetailsViewModel : BaseViewModel
{
    private readonly IClinicalService _clinicalService;

    public ObservableCollection<Prescription> Prescriptions { get; } = [];

    [ObservableProperty]
    private Consultation? consultation;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private bool hasPrescriptions;

    [ObservableProperty]
    private bool hasNoPrescriptions = true;

    public ConsultationDetailsViewModel(
        IClinicalService clinicalService)
    {
        _clinicalService = clinicalService;
        Title = "Consultation Details";
    }

    public async Task InitialiseAsync(Consultation selectedConsultation)
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            Consultation = selectedConsultation;
            Prescriptions.Clear();

            // Retrieve the complete consultation record from the API.
            var consultation =
                await _clinicalService.GetConsultationAsync(
                    selectedConsultation.Id);

            if (consultation is not null)
            {
                Consultation = consultation;
            }

            // Retrieve prescriptions issued during this consultation.
            var prescriptions =
                await _clinicalService.GetConsultationPrescriptionsAsync(
                    selectedConsultation.Id);

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
                "Unable to load the complete consultation record.");

            UpdatePrescriptionState();
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