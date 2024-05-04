using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace DiscordChatAI.Models.GoogleGeminiPro;

public class ResponseModel
{
    [JsonProperty("candidates", NullValueHandling = NullValueHandling.Ignore)]
    [JsonPropertyName("candidates")]
    public List<Candidate>? Candidates;
}