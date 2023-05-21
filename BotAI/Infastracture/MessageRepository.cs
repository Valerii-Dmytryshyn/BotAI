using BotAI.Abstract;
using BotAI.Models;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using OpenAI_API.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotAI.Infastracture
{
    public class MessageRepository : IMessageRepository
    {
        private readonly StorageAccountProxy _storage;

        public MessageRepository(StorageAccountProxy storage)
        {
            _storage = storage;
        }

        public void CreateIfNotExist() =>
            _storage.GetCloudTable(Constants.MessagesTableName);

        public async Task AddMessage(ChatMessageRole role, string message, long userId, string username)
        {
            var messageTable = _storage.GetCloudTable(Constants.MessagesTableName);
            var chatMessageId = GetNextChatMessageId(messageTable);
            var messageModel = new ChatMessageModel
            {
                Role = role,
                Content = message,
                UserId = userId.ToString(),
                UserName = username,
                PartitionKey = chatMessageId.ToString(),
                RowKey = chatMessageId.ToString(),
            };

            await _storage.InsertEntity(messageTable, messageModel);
        }

        public List<ChatMessageModel> GetChatMessages(long userId)
        {
            var userTable = _storage.GetCloudTable(Constants.MessagesTableName);
            var query = _storage.GetAllQuery<ChatMessageModel>().Where(TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, userId.ToString()));

            return userTable.ExecuteQuery(query).ToList();
        }

        private Guid GetNextChatMessageId(CloudTable table)
        {
            Guid id = Guid.NewGuid();
            bool flag = true;
            while (flag)
            {
                id = Guid.NewGuid();
                var query = _storage.GetAllQuery<ChatMessageModel>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));
                var lastChatMessage = table.ExecuteQuery(query);

                if (!lastChatMessage.Any())
                    flag = false;
            }

            return id;
        }
    }
}
