using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotAI.Abstract
{
    public interface IBotHelper
    {
        Task SendText(long userId, string message);
        Task SendImage(long userId, InputFile image);
    }
}
