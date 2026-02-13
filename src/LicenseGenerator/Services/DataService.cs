using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace LicenseGenerator.Services;

public class DataService : IDataService
{
    private readonly string _dataDirectory;

    public DataService()
    {
        _dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LicenseGenerator");
        if (!Directory.Exists(_dataDirectory)) Directory.CreateDirectory(_dataDirectory);
    }

    public async Task ExportDataAsync(string destinationPath)
    {
        await Task.Run(() =>
        {
            // Create a temporary directory to prepare the backup
            string tempPath = Path.Combine(Path.GetTempPath(), "LicenseGeneratorBackup_" + Guid.NewGuid().ToString("N"));
            try
            {
                Directory.CreateDirectory(tempPath);

                // Copy Keys, Licenses and settings.json
                CopyDirectory(Path.Combine(_dataDirectory, "Keys"), Path.Combine(tempPath, "Keys"));
                CopyDirectory(Path.Combine(_dataDirectory, "Licenses"), Path.Combine(tempPath, "Licenses"));
                
                string settingsFile = Path.Combine(_dataDirectory, "settings.json");
                if (File.Exists(settingsFile))
                {
                    File.Copy(settingsFile, Path.Combine(tempPath, "settings.json"));
                }

                // Delete destination if exists
                if (File.Exists(destinationPath)) File.Delete(destinationPath);

                // Zip the temp directory
                ZipFile.CreateFromDirectory(tempPath, destinationPath);
            }
            finally
            {
                if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
            }
        });
    }

    public async Task ImportDataAsync(string sourcePath)
    {
        await Task.Run(() =>
        {
            if (!File.Exists(sourcePath)) throw new FileNotFoundException("Backup file not found.");

            string tempPath = Path.Combine(Path.GetTempPath(), "LicenseGeneratorRestore_" + Guid.NewGuid().ToString("N"));
            try
            {
                ZipFile.ExtractToDirectory(sourcePath, tempPath);

                // Verify content (at least one of the expected folders/files should exist)
                bool hasKeys = Directory.Exists(Path.Combine(tempPath, "Keys"));
                bool hasLicenses = Directory.Exists(Path.Combine(tempPath, "Licenses"));
                bool hasSettings = File.Exists(Path.Combine(tempPath, "settings.json"));

                if (!hasKeys && !hasLicenses && !hasSettings)
                {
                    throw new InvalidDataException("The backup file does not contain valid LicenseGenerator data.");
                }

                // Replace data
                if (hasKeys)
                {
                    string targetKeys = Path.Combine(_dataDirectory, "Keys");
                    if (Directory.Exists(targetKeys)) Directory.Delete(targetKeys, true);
                    CopyDirectory(Path.Combine(tempPath, "Keys"), targetKeys);
                }

                if (hasLicenses)
                {
                    string targetLicenses = Path.Combine(_dataDirectory, "Licenses");
                    if (Directory.Exists(targetLicenses)) Directory.Delete(targetLicenses, true);
                    CopyDirectory(Path.Combine(tempPath, "Licenses"), targetLicenses);
                }

                if (hasSettings)
                {
                    File.Copy(Path.Combine(tempPath, "settings.json"), Path.Combine(_dataDirectory, "settings.json"), true);
                }
            }
            finally
            {
                if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
            }
        });
    }

    private void CopyDirectory(string sourceDir, string destinationDir)
    {
        if (!Directory.Exists(sourceDir)) return;

        Directory.CreateDirectory(destinationDir);

        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string dest = Path.Combine(destinationDir, Path.GetFileName(file));
            File.Copy(file, dest, true);
        }

        foreach (string folder in Directory.GetDirectories(sourceDir))
        {
            string dest = Path.Combine(destinationDir, Path.GetFileName(folder));
            CopyDirectory(folder, dest);
        }
    }
}
