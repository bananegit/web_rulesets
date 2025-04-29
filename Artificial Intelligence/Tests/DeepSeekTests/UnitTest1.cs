using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumUndetectedChromeDriver;

namespace DeepSeekTests
{
    public class Tests
    {
        private String baseUrl = "https://chat.deepseek.com";
        private String username = Environment.GetEnvironmentVariable("dsUsername");
        private String password = Environment.GetEnvironmentVariable("dsPassword");

        private String longTestString = "longlonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglong";
        private String searchPrompt = "do a web search for the current price of tesla, if you can not search live data state so plainly";
        private String policyWarning = "\"Your last prompt was blocked by Web Policy for exceeding the maximum prompt length of 100 characters\"";
        private String bitmapFilePath = AppContext.BaseDirectory + "BitmapFile.bmp";

        private ChromeOptions options = new ChromeOptions();
        private Dictionary<string, object> prefs = new Dictionary<string, object>();
        private TimeSpan commandTimeout = TimeSpan.FromSeconds(600);

        //UI element classnames / ids
        private String loginInputsCn = "ds-input__input";
        private String loginButtonCn = "ds-sign-up-form__register-button";
        private String sendPromptButtonCn = "_7436101";
        private String replyContainerCn = "ds-markdown-paragraph";
        private String uploadErrorContainerCn = "_5119742";
        private String thinkContainerCN = "_19db599";
        private String inputId = "chat-input";


        [SetUp]
        public void Setup()
        {
            options = new ChromeOptions();
            prefs = new Dictionary<string, object>();
            options.AddArguments("--no-sandbox");
            options.AddArguments("--disable-dev-shm-usage");
            options.AddArguments("--headless=new");
            options.AddArguments("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:137.0) Gecko/20100101 Firefox/137.0");
            var t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            
            prefs.Add("profile.content_settings.exceptions.clipboard", new Dictionary<string, object>() { { "[*.]deepseek.com,*", new Dictionary<string, object> { { "last_modfied", (int)t.TotalSeconds * 1000 }, { "setting", 1 } } } });
        }

        [TestCase(null, TestName ="Enforce DeepThink Setting (WebApp)")]
        public async Task EnforceDeepThinkSettingsTest(object? param)
        {

            using (var driver = UndetectedChromeDriver.Create(options, commandTimeout:commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
            {
                try
                {
                    authenticate(driver);

                    var textArea = driver.FindElement(By.Id(inputId));
                    textArea.SendKeys(searchPrompt);
                    var sendButton = driver.FindElement(By.ClassName(sendPromptButtonCn));
                    sendButton.Click();

                    var reply = driver.FindElement(By.ClassName(replyContainerCn));
                    var thinkContainer = driver.FindElement(By.ClassName(thinkContainerCN));

                    StringAssert.IsMatch("Thought for [0-9]* seconds", thinkContainer.Text);
                }
                catch (Exception)
                {
                    var ss = driver.GetScreenshot();
                    Console.WriteLine("Error Screenshot: \r\n" + ss.AsBase64EncodedString);
                    throw;
                }
            }
        }

        [TestCase(null, TestName = "Enforce Search Setting (WebApp)")]
        public async Task EnforceSearchSettingsTest(object? param)
        {

            using (var driver = UndetectedChromeDriver.Create(options, commandTimeout: commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
            {
                try
                {
                    authenticate(driver);

                    var textArea = driver.FindElement(By.Id(inputId));
                    textArea.SendKeys(searchPrompt);
                    var sendButton = driver.FindElement(By.ClassName(sendPromptButtonCn));
                    sendButton.Click();

                    var reply = driver.FindElement(By.ClassName(replyContainerCn));

                    StringAssert.IsMatch(".*(can't|cannot|can not).*search.*", reply.Text);
                }
                catch (Exception)
                {
                    var ss = driver.GetScreenshot();
                    Console.WriteLine("Error Screenshot: \r\n" + ss.AsBase64EncodedString);
                    throw;
                }
            }
        }

        [TestCase(null, TestName = "Prevent pasting from clipboard (WebApp)")]
        public async Task PreventPastingFromClipBoardTest(object? param)
        {
            
            using (var driver = UndetectedChromeDriver.Create(options, commandTimeout: commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
                {
                    try
                    {
                        authenticate(driver);
                    
                        driver.ExecuteScript("navigator.clipboard.writeText(\"" + longTestString + "\");");
                        var clipboardContent = driver.ExecuteScript("return await navigator.clipboard.readText();");
                    
                        var textArea = driver.FindElement(By.Id(inputId));
                        textArea.SendKeys(Keys.Control + "v");

                        Assert.That(clipboardContent, Is.EqualTo(longTestString));
                        Assert.That(textArea.Text, Is.EqualTo(String.Empty));
                    }
                    catch (Exception)
                    {
                        var ss = driver.GetScreenshot();
                        Console.WriteLine("Error Screenshot: \r\n" + ss.AsBase64EncodedString);
                        throw;
                    }
            }
        }

        [TestCase(null, TestName = "Block File Upload (WebApp)")]
        public async Task BlockFileUploadsTest(object? param)
        {

            using (var driver = UndetectedChromeDriver.Create(options, commandTimeout: commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
            {
                try
                {
                    authenticate(driver);

                    var fileInput = driver.FindElement(By.CssSelector("input[type=file]"));
                    fileInput.SendKeys(bitmapFilePath);
                    await Task.Delay(5000);
                    var errorElement = driver.FindElement(By.ClassName(uploadErrorContainerCn));

                    Assert.That(errorElement.Text, Is.EqualTo("Upload failed"));
                }
                catch (Exception)
                {
                    var ss = driver.GetScreenshot();
                    Console.WriteLine("Error Screenshot: \r\n" + ss.AsBase64EncodedString);
                    throw;
                }
            }
        }

        [TestCase(null, TestName = "Replace prompt (WebApp)")]
        public async Task ReplacePromptTest(object? param)
        {

            using (var driver = UndetectedChromeDriver.Create(options, commandTimeout: commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
            {
                try
                {
                    authenticate(driver);

                    var textArea = driver.FindElement(By.Id(inputId));
                    textArea.SendKeys(longTestString);
                    var sendButton = driver.FindElement(By.ClassName(sendPromptButtonCn));
                    sendButton.Click();

                    var reply = driver.FindElement(By.ClassName(replyContainerCn));
                    Assert.That(reply.Text, Is.EqualTo(policyWarning));
                }
                catch (Exception)
                {
                    var ss = driver.GetScreenshot();
                    Console.WriteLine("Error Screenshot: \r\n" + ss.AsBase64EncodedString);
                    throw;
                }
            }
        }

        private void authenticate(UndetectedChromeDriver? driver)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(300000);

            driver.Navigate().GoToUrl(baseUrl);

            var inputs = driver.FindElements(By.ClassName(loginInputsCn));

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

            var loginButton = driver.FindElement(By.ClassName(loginButtonCn));
            loginButton.Click();
        }



    }
}
