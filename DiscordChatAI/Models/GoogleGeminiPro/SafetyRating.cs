using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DiscordChatAI.Models.GoogleGeminiPro;

public class SafetyRating
{
    [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("category")]
    public string? Category;

    [JsonProperty("probability", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("probability")]
    public string? Probability;
}