using System;
using System.Collections.Generic;
using LicenseGenerator.Models;

namespace LicenseGenerator.Services;

public interface ILicenseGeneratorService
{
    IEnumerable<string> GetAvailableApps();
    bool CreateApp(string appId);
    bool DeleteApp(string appId);
    string GenerateLicense(string appId, string registrationName, string hardwareId, DateTime? expirationDate);
    IEnumerable<LicenseData> GetLicenseHistory();
    bool DeleteLicense(string fileName);
    int GetLicenseCountForApp(string appId);
    string GetAppKeysDirectory(string appId);
    (int AppsCount, int LicensesCount) GetStats();
}
