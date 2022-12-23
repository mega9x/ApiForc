using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using RandomUserAgent;
using Selenium.WebDriver.UndetectedChromeDriver;
using Sl.Selenium.Extensions.Chrome;

namespace Crawler.CustomBrowser;

public class Chrome : IBrowser
{
    private IWebDriver _driver;

    public Chrome(bool showWindow)
    {
        var para = new ChromeDriverParameters();
        _driver = UndetectedChromeDriver.Instance(!showWindow);
        _driver.Manage().Cookies.DeleteAllCookies();
        _driver.ExecuteJavaScript("Object.defineProperties(navigator, {webdriver:{get:()=>undefined}});");
    }
    
    
    public IWebDriver GetDriver()
    {
        return _driver;
    }
}