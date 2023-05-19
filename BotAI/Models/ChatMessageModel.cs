using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using OpenAI_API.Chat;

namespace BotAI.Models
{
    public class ChatMessageModel: TableEntity
    {
        public ChatMessageModel()
        {
            this.Role = ChatMessageRole.User;
        }

        public ChatMessageModel(ChatMessageRole role, string content)
        {
            this.Role = role;
            this.Content = content;
        }

        [JsonProperty("role")]
        internal string rawRole { get; set; }

        [JsonIgnore]
        public ChatMessageRole Role
        {
            get
            {
                return ChatMessageRole.FromString(rawRole);
            }
            set
            {
                rawRole = value.ToString();
            }
        }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("userId")]
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
