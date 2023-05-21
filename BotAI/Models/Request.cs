using Telegram.Bot.Types;

namespace BotAI.Models
{
    public class Request
    {
        public Update Update { get; set; }
        public BotUser User { get; set; }

        public Request(Update update, BotUser user)
        {
            Update = update;
            User = user;
        }

        public string MessageText => Update.Message.Text;
    }
}
