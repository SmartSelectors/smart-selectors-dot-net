# Smart Selectors .Net

[![](https://img.shields.io/github/workflow/status/SmartSelectors/smart-selectors-dot-net/publish-SmartSelectors.Selenium?style=plastic)](https://github.com/SmartSelectors/smart-selectors-dot-net/actions) [![](https://img.shields.io/nuget/dt/SmartSelectors.Selenium?color=blue&style=plastic)](https://www.nuget.org/packages/SmartSelectors.Selenium/) [![](https://img.shields.io/github/license/SmartSelectors/smart-selectors-dot-net?color=lightgrey&style=plastic)](https://github.com/SmartSelectors/smart-selectors-dot-net/blob/master/LICENSE)

Smart Selectors .NET is a set of Selenium extension methods to find elemens using machine learning models.

## Setup

Add the SmartSelectors.Selenium Nuget package to your Selenium project from Visual Studio Nuget Package Manager or from the console

```
dotnet add package SmartSelenium.Selenium
```

### Find icons

```csharp
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using SmartSelectors;
using SmartSelectors.Selenium;

namespace SmartSelectorsExample
{
    public class GItHubReadmeExample
    {
        [Test]
        public void ClickFavoriteIcon()
        {
            var driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://github.com/SmartSelectors/smart-selectors-dot-net");
            var favoriteIcon = driver.FindIcon(Icons.Favorite);
            favoriteIcon.Click();
        }
    }
}
```

The SmartSelectors.Selenium namespace adds some fancy extension methods to the driver. The first one is the FindIcon method, that receives an Enum value representing an icon, finds the icon in the page and returns an IWebElement.

Search trough the Icons Enum in SmartSelectors namespace to check the available options.

### Find text in images
Coming soon...

### Self healing selectors

If you already have a working selector but want to make it more reliable, you can use the overload to the regular FindElement method as a fallback. Just add as an extra parameter the Icon the selector represents.

```csharp
var driver = new ChromeDriver();
driver.Navigate().GoToUrl("https://github.com/SmartSelectors/smart-selectors-dot-net");
var favoriteIcon = driver.FindElement(By.XPath("//button[contains(@class,'js-toggler-target')]"), Icons.Favorite);
favoriteIcon.Click();
```

In case the DOM changes and the selector gets broken, the icon search will be executed as in the first example.

### License: Apache License 2.0
