using BotAI.Abstract;
using BotAI.Models;
using OpenAI_API.Chat;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotAI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public UserService(IUserRepository userRepository, IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public async Task<BotUser> GetUser(Update update)
        {
            var user = await _userRepository.RegisterIfNotExistOrGetUser(BotUser.GenerateBotUser(update));
            var history = _messageRepository.GetChatMessages(user.Id);
            user.Messages = history.Select(m => new ChatMessage(m.Role, m.Content)).ToList();

            return user;
        }

        public void ChangeMode(Request request)
        {
            if (request.MessageText == Constants.TextModeCommand)
                _userRepository.ChangeMode(request.User, GenerationMode.Text);
            else if(request.MessageText == Constants.ImageModeCommand)
                _userRepository.ChangeMode(request.User, GenerationMode.Image);
        }


        public async void AddMessage(ChatMessageRole role, string message, long userId, string username) =>
            await _messageRepository.AddMessage(role, message, userId, username);
    }
}
