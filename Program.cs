using Microsoft.Extensions.DependencyInjection;
using MovieSearchBot;
using Telegram.Bot;

var apiKey = Environment.GetEnvironmentVariable("API_KEY");
var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");
if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(botToken))
{
    Console.WriteLine("enter your api key and bot token");
    Environment.Exit(1);
}

ServiceCollection services = new();
services.AddScoped<ApiWrapper>(_ => new ApiWrapper(apiKey));
IServiceProvider provider = services.BuildServiceProvider();

var bot = new TelegramBotClient(botToken);
var handler = new Handler(provider);

bot.StartReceiving(handler.UpdateHandler, handler.HandlePollingError);

await Task.Delay(-1);