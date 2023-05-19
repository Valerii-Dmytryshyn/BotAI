using BotAI.Abstract;
using System.Threading.Tasks;
using Telegram.Bot;

namespace BotAI.Services
{
    public class BotHelper : IBotHelper
    {
        private readonly ITelegramBotClient _botClient;

        public BotHelper(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task SendText(long userId, string message) =>
            await _botClient.SendTextMessageAsync(userId, message);
    }
}
