using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MovieSearchBot;

public class Handler(IServiceProvider serviceProvider)
{
    public Task HandlePollingError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        string errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
    
    public async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cts)
    {
        if (update.Type != UpdateType.Message)
            return;
        
        var message = update.Message!;
        
        if (message.Type != MessageType.Text)
            return;
        
        if (message.Chat.Type != ChatType.Private)
            return;

        if (message.Text == "/start")
        {
            await botClient.SendTextMessageAsync(
                chatId: update.Message!.Chat.Id,
                text: "Привет, я бот для поиска фильмов! Напиши мне в личку название фильма и я попытаюсь его найти",
                cancellationToken: cts
            );
            return;
        }
        
        var movies = await serviceProvider.GetRequiredService<ApiWrapper>().FindMovie(message.Text!);
        var movie = movies!.FirstOrDefault();
        if (movie == null)
            return;
        await botClient.SendPhotoAsync(
            chatId: message.Chat.Id,
            cancellationToken: cts,
            replyToMessageId: message.MessageId,
            caption: $"<b>{movie.Name!}</b>" + "\n\n" + movie.Description,
            photo: new InputFileUrl(movie.Poster!.Url!),
            parseMode: ParseMode.Html,
            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Смотреть", $"https://www.kinopoisk.ru/film/{movie.Id}")));
    }
    
    // Add inline query - TO BE DONE
}