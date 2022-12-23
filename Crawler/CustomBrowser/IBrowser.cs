using OpenQA.Selenium;

namespace Crawler.CustomBrowser;

public interface IBrowser
{
    IWebDriver GetDriver();
}