using Newtonsoft.Json;

namespace ModelsLib;

public class ModerationModel
{
    [JsonProperty("input")]
    public string Input { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("conversation_id")]
    public Guid ConversationId { get; set; }

    [JsonProperty("message_id")]
    public Guid MessageId { get; set; }
}