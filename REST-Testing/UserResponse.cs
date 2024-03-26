using System.Text.Json.Serialization;

namespace REST_Testing
{
    public record class UserResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("age")]
        public int Age { get; set; }
        [JsonPropertyName("sex")]
        public string Sex { get; set; }
        [JsonPropertyName("zipCode")]
        public string ZipCode { get; set; }
    }
}
