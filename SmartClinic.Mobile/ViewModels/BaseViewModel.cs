using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartClinic.Mobile.ViewModels;

/// <summary>
/// Base class for all view models in the Smart Clinic mobile application.
/// Provides common observable state used throughout the app.
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    /// <summary>
    /// Prevents actions from running while another operation is in progress.
    /// </summary>
    public bool IsNotBusy => !IsBusy;

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }
}
