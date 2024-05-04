using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DiscordChatAI.Models.GoogleGeminiPro;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace DiscordChatAI.Helpers;

public class GoogleGeminiProClient
{
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

    private static GoogleGeminiProClient? _instance;
    public string? ApiKey { get; set; }

    public static GoogleGeminiProClient Instance
    {
        get { return _instance ??= new GoogleGeminiProClient(); }
    }

    public async Task<string?> GenerateString(string message)
    {
        try
        {
            var requestModel = CreateRequestModel(message);

            var response = await BaseUrl
                .WithSettings(settings => settings.JsonSerializer = new DefaultJsonSerializer(new JsonSerializerOptions
                {
                    IncludeFields = true
                }))
                .SetQueryParams(new { key = ApiKey })
                .PostJsonAsync(requestModel)
                .ReceiveJson<ResponseModel>();

            if (response.Candidates == null || response.Candidates.Count < 1) return null;

            return response.Candidates.First().Content?.Parts?.First().Text;
        }
        catch (FlurlHttpException e)
        {
            Console.WriteLine(e);
            Console.WriteLine(e.Message);
            Console.WriteLine(e.Call.Response.ResponseMessage.ReasonPhrase);
            Console.WriteLine(await e.Call.Response.ResponseMessage.Content.ReadAsStringAsync());
            Console.WriteLine(e.Call.RequestBody);
        }

        return null;
    }

    private static RequestModel CreateRequestModel(string message)
    {
        var part = new Part
        {
            Text = message
        };

        var content = new Content
        {
            Parts = new List<Part> { part }
        };

        return new RequestModel
        {
            Contents = new List<Content> { content }
        };
    }
}