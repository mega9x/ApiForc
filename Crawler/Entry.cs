using System.IO.Enumeration;
using System.Text;
using Crawler.CustomBrowser;
using ModelsLib;
using ModelsLib.Const;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;

namespace Crawler;

public class Entry
{
    private const string btn = ".right-1";
    private readonly Task _wait = Task.Delay(1500);
    private string Uri = "https://chat.openai.com/chat/";
    public OptionModel Option { get; set; }
    public IBrowser Browser { get; private set; }
    public IWebDriver Driver { get; private set; }
    public string LastAsk { get; private set; }
    public string LastAnswer { get; private set; }
    public bool _asking { get; private set; } = false;
    public int Timeout { get; set; } = 90;
    private bool _lock = false;
    private bool _notReady = true;

    public Entry()
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

        this.Uri += Option.ChatSession;
        Browser = new Chrome(true);
        Driver = Browser.GetDriver();
        Driver.Url = Uri;
        Driver.Manage().Cookies.DeleteAllCookies();
        var cf = new Cookie("cf_clearance", Option.CfClearance, ".chat.openai.com", "/", DateTime.MaxValue, true, true, "None");
        var session = new Cookie("__Secure-next-auth.session-token", Option.SessionToken, "chat.openai.com", "/", DateTime.MaxValue,
            true, true, "Lax");
        // var cfBm = new Cookie("__cf_bm", "HBsiM_rv9urjd9vvGQTD6xVIw3yrpWMtv2xvq4eB7Wk-1671697177-0-ASQ64yi5WEAsxhLfUDOXhglWLoVAMuse6GWAi+MSzTKuAuhDW2Dx5xE6lPyO+RCyYFxz+Jlmhf5MPerTumicOhjodBeyS+3H4wAOfiIeOe+RaW+p/5VddEKDzQf8Du8ON2wLkOVb/tMCnkXKh+vL17CeO/wurnkgMUUDYINGsbABMDzEf2RdN0oYdnEqVsSPig==", "chat")
        Driver.Manage().Cookies.AddCookie(cf);
        Driver.Manage().Cookies.AddCookie(session);
        Driver.ExecuteJavaScript("localStorage.setItem('oai/apps/hasSeenOnboarding/chat', '\"" + DateTime.Today.ToString("yyyy-MM-dd") + "\"')");
        Driver.ExecuteJavaScript("window.open('" + Uri + "');");
        _ = Init();
    }

    private async Task Init()
    {
        while (_notReady)
        {
            try
            {
                var textarea = Driver.FindElement(By.TagName("textarea"));
            }
            catch (Exception e)
            {
                await _wait;
                continue;
            }
            _notReady = false;
        }
    }
    
    public async Task<Entry> Ask(string text)
    {
        var action = new Actions(Driver);
        var delay = Task.Delay(1500);
        while (_notReady)
        {
            await delay;
        }
        LastAsk = text;
        action.MoveToElement(Driver.FindElement(By.CssSelector(".m-0")));
        var textarea = Driver.FindElement(By.CssSelector(".m-0"));
        textarea.Click();
        _asking = true;
        await Task.Delay(500);
        var lines = text.Split("\\n");
        foreach(var line in lines)
        {
            textarea.SendKeys(line);
            await Task.Delay(500);
            action.SendKeys(Keys.Shift + Keys.Enter + Keys.Clear);
            action.Build().Perform();
            await Task.Delay(500);
        }
        textarea.SendKeys(text);
        textarea.SendKeys(Keys.Enter);
        // var requestBody = new RequestModel()
        // {
        //     ConversationId = Option.ChatSession,
        //     Messages = new Message[]
        //     {
        //         new Message()
        //         {
        //             Content = new Content
        //             {
        //                 Parts = new string[]
        //                 {
        //                     text
        //                 }
        //             }
        //         }
        //     },
        // };
        // var bodyString = JsonConvert.SerializeObject(requestBody);
        // var js = await File.ReadAllTextAsync("./js/FetchMessage.js");
        // var result = Driver.ExecuteJavaScript<string>(js + bodyString + "');");
        return this;
    }

    public async Task<string> GetRply()
    {
        var currentTime = 0;
        var str = "";
        _asking = true;
        while (currentTime != Timeout)
        {
            try
            {
                Driver.FindElements(By.CssSelector(".result-streaming"));
                await Task.Delay(1000);
                currentTime++;
                if (Driver.FindElements(By.CssSelector(".result-streaming")) is null)
                {
                    break;
                }
            }
            catch(Exception e)
            {
                break;
            }
        }
        if (currentTime >= Timeout)
        {
            _asking = false;
            return "没有获取到回答, 可能是服务器有问题. 请联系我的维护人员!";
        }
        var allAnswers = Driver.FindElements(By.CssSelector(".markdown"));
        foreach (var element in allAnswers.Last().FindElements(By.TagName("p")))
        {
            str += element.Text;
            str += Environment.NewLine;
        }
        _asking = false;
        return str;
    }
}