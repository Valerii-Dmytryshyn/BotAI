using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotAI.Abstract
{
    public interface IOpenaiService
    {
        Task<string> AskQuestion(string question);
        Task<InputFile> GenerateImage(string request);
    }
}
