using BotAI.Abstract;
using BotAI.Models;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotAI.Services
{
    public class OpenaiService : IOpenaiService
    {
        private readonly IOpenAIAPI _openAI;
        private readonly IUserService _userService;
        private readonly BotUser _user;
        private readonly ChatRequest _chatRequest;

        public OpenaiService(IOpenAIAPI openAI, IUserService userService, BotUser user)
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

            _userService.AddMessage(ChatMessageRole.User, question, _user.Id, _user.Username);
            _userService.AddMessage(ChatMessageRole.System, response.ToString(), _user.Id, _user.Username);

            return response.ToString();
        }

        public async Task<InputFile> GenerateImage(string request)
        {
            var result = await _openAI.ImageGenerations.CreateImageAsync(request);
            return InputFile.FromString(result.Data[0].Url);
        }

        private ChatRequest GenerateUserChatRequst(BotUser user)
        {
            var chatRequest = new ChatRequest();
            chatRequest.Messages = user.Messages;

            return chatRequest;
        }
    }
}
