using System.Collections.Generic;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Microsoft.Azure.Cosmos.Table;
using OpenAI_API.Chat;

namespace BotAI.Models
{
    public class BotUser : TableEntity
    {
        public string Username { get; set; }
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<ChatMessage> Messages { get; set; }
        public GenerationMode Mode { get; set; }

        public static BotUser GenerateBotUser(Update update)
        {
            return update.Type switch
            {
                UpdateType.CallbackQuery => new BotUser
                {
                    Username = update.CallbackQuery.From.Username,
                    Id = update.CallbackQuery.Message.Chat.Id,
                    FirstName = update.CallbackQuery.Message.From.FirstName,
                    LastName = update.CallbackQuery.Message.From.LastName,
                    PartitionKey = update.CallbackQuery.Message.Chat.Id.ToString(),
                    RowKey = update.CallbackQuery.Message.Chat.Id.ToString(),
                    Messages = new List<ChatMessage>()
                },
                UpdateType.Message => new BotUser
                {
                    Username = update.Message.Chat.Username,
                    Id = update.Message.Chat.Id,
                    FirstName = update.Message.Chat.FirstName,
                    LastName = update.Message.Chat.LastName,
                    PartitionKey = update.Message.Chat.Id.ToString(),
                    RowKey = update.Message.Chat.Id.ToString(),
                    Messages = new List<ChatMessage>()
                },
                _ => new BotUser()
            };
        }
    }
}
