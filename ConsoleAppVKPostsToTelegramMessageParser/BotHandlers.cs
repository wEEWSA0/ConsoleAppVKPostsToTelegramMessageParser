using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace ConsoleAppVKPostsToTelegramMessageParser
{
    internal static class BotHandlers
    {
        private static BotLogic _logic = new BotLogic();
        private static BotMessageManager _sender = new BotMessageManager();
        private static VKParser _parser = new VKParser();
        private const string undefinedCommand = "undefined command";

        public static VKParser Parser { get { return _parser; } }

        public async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (!_sender.IsInitialize)
            {
                _sender = new BotMessageManager(botClient, update, cancellationToken);
            }

            if (update.CallbackQuery != null)
            {
                if (update.CallbackQuery.Data != null)
                {
                    _logic.RecieveCallbackDataMessage(update.CallbackQuery.Message, update.CallbackQuery.Data);

                    return;
                }
            }

            if (update.Message is not { } message) { return; }

            if (message.Text is not { } messageText) { return; }

            _logic.RecieveTextMessage(message);
        }

        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
