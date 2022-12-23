using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Crawler.CustomBrowser;

public class Wait
{
    public static Wait Instance = new Lazy<Wait>(() => new Wait()).Value;
    public int WaitSeconds { get; set; } = 30;
    public WebDriverWait ToWait(IWebDriver driver)
    {
        return new(driver, TimeSpan.FromSeconds(WaitSeconds));
    }
}