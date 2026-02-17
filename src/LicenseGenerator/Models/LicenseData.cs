using System;
using System.Text.Json.Serialization;

namespace LicenseGenerator.Models
{
    public class LicenseData
    {
        [JsonPropertyName("AppId")]
        public string AppId { get; set; } = string.Empty;
        
        [JsonPropertyName("RegistrationName")]
        public string RegistrationName { get; set; } = string.Empty;
        
        [JsonPropertyName("HardwareId")]
        public string HardwareId { get; set; } = string.Empty;
        
        [JsonPropertyName("ExpirationDate")]
        public DateTime? ExpirationDate { get; set; }
        
        [JsonPropertyName("Signature")]
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
