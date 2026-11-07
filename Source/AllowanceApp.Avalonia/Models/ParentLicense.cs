using System.Text.Json.Serialization;

namespace AllowanceApp.Avalonia.Models
{
    public class ParentLicense
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public int Id { get; set; } = int.MinValue;

        [JsonPropertyName("secret_message")]
        public string SecretMessage { get; set; } = string.Empty;
    }
}
