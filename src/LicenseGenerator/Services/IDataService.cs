using System.Threading.Tasks;

namespace LicenseGenerator.Services;

public interface IDataService
{
    Task ExportDataAsync(string destinationPath);
    Task ImportDataAsync(string sourcePath);
}
