namespace SmartSelectors.Playwright.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using PlaywrightSharp;

    public class FirstTest
    {
        [Test]
        public static async Task Run()
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new LaunchOptions()
            {
                Headless = false
            });
            var page = await browser.NewPageAsync();

            await page.GoToAsync("https://www.amazon.com/");
            var handle = await page.QuerySelectorAsync("#nav-cart-count-container");
            await handle.ClickAsync();

            Thread.Sleep(2000);
        }
    }
}
