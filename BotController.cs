using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Shelegram;

public sealed class BotController
{
    private readonly TelegramBotClient _bot;
    private readonly CancellationToken _cancellationToken;
    public BotController(string token, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _bot = new TelegramBotClient(token);
        _bot.OnMessage += OnMessage;
    }

    public async Task Test()
    {
        var me = await _bot.GetMe();
        Console.WriteLine($"Shelegram started with #{me.Id} / {me.FirstName}.");
    }

    async Task OnMessage(Message msg, UpdateType type)
    {
        if(msg.Text is null) return;    // we only handle Text messages here
        if(msg.Chat.Id != 1078791068) return;//we do not handle non-admin messages
        var text = msg.Text;
        if(text == "/start")
        {
            await SendAsync(msg.Chat, @"
Usage:
/test : test echo hello world
/cputemp : see cpu temperature
/restartnetwork : restart network manager
/restart : restart os
");
            return;
        }
        else if(text == "/test")
        {
            await RunSendAsync(msg.Chat, msg.Id, @"echo hello-world");
        }
        else if(text == "/cputemp")
        {
            await RunSendAsync(msg.Chat, msg.Id, @"sudo vcgencmd measure_temp");
        }
        else if(text == "/restartnetwork")
        {
            await RunSendAsync(msg.Chat, msg.Id, @"sudo service NetworkManager restart");
        }
        else if(text == "/restart")
        {
            await RunSendAsync(msg.Chat, msg.Id, @"shutdown -r now");
        }
        else
        {
            await RunSendAsync(msg.Chat, msg.Id, text);
        }
    }
    async Task SendAsync(ChatId chatId, string text)
    {
        await _bot.SendMessage(chatId, text, cancellationToken: _cancellationToken);
    }

    async Task RunSendAsync(ChatId chatId, int messageId, string text)
    {
        bool success;
        try
        {
            success = await HelperMethods.RunCommandWithBash(text, async (line) => await SendAsync(chatId, line), _cancellationToken);
        }
        catch(Exception e)
        {
            await SendAsync(chatId, e.Message);
            success = false;
        }
        try
        {
            var reaction = new ReactionTypeEmoji()
            {
                Emoji = success ? "👍" : "👎"
            };
            await _bot.SetMessageReaction(chatId, messageId, [reaction], cancellationToken: _cancellationToken);
        }
        catch
        {

        }
    }
}
