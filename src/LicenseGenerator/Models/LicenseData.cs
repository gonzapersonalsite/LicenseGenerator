using System;
using System.Text.Json.Serialization;

namespace LicenseGenerator.Models
{
    public class LicenseData
    {
        public string AppId { get; set; } = string.Empty;
        public string RegistrationName { get; set; } = string.Empty;
        public string HardwareId { get; set; } = string.Empty;
        public DateTime? ExpirationDate { get; set; }
        public string Signature { get; set; } = string.Empty;
        
        [JsonIgnore]
        public string? FileName { get; set; } // For deletion tracking

        public string GetDataToSign()
        {
            var dateStr = ExpirationDate?.ToString("yyyy-MM-dd") ?? "NEVER";
            return $"{AppId}|{RegistrationName}|{HardwareId}|{dateStr}";
        }
    }
}
