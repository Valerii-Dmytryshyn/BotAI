using BotAI.Abstract;
using BotAI.Infastracture;
using BotAI.Models;
using BotAI.Services;
using Moq;
using OpenAI_API.Chat;
using Telegram.Bot.Types;

namespace BotAI.Test.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private IUserService _sut;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IMessageRepository> _messageRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _messageRepositoryMock = new Mock<IMessageRepository>();
            _sut = new UserService(_userRepositoryMock.Object, _messageRepositoryMock.Object);
        }

        [Test]
        public void ChangeMode_ShouldChangeModeToText_WhenCommandIsTextmode()
        {
            //arrange
            var request = new Request(new Update { Message = new Message { Text = Constants.TextModeCommand } }, new BotUser());
            _userRepositoryMock.Setup(u => u.ChangeMode(request.User, GenerationMode.Text));
            
            //act
            _sut.ChangeMode(request);

            //assert
            _userRepositoryMock.Verify(u => u.ChangeMode(request.User, GenerationMode.Text), Times.Once);
        }

        [Test]
        public void ChangeMode_ShouldChangeModeToImage_WhenCommandIsImagemode()
        {
            //arrange
            var request = new Request(new Update { Message = new Message { Text = Constants.ImageModeCommand } }, new BotUser());
            _userRepositoryMock.Setup(u => u.ChangeMode(request.User, GenerationMode.Image));

            //act
            _sut.ChangeMode(request);

            //assert
            _userRepositoryMock.Verify(u => u.ChangeMode(request.User, GenerationMode.Image), Times.Once);
        }

        [Test]
        public void VerifyGetUser()
        {  //arrange
            var timestamp = DateTime.UtcNow;
            var user = new BotUser()
            {
                Id = 1,
                Username = "Test",
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                Messages = new List<ChatMessage>(),
                Mode = (int)GenerationMode.Text,
                PartitionKey = "1",
                ETag = "Etag",
                RowKey = "1",
            };
            var update = new Update
            {
                Message = new Message
                {
                    Chat = new Chat
                    {
                        Id = 1,
                        FirstName = "TestFirstName",
                        LastName = "TestLastName",
                        Username = "Test"
                    }
                },
            };
            _userRepositoryMock.Setup(u => u.RegisterIfNotExistOrGetUser(It.IsAny<BotUser>())).ReturnsAsync(user);
            _messageRepositoryMock.Setup(m => m.GetChatMessages(user.Id))
                .Returns(new List<ChatMessageModel> { new ChatMessageModel { Content = "test message", Role = ChatMessageRole.User } });

            //act
            var result = _sut.GetUser(update).Result;

            //assert
            Assert.Multiple(() =>
            {
               Assert.That(result.Id, Is.EqualTo(1));
               Assert.That(result.Username, Is.EqualTo("Test"));
               Assert.That(result.FirstName, Is.EqualTo("TestFirstName"));
               Assert.That(result.LastName, Is.EqualTo("TestLastName"));
               Assert.That(result.Messages.First().Content, Is.EqualTo("test message"));
               Assert.That(result.Messages.First().Role, Is.EqualTo(ChatMessageRole.User));
               Assert.That(result.Mode, Is.EqualTo(GenerationMode.Text));
               Assert.That(result.PartitionKey, Is.EqualTo("1"));
               Assert.That(result.ETag, Is.EqualTo("Etag"));
               Assert.That(result.RowKey, Is.EqualTo("1"));
            });
        }
    }
}
