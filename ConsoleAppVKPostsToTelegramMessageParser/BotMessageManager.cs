using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleAppVKPostsToTelegramMessageParser
{
    internal class BotMessageManager
    {
        public const string scissorsSticker = "CAACAgIAAxkBAAEFS4Ji0ulnUrikmCmJLXSTlMCxVP3YbgAC3xkAAnXPkUr_T7SB_z35nikE";
        public const string paperSticker = "CAACAgIAAxkBAAEFS4Bi0ulYjSp1pVuDS4aowSF8_MN4ogACwRoAAtXamUol2bjy5-0MnikE";
        public const string rockSticker = "CAACAgIAAxkBAAEFS3xi0uiGYLxA_16yGdYaCQKgtMv5_wAC8h4AAq29mUqS1H79EtNA1SkE";

        private bool _isInitialize = false;

        public bool IsInitialize { get { return _isInitialize; } }

        struct MessageSender
        {
            public ITelegramBotClient botClient;
            public Update update; // delete?
            public CancellationToken cancellationToken;
        }

        private static MessageSender _sender = new MessageSender();

        public BotMessageManager() { _isInitialize = false; }

        public BotMessageManager(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            _sender.botClient = botClient;
            _sender.update = update;
            _sender.cancellationToken = cancellationToken;
            _isInitialize = true;
        }

        public async static Task SendMessageWithOptions(long chatId, string sendText)
        {
            Console.WriteLine("message sent");
            Message sentMessage = await _sender.botClient.SendTextMessageAsync(
            chatId: chatId,
            text: sendText,
            cancellationToken: _sender.cancellationToken);
        }

        public async static Task SendMessageWithOptions(long chatId, string sendText, InlineKeyboardMarkup inlineKeyboard)
        {
            Console.WriteLine("message sent");
            Message sentMessage = await _sender.botClient.SendTextMessageAsync(
            chatId: chatId,
            text: sendText,
            replyMarkup: inlineKeyboard,
            cancellationToken: _sender.cancellationToken);
        }

        public async static Task SendStickerWithOptions(long chatId, string stickerId)
        {
            Message message1 = await _sender.botClient.SendStickerAsync(
            chatId: chatId,
            sticker: stickerId,
            cancellationToken: _sender.cancellationToken);
        }
    }
}
