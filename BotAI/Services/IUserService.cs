using BotAI.Models;
using OpenAI_API.Chat;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotAI.Services
{
    public interface IUserService
    {
        Task<BotUser> GetUser(Update update);
        void AddMessage(ChatMessageRole role, string message, long userId, string username);
        void ChangeMode(Request request);
    }
}