using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DiscordChatAI.Models.GoogleGeminiPro;

public class Content
{
    [JsonProperty("parts", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("parts")]
    public List<Part>? Parts;

    [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("role")]
    public string? Role;
}