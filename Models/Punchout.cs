using System.Text.Json.Serialization;

namespace PunchoutUtils.Models
{
    public class Punchout
    {
        public Punchout() => CreatedAt = DateTime.Now;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; }

        [JsonPropertyName("entries")]
        public IList<PunchoutEntry> Entries { get; set; } = new List<PunchoutEntry>();
    }
}
