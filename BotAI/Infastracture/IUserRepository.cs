using BotAI.Models;
using OpenAI_API.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotAI.Infastracture
{
    public interface IUserRepository
    {
        List<ChatMessage> GetUserMessages(long userId);
        Task<BotUser> RegisterIfNotExistOrGetUser(BotUser user);
        Task AddMessage(BotUser user, ChatMessage chatMessage);
    }
}