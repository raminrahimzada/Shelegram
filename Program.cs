namespace Shelegram;

internal static class Program
{
    static async Task Main(string[] args)
    {
        string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? "";
        if(string.IsNullOrWhiteSpace(token)) token = args.FirstOrDefault() ?? "";
        if(string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Provide TELEGRAM_BOT_TOKEN environment variable or pass it as first argument");
            Environment.Exit(1);
            return;
        }
        using var cts = new CancellationTokenSource();
        var bot = new BotController(token, cts.Token);
        await bot.Test();
        Console.CancelKeyPress += (a, b) =>
        {
            cts.Cancel();
        };
        await Task.Delay(Timeout.Infinite, cts.Token);
    }
}
