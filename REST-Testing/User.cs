using System.Text.Json.Serialization;

namespace REST_Testing
{
    public class User
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("age")]
        public int Age { get; set; }
        [JsonPropertyName("sex")]
        public string Sex { get; set; }
        [JsonPropertyName("zipCode")]
        public string ZipCode { get; set; }

        public User(int age, string name, Enums.Sex sex, string zipCode)
        {
            Age = age;
            Name = name;
            Sex = sex.ToString();
            ZipCode = zipCode;
        }

        public User(string name, Enums.Sex sex)
        {
            Name = name;
            Sex = sex.ToString();
        }
    }
} 
