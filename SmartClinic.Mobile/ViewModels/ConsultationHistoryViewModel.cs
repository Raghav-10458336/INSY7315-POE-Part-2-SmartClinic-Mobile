using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class ConsultationHistoryViewModel : BaseViewModel
{
    private readonly IClinicalService _clinicalService;

    public ObservableCollection<Consultation> Consultations { get; } = [];

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private bool hasConsultations;

    [ObservableProperty]
    private bool hasNoConsultations = true;

    public ConsultationHistoryViewModel(
        IClinicalService clinicalService)
    {
        _clinicalService = clinicalService;
        Title = "Consultation History";
    }

    public async Task LoadConsultationsAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            Consultations.Clear();

            // Retrieve the patient's completed consultation records.
            var consultations =
                await _clinicalService.GetConsultationsAsync();

            foreach (var consultation in consultations
                .OrderByDescending(c => c.ConsultationDate))
            {
                Consultations.Add(consultation);
            }

            UpdateConsultationState();
        }
        catch (Exception)
        {
            ShowError(
                "Unable to load your consultation history.");

            HasConsultations = false;
            HasNoConsultations = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateConsultationState()
    {
        HasConsultations = Consultations.Count > 0;
        HasNoConsultations = !HasConsultations;
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