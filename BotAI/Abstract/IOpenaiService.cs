using System.Threading.Tasks;

namespace BotAI.Abstract
{
    public interface IOpenaiService
    {
        Task<string> AskQuestion(string question);
    }
}
