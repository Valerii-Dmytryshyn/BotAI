using BotAI.Models;
using OpenAI_API.Chat;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotAI.Infastracture
{
    public class UserRepository : IUserRepository
    {
        private readonly StorageAccountProxy _storage;

        public UserRepository(StorageAccountProxy storage)
        {
            _storage = storage;
        }

        public async Task<BotUser> RegisterIfNotExistOrGetUser(BotUser user)
        {
            var userTable = _storage.GetCloudTable(Constants.UserTableName);
            var query = _storage.GetSearhQuery<BotUser>(user.Id.ToString());
            var userFromTable = userTable.ExecuteQuery(query).FirstOrDefault();

            if (userFromTable != null)
                return userFromTable;

            await _storage.InsertEntity(userTable, user);
            return user;
        }

        public List<ChatMessage> GetUserMessages(long userId)
        {
            var userTable = _storage.GetCloudTable(Constants.UserTableName);
            var query = _storage.GetSearhQuery<BotUser>(userId.ToString());

            return userTable.ExecuteQuery(query).FirstOrDefault().Messages;
        }

        public async Task AddMessage(BotUser user, ChatMessage chatMessage)
        {
            user.Messages.Add(chatMessage);
            var userTable = _storage.GetCloudTable(Constants.UserTableName);

            await _storage.InsertEntity(userTable, user);
        }
    }
}
