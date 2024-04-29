namespace SmartSelectors.Selenium.Tests
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;

    public class DriverExtensionMethodsIntegrationTests
    {
        private IWebDriver _driver;
        private const string CartCardSelector = "#main-menu div.shadow p.no-items";
        private WebDriverWait _wait;

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            options.AddArgument("--lang=en");
            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            _driver.Manage().Window.Maximize();
            _wait = new WebDriverWait(_driver, TimeSpan.FromMilliseconds(30000));
        }

        [Test]
        public void FindIconWithLocalModel()
        {
            _driver.Navigate().GoToUrl("https://www.restorationseeds.com/");
            var cartIcon = _driver.FindIcon(Icons.Cart);
            cartIcon.Click();
            var cartMessage = _driver.FindElement(By.CssSelector(CartCardSelector));
            cartMessage.Text.Should().Be("YOUR CART IS EMPTY");
        }

        [Test]
        public void FindIconWithApiModel()
        {
            _driver.Navigate().GoToUrl("https://www.restorationseeds.com/");
            var cartIcon = _driver.FindIcon(Icons.Cart, true);
            cartIcon.Click();
            var cartMessage = _driver.FindElement(By.CssSelector(CartCardSelector));
            cartMessage.Text.Should().Be("YOUR CART IS EMPTY");
        }

        [Test]
        public void FindIconShouldThrowNoSuchElementException()
        {
            _driver.Navigate().GoToUrl("https://www.restorationseeds.com/");
            _driver.Invoking(x => x.FindIcon(Icons.Add))
                .Should().Throw<NoSuchElementException>()
                .WithMessage($"Unable to locate {Icons.Add} icon.");
        }

        [Test]
        public void FindIconWithASelfHealingSelector()
        {
            _driver.Navigate().GoToUrl("https://www.restorationseeds.com/");
            var cartIcon = _driver.FindElement(By.CssSelector("#brokenSelector"), Icons.Cart);
            cartIcon.Click();
            var cartMessage = _driver.FindElement(By.CssSelector(CartCardSelector));
            cartMessage.Text.Should().Be("YOUR CART IS EMPTY");
        }

        [Test]
        public void SelfHealingSelectorShouldThrowNoSuchElementException()
        {
            _driver.Navigate().GoToUrl("https://www.restorationseeds.com/");
            _driver.Invoking(x => x.FindElement(By.CssSelector("#brokenSelector"), Icons.Add))
                .Should().Throw<NoSuchElementException>()
                .Where(x => x.Message.Contains("no such element: Unable to locate element:"))
                .Where(x => x.Message.Contains($"Unable to locate {Icons.Add} icon."));
        }

        [Test]
        public void FindSeveralIcons()
        {
            _driver.Navigate().GoToUrl("http://opencart.abstracta.us/");
            var favIcons = _driver.FindIcons(Icons.Favorite);
            favIcons.Should().HaveCountGreaterThan(1);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }
    }
}