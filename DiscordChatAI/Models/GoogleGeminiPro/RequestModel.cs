using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DiscordChatAI.Models.GoogleGeminiPro;

public class RequestModel
{
    [JsonProperty("contents", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("contents")]
    public List<Content>? Contents { get; set; }
}