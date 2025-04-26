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
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--no-sandbox");
            options.AddArguments("--disable-dev-shm-usage");
            options.AddArguments("--headless=new");
            options.AddArguments("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:137.0) Gecko/20100101 Firefox/137.0");
            options.AddArguments("--ignore-certificate-errors");
            using (var driver = UndetectedChromeDriver.Create(options, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
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
                
                try
                {
                    var loginButton = driver.FindElement(By.ClassName("ds-sign-up-form__register-button"));
                    loginButton.Click();

                    //TextCopy.ClipboardService.SetText(shortTestString);
                    driver.ExecuteScript("navigator.clipboard.writeText(\"" + shortTestString + "\");");
                    var textArea = driver.FindElement(By.Id(inputId));
                    textArea.SendKeys(Keys.Control + "v");

                    Assert.That(textArea.Text, Is.EqualTo(shortTestString));
                }
                catch (Exception)
                {
                    var ss = driver.GetScreenshot();
                    Console.WriteLine(ss.AsBase64EncodedString);
                    throw;
                }
            }

            //driver.Quit();
        }
    }
}
