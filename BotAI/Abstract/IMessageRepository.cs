using BotAI.Models;
using OpenAI_API.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotAI.Abstract
{
    public interface IMessageRepository
    {
        Task AddMessage(ChatMessageRole role, string message, long userId, string username);
        void CreateIfNotExist();
        List<ChatMessageModel> GetChatMessages(long userId);
    }
}