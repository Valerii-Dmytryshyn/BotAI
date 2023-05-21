using BotAI.Abstract;
using BotAI.Models;
using BotAI.Services;
using Moq;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Images;
using Telegram.Bot.Types;

namespace BotAI.Test.Services
{
    [TestFixture]
    public class OpenaiServiceTests
    {
        private IOpenaiService _sut;
        private Mock<IOpenAIAPI> _openApiMock;
        private Mock<IUserService> _userServiceMock;
        private BotUser _user;

        [SetUp]
        public void SetUp()
        {
            _openApiMock = new Mock<IOpenAIAPI>();
            _userServiceMock = new Mock<IUserService>();
            _user = new BotUser()
            {
                Id = 1,
                Username = "TestUser",
                Messages = new List<ChatMessage>()
            };
            _sut = new OpenaiService(_openApiMock.Object, _userServiceMock.Object, _user);
        }

        [Test]
        public void VerifyAskQuestion()
        {
            //arrange
            var question = "Test Question";
            var answer = "Test Answer";
            var chatResult = new ChatResult()
            {
                Choices = new List<ChatChoice> 
                {
                    new ChatChoice
                    {
                        Message = new ChatMessage(ChatMessageRole.System, answer)
                    }
                }
            };
            _openApiMock.Setup(o => o.Chat.CreateChatCompletionAsync(It.IsAny<ChatRequest>())).ReturnsAsync(chatResult);
            _userServiceMock.Setup(u => u.AddMessage(ChatMessageRole.User, question, _user.Id, _user.Username));
            _userServiceMock.Setup(u => u.AddMessage(ChatMessageRole.System, answer, _user.Id, _user.Username));

            //act
            var result = _sut.AskQuestion(question).Result;

            //assert
            Assert.That(result, Is.EqualTo(answer));
            _openApiMock.Verify(o => o.Chat.CreateChatCompletionAsync(It.IsAny<ChatRequest>()), Times.Once);
            _userServiceMock.Verify(u => u.AddMessage(ChatMessageRole.User, question, _user.Id, _user.Username), Times.Once);
            _userServiceMock.Verify(u => u.AddMessage(ChatMessageRole.System, answer, _user.Id, _user.Username), Times.Once);
        }

        [Test]
        public void VerifyGenerateImage()
        {
            //arrange
            var request = "Test request";
            var imageResult = new ImageResult()
            {
                Data = new List<Data> 
                {
                    new Data
                    {
                        Url = "https://example.com/image"
                    }
                }
            };
            _openApiMock.Setup(o => o.ImageGenerations.CreateImageAsync(request)).ReturnsAsync(imageResult);

            //act
            var result = _sut.GenerateImage(request).Result;

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<InputFile>(result);
            _openApiMock.Verify(o => o.ImageGenerations.CreateImageAsync(request), Times.Once);
        }
    }
}
