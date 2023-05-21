using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using BotAI.Models;
using Telegram.Bot.Types;
using BotAI.Abstract;
using BotAI.Services;
using OpenAI_API;
using BotAI.Infastracture;
using Telegram.Bot.Requests.Abstractions;

namespace BotAI
{
    public class BotAI
    {
        private readonly ITelegramBotClient _botClient;
        private IOpenaiService _openaiService;
        private readonly IUserService _userService;
        private readonly IBotHelper _botHelper;
        private readonly string _openaiKey;
        private const string SetUpFunctionName = "Setup";
        private const string UpdateFunctionName = "BotAI";

        public BotAI()
        {
            _openaiKey = Environment.GetEnvironmentVariable("OpenAiToken");
            var telegramToken = Environment.GetEnvironmentVariable("TelegramToken");

            var storageAccountProxy = new StorageAccountProxy();
            var userRepository = new UserRepository(storageAccountProxy);
            var messageRepository = new MessageRepository(storageAccountProxy);
            messageRepository.CreateIfNotExist();

            _botClient = new TelegramBotClient(telegramToken);
            _userService = new UserService(userRepository, messageRepository);
            _botHelper = new BotHelper(_botClient);
        }

        [FunctionName(SetUpFunctionName)]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            var handleUpdateFunctionUrl = "https://" + req.Host.ToString() + req.Path.ToString().Replace(SetUpFunctionName, UpdateFunctionName);
            await _botClient.SetWebhookAsync(handleUpdateFunctionUrl);

            return new JsonResult($"Webhook '{handleUpdateFunctionUrl}' was set");
        }

        //TODO: Image to text recognition
        [FunctionName(UpdateFunctionName)]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            var request = await GenerateRequest(req);
            PopulateOpenAiService(request.User);//TODO: refactor it somehow

            //if (request.Update.Type != UpdateType.Message) { }//TODO:
            if (NeedToChangeMode(request))
            {
                _userService.ChangeMode(request);
                await _botHelper.SendText(request.User.Id, "Mode was changed to " + (request.MessageText == Constants.TextModeCommand ? 
                    "Text Genaration mode. Now you can ask any assist. For example 'Write code that will sum two numbers'" :
                    "Image Genaration mode. Now you can write any request and bot will return you image related to your request. For example 'Spider-man in Toy Story'"));
                return new OkResult();
            }

            if (request.User.Mode == GenerationMode.Text)
            {
                var response = await _openaiService.AskQuestion(request.MessageText);
                await _botHelper.SendText(request.User.Id, response);
            }
            else if (request.User.Mode == GenerationMode.Image)
            {
                await _botHelper.SendText(request.User.Id, "Your request is processing...");
                var image = await _openaiService.GenerateImage(request.MessageText);
                await _botHelper.SendImage(request.User.Id, image);
            }

            return new OkResult();
        }

        //TODO: change if not message type
        private async Task<Request> GenerateRequest(HttpRequest req)
        {
            var body = await req.ReadAsStringAsync();
            var update = JsonConvert.DeserializeObject<Update>(body);
            var user = await _userService.GetUser(update);

            return new Request(update, user);
        }

        private void PopulateOpenAiService(BotUser user)
        {
            var openai = new OpenAIAPI(_openaiKey);
            _openaiService = new OpenaiService(openai, _userService, user);
        }

        private bool NeedToChangeMode(Request request) =>
            request.MessageText == Constants.TextModeCommand || request.MessageText == Constants.ImageModeCommand;
    }
}
