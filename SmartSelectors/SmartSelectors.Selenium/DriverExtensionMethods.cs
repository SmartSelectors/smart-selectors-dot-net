namespace SmartSelectors.Selenium
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using OpenQA.Selenium;

    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public static class DriverExtensionMethods
    {
        private const string ExceptionMessage = "Unable to locate {0} icon.";

        /// <summary>
        /// Finds an Icon element matching <paramref name="label"/>
        /// </summary>
        /// <param name="label">The label representing the Icon to find</param>
        /// <param name="useRemoteModel">Flag to use the local/remote model (optional, defaults to false) </param>
        /// <returns>The first matching <see cref="T:OpenQA.Selenium.IWebElement" />.</returns>
        /// <exception cref="T:OpenQA.Selenium.NoSuchElementException">If no element matches the criteria.</exception>
        public static IWebElement FindIcon(this IWebDriver driver, Icons label, bool useRemoteModel = false)
        {
            var model = useRemoteModel ? (IModel) new ApiModel() : new OnnxModel(null);
            var webElement = FindAndPredictTWebElement(driver, model, label);
            if (webElement == null) throw new NoSuchElementException(string.Format(ExceptionMessage, label));
            return webElement;
        }

        /// <summary>
        /// Finds an Icon element matching <paramref name="label"/>
        /// </summary>
        /// <param name="label">The label representing the Icon to find</param>
        /// <param name="useRemoteModel">Flag to use the local/remote model (optional, defaults to false) </param>
        /// <returns>A list of matching <see cref="T:OpenQA.Selenium.IWebElement" />.</returns>
        /// <exception cref="T:OpenQA.Selenium.NoSuchElementException">If no element matches the criteria.</exception>
        public static IReadOnlyCollection<IWebElement> FindIcons(this IWebDriver driver, Icons label, bool useRemoteModel = false)
        {
            var model = useRemoteModel ? (IModel)new ApiModel() : new OnnxModel(null);
            var webElements = FindAndPredictTWebElements(driver, model, label);
            if (webElements == null) throw new NoSuchElementException(string.Format(ExceptionMessage, label));
            return webElements;
        }

        /// <summary>
        /// Finds the first <see cref="T:OpenQA.Selenium.IWebElement" /> using the given  <see cref="T:OpenQA.Selenium.By" /> method.
        /// If an element is not found it finds an Icon element matching <paramref name="label"/>
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <param name="label">The label representing the Icon to find</param>
        /// <returns>The first matching <see cref="T:OpenQA.Selenium.IWebElement" />.</returns>
        /// <exception cref="T:OpenQA.Selenium.NoSuchElementException">If no element matches the criteria.</exception>
        public static IWebElement FindElement(this IWebDriver driver, By by, Icons label)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (NoSuchElementException e)
            {
                var model = new OnnxModel(null);
                var webElement = FindAndPredictTWebElement(driver, model, label);
                if (webElement == null) throw new NoSuchElementException($"{e.Message}.\n {string.Format(ExceptionMessage, label)}");
                return webElement;
            }
        }

        private static IWebElement FindAndPredictTWebElement(IWebDriver driver, IModel model, Icons label)
        {
            var javaScriptExecutor = (IJavaScriptExecutor)driver;
            if (!(javaScriptExecutor.ExecuteScript(Properties.Resources.GetElements) is IList<IWebElement> webElements)) return default;
            IWebElement webElement = null;
            foreach (var element in webElements)
            {
                var elementImage = ((ITakesScreenshot) element)?.GetScreenshot();
                var byteArray = elementImage?.AsByteArray;
                if (!model.Predict(byteArray, label.ToString().ToLower()).Prediction) continue;
                webElement = element;
                break;
            }
            return webElement;
        }

        private static List<IWebElement> FindAndPredictTWebElements(IWebDriver driver, IModel model, Icons label)
        {
            var javaScriptExecutor = (IJavaScriptExecutor)driver;
            if (!(javaScriptExecutor.ExecuteScript(Properties.Resources.GetElements) is IList<IWebElement> webElements)) return default;
            var elements = new List<IWebElement>();
            foreach (var element in webElements)
            {
                var elementImage = ((ITakesScreenshot)element)?.GetScreenshot();
                var byteArray = elementImage?.AsByteArray;
                if (!model.Predict(byteArray, label.ToString().ToLower()).Prediction) continue;
                elements.Add(element);
            }
            return elements;
        }
    }
}
