using BotAI.Models;
using System.Threading.Tasks;

namespace BotAI.Abstract
{
    public interface IBotHelper
    {
        Task SendText(long userId, string message);
    }
}
