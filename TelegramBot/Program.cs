using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TelegramBot;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;

public class Program
{
    // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
    private static ITelegramBotClient _botClient;

    // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
    private static ReceiverOptions _receiverOptions;

    // Это словарь, хранящий последнее действие каждого из пользователей
    private static Dictionary<long, string> _history;

    // 
    private static Functions _functions;

    private static async Task Main()
    {
        _botClient = new TelegramBotClient(ConfigurationManager.AppSettings["TelegramApiKey"]); 
        _receiverOptions = new ReceiverOptions 
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message, 
            },
            // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда бот был оффлайн
            // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
            ThrowPendingUpdates = true,
        };
        _history = new Dictionary<long, string>();
        _functions = new Functions();
        using var cts = new CancellationTokenSource();
        // UpdateHander - обработчик приходящих Update`ов
        // ErrorHandler - обработчик ошибок, связанных с Bot API
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); 
        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"{me.FirstName} запущен!");
        await Task.Delay(-1);
    }

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    var message = update.Message;
                    var user = message.From;
                    // Вывод в консоль принятых ботом сообщений
                    Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text} в чате {message.Chat.Id}");
                    if (message.Text != null)
                    {
                        var text = "";
                        var isCommand = true;
                        (text, isCommand) = await _functions.Command(message, _history);
                        _ = botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: text
                        );
                        if (isCommand) _history[user.Id] = message.Text.ToLower();
                    }
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"При обращении к Telegram API произошла ошибка:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => $"Ошибка при исполнении: {error}"
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}