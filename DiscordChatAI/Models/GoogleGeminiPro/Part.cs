using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DiscordChatAI.Models.GoogleGeminiPro;

public class Part
{
    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("text")]
    public string? Text;
}