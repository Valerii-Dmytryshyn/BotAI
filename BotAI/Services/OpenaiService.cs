using BotAI.Abstract;
using BotAI.Models;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BotAI.Services
{
    public class OpenaiService : IOpenaiService
    {
        private readonly OpenAIAPI _openAI;
        private readonly IUserService _userService;
        private readonly BotUser _user;
        private readonly ChatRequest _chatRequest;

        public OpenaiService(OpenAIAPI openAI, IUserService userService, BotUser user)
        {
            _chatRequest = GenerateUserChatRequst(user);

            _openAI = openAI;
            _userService = userService;
            _user = user;
        }

        public async Task<string> AskQuestion(string question)
        {
            _chatRequest.Messages.Add(new ChatMessage(ChatMessageRole.User, question));
            var response = await _openAI.Chat.CreateChatCompletionAsync(_chatRequest);

            _userService.AddMessage(ChatMessageRole.User, question, _user.Id);
            _userService.AddMessage(ChatMessageRole.System, response.ToString(), _user.Id);

            return response.ToString();
        }

        private ChatRequest GenerateUserChatRequst(BotUser user)
        {
            var chatRequest = new ChatRequest();
            chatRequest.Messages = user.Messages;

            return chatRequest;
        }
    }
}
