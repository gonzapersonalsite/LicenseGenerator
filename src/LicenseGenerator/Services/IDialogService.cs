using System.Threading.Tasks;

namespace LicenseGenerator.Services;

public interface IDialogService
{
    Task<string?> SaveFileAsync(string title, string defaultFileName, string[] extensions);
    Task<string?> OpenFileAsync(string title, string[] extensions);
    Task<bool> ShowConfirmationDialogAsync(string title, string message, string? confirmText = null, string? cancelText = null);
}
