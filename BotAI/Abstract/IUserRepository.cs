using BotAI.Models;
using OpenAI_API.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotAI.Abstract
{
    public interface IUserRepository
    {
        List<ChatMessage> GetUserMessages(long userId);
        Task<BotUser> RegisterIfNotExistOrGetUser(BotUser user);
        Task AddMessage(BotUser user, ChatMessage chatMessage);
        Task ChangeMode(BotUser user, GenerationMode mode);
    }
}