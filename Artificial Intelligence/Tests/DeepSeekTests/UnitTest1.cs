using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumUndetectedChromeDriver;

namespace DeepSeekTests
{
    public class Tests
    {
        private String baseUrl = "https://chat.deepseek.com";
        private String shortTestString = "short";
        private String longTestString = "longlonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglong";
        private String inputId = "chat-input";
        private String username = "aitest068@gmail.com";
        private String password = "testPassword";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            using (var driver = UndetectedChromeDriver.Create(driverExecutablePath: await new ChromeDriverInstaller().Auto()))
                {

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(10000);

                driver.Navigate().GoToUrl(baseUrl);

                var inputs = driver.FindElements(By.ClassName("ds-input__input"));

                foreach (var input in inputs)
                {
                    if (input.GetAttribute("type") == "text")
                    {
                        input.SendKeys(username);
                    }
                    else if (input.GetAttribute("type") == "password")
                    {
                        input.SendKeys(password);
                    }
                }
                var loginButton = driver.FindElement(By.ClassName("ds-sign-up-form__register-button"));
                loginButton.Click();
                //await Task.Delay(5000);

                TextCopy.ClipboardService.SetText(shortTestString);
                var textArea = driver.FindElement(By.Id(inputId));
                textArea.SendKeys(Keys.Control + "v");

                Assert.That(textArea.Text, Is.EqualTo(shortTestString));
            }

            //driver.Quit();
        }
    }
}