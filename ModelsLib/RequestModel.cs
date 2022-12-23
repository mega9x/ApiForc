using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ModelsLib;

public class RequestModel
{
    [JsonProperty("action")] public string Action { get; set; } = "next";

    [JsonProperty("messages")]
    public Message[] Messages { get; set; }

    [JsonProperty("conversation_id")]
    public string ConversationId { get; set; }

    [JsonProperty("parent_message_id")] public string ParentMessageId { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("model")] public string Model { get; set; } = "text-davinci-002-render";
}

public class Message
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("role")] public string Role { get; set; } = "user";

    [JsonProperty("content")]
    public Content Content { get; set; }
}

public class Content
{
    [JsonProperty("content_type")] public string ContentType { get; set; } = "text";

    [JsonProperty("parts")]
    public string[] Parts { get; set; }
}