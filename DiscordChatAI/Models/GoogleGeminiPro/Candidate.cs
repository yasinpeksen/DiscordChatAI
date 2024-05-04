using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DiscordChatAI.Models.GoogleGeminiPro;

public class Candidate
{
    [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("content")]
    public Content? Content;

    [JsonProperty("finishReason", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("finishReason")]
    public string? FinishReason;

    [JsonProperty("index", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("index")]
    public int Index;

    [JsonProperty("safetyRatings", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("safetyRatings")]
    public List<SafetyRating>? SafetyRatings;
}