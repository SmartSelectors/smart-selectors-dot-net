namespace SmartSelectors.Selenium.Tests
{
    using System;
    using FluentAssertions;
    using NUnit.Framework;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;

    public class DriverExtensionMethodsIntegrationTests
    {
        private IWebDriver _driver;
        private const string CartMessageXPath = "//h1[@data-automation-id='cart-list-title']/span/span";
        private const string ExpectedCartMessage = "0 items in your cart";

        [SetUp]
        public void SetUp()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            _driver.Manage().Window.Maximize();
        }

        [Test]
        public void FindIconWithLocalModel()
        {
            _driver.Navigate().GoToUrl("https://www.walmart.com/");
            var cartIcon = _driver.FindIcon(Icons.Cart);
            cartIcon.Click();
            var cartMessage = _driver.FindElement(By.XPath(CartMessageXPath));
            cartMessage.Text.Should().Be(ExpectedCartMessage);
        }

        [Test]
        public void FindIconWithApiModel()
        {
            _driver.Navigate().GoToUrl("https://www.walmart.com/");
            var cartIcon = _driver.FindIcon(Icons.Cart, true);
            cartIcon.Click();
            var cartMessage = _driver.FindElement(By.XPath(CartMessageXPath));
            cartMessage.Text.Should().Be(ExpectedCartMessage);
        }

        [Test]
        public void FindIconShouldThrowNoSuchElementException()
        {
            _driver.Navigate().GoToUrl("https://www.walmart.com/");
            _driver.Invoking(x => x.FindIcon(Icons.Add))
                .Should().Throw<NoSuchElementException>()
                .WithMessage($"Unable to locate {Icons.Add} icon.");
        }

        [Test]
        public void FindIconWithASelfHealingSelector()
        {
            _driver.Navigate().GoToUrl("https://www.walmart.com/");
            var cartIcon = _driver.FindElement(By.CssSelector("#brokenSelector"), Icons.Cart);
            cartIcon.Click();
            var cartMessage = _driver.FindElement(By.XPath(CartMessageXPath));
            cartMessage.Text.Should().Be(ExpectedCartMessage);
        }

        [Test]
        public void SelfHealingSelectorShouldThrowNoSuchElementException()
        {
            _driver.Navigate().GoToUrl("https://www.walmart.com/");
            _driver.Invoking(x => x.FindElement(By.CssSelector("#brokenSelector"), Icons.Add))
                .Should().Throw<NoSuchElementException>()
                .Where(x => x.Message.Contains("no such element: Unable to locate element:"))
                .Where(x => x.Message.Contains($"Unable to locate {Icons.Add} icon."));
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }
    }
}