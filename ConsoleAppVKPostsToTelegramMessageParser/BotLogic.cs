using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleAppVKPostsToTelegramMessageParser
{
    internal class BotLogic
    {
        enum Action
        {
            None,
            ChangeSite,
            ChoosePostNum
        }

        const string emptyMessage = "empty message";
        const string undefinedMessage = "Извините, но команда не распознанна";
        const string startMessage = "Бот пересылает посты из вк в телеграмм";
        
        public const string emptyUrlMessage = "Для начала работы нужно установить ссылку на группу /set_site";
        public const string undefinedUrlMessage = "Ссылка не действительна. Проверьте правильность написания и введите заново /set_site";
        public const string noPostsMessage = "Не найдено постов по ссылке /set_site";
        public const string postNotFoundMessage = "Пост не найден";

        private const string urlChangeMessage = "Введите ссылка на группу";
        private const string succesUrlChangeMessage = "Ссылка на группу установлена";
        private const string SiteHasPinnedPostMessage = "У группы есть закрепленное сообщение?";
        private const string postChooseMessage = "Введите номер поста";
        private const string yesMessage = "Yes";
        private const string noMessage = "No";

        private Action _action = Action.None;

        public async void RecieveTextMessage(Message message)
        {
            string messageText = "" + message.Text;

            Console.WriteLine($"Received a '{messageText}' message in chat {message.Chat.Id}.");

            switch (messageText.ToLower())
            {
                case "/start":
                    {
                        StartMessage(message.Chat.Id);
                    }
                    break;
                case "/take_bot_info":
                    {
                        StartMessage(message.Chat.Id);
                    }
                    break;
                case "/set_site":
                    {
                        _action = Action.ChangeSite;

                        await BotMessageManager.SendMessageWithOptions(message.Chat.Id, urlChangeMessage);
                    }
                    break;
                case "/take_last_post":
                    {
                        await SendPost(message.Chat.Id, 0);
                    }
                    break;
                case "/take_post_by_num":
                    {
                        _action = Action.ChoosePostNum;

                        await BotMessageManager.SendMessageWithOptions(message.Chat.Id, postChooseMessage);
                    }
                    break;
                default:
                    {
                        if (_action != Action.None)
                        {
                            if (_action == Action.ChangeSite)
                            {
                                _action = Action.None;
                                BotHandlers.Parser.SetUrl(messageText);

                                InlineKeyboardMarkup inlineKeyboard = new(new[]
                                {
                                    new []
                                    {
                                        InlineKeyboardButton.WithCallbackData(text: "Да", callbackData: yesMessage),
                                        InlineKeyboardButton.WithCallbackData(text: "Нет", callbackData: noMessage)
                                    },
                                });

                                await BotMessageManager.SendMessageWithOptions(message.Chat.Id, SiteHasPinnedPostMessage, inlineKeyboard);
                            } else if (_action == Action.ChoosePostNum)
                            {
                                int num;
                                if (int.TryParse(messageText, out num))
                                {
                                    await SendPost(message.Chat.Id, num);
                                    _action = Action.None;
                                }
                                else
                                {
                                    await BotMessageManager.SendMessageWithOptions(message.Chat.Id, undefinedMessage);
                                }
                            }
                        }
                        else
                        {
                            await BotMessageManager.SendMessageWithOptions(message.Chat.Id, undefinedMessage);
                        }
                    }
                    break;
            }
        }

        public async void RecieveCallbackDataMessage(Message message, string data)
        {
            switch (data)
            {
                case yesMessage:
                    {
                        await SetPinnedPostState(message.Chat.Id, true);
                    }
                    break;
                case noMessage:
                    {
                        await SetPinnedPostState(message.Chat.Id, false);
                    }
                    break;
                default:
                    {
                        throw new Exception("undefined callbackData");
                    }
            }
        }

        private string Mes(params string[] str)
        {
            string newStr = str[0];

            for (int i = 1; i < str.Length; i++)
            {
                newStr += "\n" + str[i];
            }

            return newStr;
        }

        private async static void StartMessage(long chatId)
        {
            await BotMessageManager.SendMessageWithOptions(chatId, startMessage);
            await BotMessageManager.SendMessageWithOptions(chatId, emptyUrlMessage);
        }

        private async Task SetPinnedPostState(long chatId, bool hasPinnedPost)
        {
            BotHandlers.Parser.SetPinnedPostState(false);

            await BotMessageManager.SendMessageWithOptions(chatId, succesUrlChangeMessage);
        }

        private async Task SendPost(long chatId, int post)
        {
            string mes = BotHandlers.Parser.GetPost(post);

            await BotMessageManager.SendMessageWithOptions(chatId, mes);
        }
    }
}
