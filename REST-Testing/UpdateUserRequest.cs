using System.Text.Json.Serialization;

namespace REST_Testing
{
    public class UpdateUserRequest
    {
        [JsonPropertyName("userNewValues")]
        public User UserNewValues { get; set; }

        [JsonPropertyName("userToChange")]
        public User UserToChange { get; set; }
    }
}
