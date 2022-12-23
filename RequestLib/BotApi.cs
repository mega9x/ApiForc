using ModelsLib;
using ModelsLib.Const;
using Newtonsoft.Json;

namespace RequestLib;

public class BotApi
{
    public OptionModel Option { get; set; }
    public BotApi()
    {
        if (!Directory.Exists(ConfigStr.CONFIG_PATH))
        {
            Directory.CreateDirectory(ConfigStr.CONFIG_PATH);
        }
        if (!File.Exists(Path.Combine(ConfigStr.GENERAL_CONFIG)))
        {
            Option = new OptionModel();
            var json = JsonConvert.SerializeObject(Option);
            using var fs = File.Create(Path.Combine(ConfigStr.CONFIG_PATH,ConfigStr.GENERAL_CONFIG));
            using var sw = new StreamWriter(fs);
            sw.WriteLine(json);
            sw.Flush();
        }
        else
        {
            Option = JsonConvert.DeserializeObject<OptionModel>(File.ReadAllText(ConfigStr.GENERAL_CONFIG))!;
        }
        if (Option.SessionToken is null || Option.CfClearance is null || Option.SessionToken is null ||
            Option.SessionToken.Length * Option.CfClearance.Length * Option.SessionToken.Length <= 0)
        {
            throw new Exception("config.json is invalid.");
        }
    }
}