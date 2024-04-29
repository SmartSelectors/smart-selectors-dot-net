namespace SmartSelectors.Selenium.Tests
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;
    using SeleniumExtras.WaitHelpers;

    public class FindIconTests
    {
        private IWebDriver _driver;

        [SetUp]
        public void SetUp()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            _driver.Manage().Window.Maximize();
        }

        [Test]
        public void FindIconAdaptiveSeedsCart()
        {
            _driver.Navigate().GoToUrl("https://www.adaptiveseeds.com/");
            Thread.Sleep(1000);
            var cartIcon = _driver.FindIcon(Icons.Cart);
            cartIcon.Click();
            var wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(500));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(
                ".woocommerce-mini-cart__empty-message")));
            var cartMessage = _driver.FindElement(By.CssSelector(".woocommerce-mini-cart__empty-message"));
            cartMessage.Text.Should().Be("No products in the cart.");
        }

        [Test]
        public void FindIconRestorationSeedsCart()
        {
            _driver.Navigate().GoToUrl("https://www.restorationseeds.com/");
            var cartIcon = _driver.FindIcon(Icons.Cart);
            cartIcon.Click();
            var cartMessage = _driver.FindElement(By.CssSelector("#main-menu div.shadow p.no-items"));
            cartMessage.Text.Should().Be("YOUR CART IS EMPTY");
        }

        [Test]
        public void FindIconEbayCart()
        {
            _driver.Navigate().GoToUrl("https://www.ebay.com/");
            var cartIcon = _driver.FindIcon(Icons.Cart);
            cartIcon.Click();
            var cartMessage = _driver.FindElement(By.CssSelector("div.empty-cart > div.font-title-3"));
            cartMessage.Text.Should().Be("No tienes artículos en el carro de compras.");
        }

        [Test]
        public void FindIconUnderwoodGardensCart()
        {
            _driver.Navigate().GoToUrl("https://store.underwoodgardens.com/");
            var cartIcon = _driver.FindIcon(Icons.Cart);
            cartIcon.Click();
            var cartMessage = _driver.FindElement(By.CssSelector("div.previewCart-emptyBody"));
            cartMessage.Text.Should().Be("Your cart is empty");
        }

        [Test]
        public void FindIconUnderwoodGardensFavorite()
        {
            _driver.Navigate().GoToUrl("https://store.underwoodgardens.com/");
            var wishList = _driver.FindIcon(Icons.Favorite);
            wishList.Click();
            var header = _driver.FindElement(By.CssSelector("h1.page-heading"));
            header.Text.Should().Be("Sign in");
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }
    }
}
