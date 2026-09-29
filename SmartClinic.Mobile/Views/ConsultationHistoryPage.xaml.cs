using Microsoft.Extensions.DependencyInjection;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class ConsultationHistoryPage : ContentPage
{
    private readonly ConsultationHistoryViewModel _viewModel;

    public ConsultationHistoryPage(
        ConsultationHistoryViewModel viewModel)
    {
        InitializeComponent();

        // Connect the consultation history page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh the patient's consultation history when the page opens.
        await _viewModel.LoadConsultationsAsync();
    }

    private async void OnViewConsultationClicked(
        object? sender,
        EventArgs e)
    {
        if (sender is not Button button ||
            button.BindingContext is not Consultation consultation)
        {
            return;
        }

        // Resolve the consultation details page through dependency injection.
        var detailsPage = Handler?.MauiContext?.Services
            .GetService<ConsultationDetailsPage>();

        if (detailsPage is null)
        {
            return;
        }

        // Load the selected clinical record before opening the details page.
        await detailsPage.InitialiseAsync(consultation);

        await Navigation.PushAsync(detailsPage);
    }
}