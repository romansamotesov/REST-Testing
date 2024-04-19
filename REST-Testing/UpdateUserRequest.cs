using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
