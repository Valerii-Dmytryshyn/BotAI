using BotAI.Abstract;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
            await _botClient.SendTextMessageAsync(userId, message, null, ParseMode.Markdown);

        public async Task SendImage(long userId, InputFile image) => 
            await _botClient.SendPhotoAsync(userId, image);
    }
}
