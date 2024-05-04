using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscordChatAI.Helpers;

namespace DiscordChatAI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    private string? _discordToken;

    [ObservableProperty]
    private string? _googleApiKey;

    [ObservableProperty]
    private string? _info;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    private bool _isStarted;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartCommand))]
    private bool _isVerified;

    private bool CanStart => !string.IsNullOrEmpty(DiscordToken) && !IsStarted && IsVerified;

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task Start()
    {
        if (string.IsNullOrEmpty(DiscordToken))
            return;

        await DiscordBot.Instance.Start(DiscordToken);

        IsStarted = true;
    }

    [RelayCommand]
    private async Task Verify()
    {
        if (GoogleApiKey == null)
            return;
        GoogleGeminiProClient.Instance.ApiKey = GoogleApiKey;
        var response = await GoogleGeminiProClient.Instance.GenerateString("Hello");

        IsVerified = response != null && !string.IsNullOrWhiteSpace(response);
        if (IsVerified)
        {
            Info = "Google Gemini Pro API key verified successfully";
            return;
        }

        Info = "Google Gemini Pro API key is not verified";
    }
}