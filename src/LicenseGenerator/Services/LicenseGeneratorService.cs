using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LicenseGenerator.Models;

namespace LicenseGenerator.Services;

public class LicenseGeneratorService : ILicenseGeneratorService
{
    private readonly string _keysDirectory;
    private readonly string _licensesDirectory;

    public LicenseGeneratorService()
    {
        string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LicenseGenerator");
        _keysDirectory = Path.Combine(baseDir, "Keys");
        _licensesDirectory = Path.Combine(baseDir, "Licenses");

        if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);
        if (!Directory.Exists(_keysDirectory)) Directory.CreateDirectory(_keysDirectory);
        if (!Directory.Exists(_licensesDirectory)) Directory.CreateDirectory(_licensesDirectory);
    }

    public IEnumerable<string> GetAvailableApps()
    {
        if (!Directory.Exists(_keysDirectory)) return Enumerable.Empty<string>();
        
        return Directory.GetDirectories(_keysDirectory)
                        .Select(Path.GetFileName)
                        .Where(f => f != null)
                        .Cast<string>();
    }

    public bool CreateApp(string appId)
    {
        try
        {
            string appDir = Path.Combine(_keysDirectory, appId);
            if (Directory.Exists(appDir)) return false;

            Directory.CreateDirectory(appDir);

            using var rsa = RSA.Create(2048);
            
            string privateKey = rsa.ExportPkcs8PrivateKeyPem();
            string publicKey = rsa.ExportSubjectPublicKeyInfoPem();

            File.WriteAllText(Path.Combine(appDir, "private.pem"), privateKey);
            File.WriteAllText(Path.Combine(appDir, "public.pem"), publicKey);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool DeleteApp(string appId)
    {
        try
        {
            string appDir = Path.Combine(_keysDirectory, appId);
            if (Directory.Exists(appDir))
            {
                Directory.Delete(appDir, true);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public string GenerateLicense(string appId, string registrationName, string hardwareId, DateTime? expirationDate)
    {
        string privateKeyPath = Path.Combine(_keysDirectory, appId, "private.pem");
        if (!File.Exists(privateKeyPath))
            throw new Exception("Private key not found for this app.");

        string privateKey = File.ReadAllText(privateKeyPath);

        var license = new LicenseData
        {
            AppId = appId,
            RegistrationName = registrationName,
            HardwareId = hardwareId,
            ExpirationDate = expirationDate
        };

        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey);

        byte[] dataToSign = Encoding.UTF8.GetBytes(license.GetDataToSign());
        byte[] signature = rsa.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        license.Signature = Convert.ToBase64String(signature);

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = null
        };
        string json = JsonSerializer.Serialize(license, jsonOptions);
        
        // Save to history
        string historyFileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{appId}.json";
        File.WriteAllText(Path.Combine(_licensesDirectory, historyFileName), json);

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    public IEnumerable<LicenseData> GetLicenseHistory()
    {
        if (!Directory.Exists(_licensesDirectory)) return Enumerable.Empty<LicenseData>();

        var history = new List<LicenseData>();
        foreach (var file in Directory.GetFiles(_licensesDirectory, "*.json").OrderByDescending(f => f))
        {
            try
            {
                var json = File.ReadAllText(file);
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
                var license = JsonSerializer.Deserialize<LicenseData>(json, jsonOptions);
                if (license != null && 
                    !string.IsNullOrWhiteSpace(license.AppId) && 
                    !string.IsNullOrWhiteSpace(license.Signature))
                {
                    license.FileName = Path.GetFileName(file);
                    history.Add(license);
                }
            }
            catch { /* Skip malformed history files */ }
        }
        return history;
    }

    public bool DeleteLicense(string fileName)
    {
        try
        {
            string filePath = Path.Combine(_licensesDirectory, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public int GetLicenseCountForApp(string appId)
    {
        if (!Directory.Exists(_licensesDirectory)) return 0;

        // Match files like {date}_{appId}.json
        string searchPattern = $"*_{appId}.json";
        return Directory.GetFiles(_licensesDirectory, searchPattern).Length;
    }

    public string GetAppKeysDirectory(string appId)
    {
        return Path.Combine(_keysDirectory, appId);
    }

    public (int AppsCount, int LicensesCount) GetStats()
    {
        int apps = GetAvailableApps().Count();
        int licenses = 0;
        if (Directory.Exists(_licensesDirectory))
        {
            licenses = Directory.GetFiles(_licensesDirectory, "*.json").Length;
        }
        return (apps, licenses);
    }
}
