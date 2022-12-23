using Crawler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController
{
    private readonly ILogger<ChatController> _logger;
    private readonly Entry _entry;

    public ChatController(ILogger<ChatController> logger, Entry entry)
    {
        _logger = logger;
        _entry = entry;
    }
    [DisableCors]
    [HttpPost]
    public async Task<string> Say(string information)
    {
        if (information is null || information.Length <= 0) return "";
        if (_entry._asking)
        {
            return "请在上一个回答发送后再来询问";
        }
        await _entry.Ask(information);
        var rply = await _entry.GetRply();
        return rply;
    }
    [DisableCors]
    [HttpGet(Name = "isasking")]
    public bool IsAsking()
    {
        return _entry._asking;
    }
}