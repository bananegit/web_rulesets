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
        private String ci = Environment.GetEnvironmentVariable("CI");

        private String longTestString = "longlonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglong";
        private String searchPrompt = "do a web search for the price of tsla, if you cannot do so answer with \"cannot search\"";
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
        private String thinkContainerCN = "_4d41763";
        private String inputCN = "d96f2d2a";
        private int executionCounter = 0;
        private int retries = 2;


        [OneTimeSetUp]
        public async Task oneTimeInit()
        {
            string traceId = "b0c04ffb";
            await startTrace(traceId);
        }

        [SetUp]
        public void Setup()
        {
            executionCounter = 0;
            options = new ChromeOptions();
            prefs = new Dictionary<string, object>();
            options.AddArguments("--no-sandbox");
            options.AddArguments("--disable-dev-shm-usage");
            //options.AddArguments("--headless=new");
            options.AddArguments("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:137.0) Gecko/20100101 Firefox/137.0");
            var t = (DateTime.UtcNow - new DateTime(1970, 1, 1));

            prefs.Add("profile.content_settings.exceptions.clipboard", new Dictionary<string, object>() { { "[*.]deepseek.com,*", new Dictionary<string, object> { { "last_modfied", (int)t.TotalSeconds * 1000 }, { "setting", 1 } } } });
        }

        private void SetupNoReset()
        {
            options = new ChromeOptions();
            prefs = new Dictionary<string, object>();
            options.AddArguments("--no-sandbox");
            options.AddArguments("--disable-dev-shm-usage");
            //options.AddArguments("--headless=new");
            options.AddArguments("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:137.0) Gecko/20100101 Firefox/137.0");
            var t = (DateTime.UtcNow - new DateTime(1970, 1, 1));

            prefs.Add("profile.content_settings.exceptions.clipboard", new Dictionary<string, object>() { { "[*.]deepseek.com,*", new Dictionary<string, object> { { "last_modfied", (int)t.TotalSeconds * 1000 }, { "setting", 1 } } } });
        }

        private void restoreSCPPolicy()
        {
            Console.WriteLine("restoring scp policy");
            var usPolicyFile = AppContext.BaseDirectory + "ScpPolicy.opg";
            File.Copy(usPolicyFile, "C:\\ProgramData\\Skyhigh\\SCP\\Policy\\Temp\\ScpPolicy.opg", true);
        }

        private void overWriteSCPPolicy()
        {
            Console.WriteLine("overwriting scp policy with us policy");
            var usPolicyFile = AppContext.BaseDirectory + "usPolicy.opg";
            File.Copy(usPolicyFile, "C:\\ProgramData\\Skyhigh\\SCP\\Policy\\Temp\\ScpPolicy.opg", true);
        }

        private void authenticate(UndetectedChromeDriver? driver)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(150000);

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

        private async Task startTrace(string traceId)
        {
            if (traceId.Length > 0)
            {
                string traceUri = "https://api.wgcs.skyhigh.cloud/tracing/v2/session/" + traceId + "/start";
                StringContent content = new StringContent("action=start");
                try
                {
                    using (var httpclient = new HttpClient())
                    {
                        await httpclient.PostAsync(traceUri, content);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to start trace with ID " + traceId);
                    Console.WriteLine(e);
                }
            }
        }

        [TestCase(null, TestName = "Enforce DeepThink Setting (WebApp)")]
        public async Task EnforceDeepThinkSettingsTest(object? param)
        {
            while (executionCounter <= retries)
            {
                using (var driver = UndetectedChromeDriver.Create(options, commandTimeout: commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
                {
                    try
                    {
                        authenticate(driver);

                        var textArea = driver.FindElement(By.ClassName(inputCN));
                        textArea.SendKeys(searchPrompt);
                        var sendButton = driver.FindElement(By.ClassName(sendPromptButtonCn));
                        sendButton.Click();

                        var reply = driver.FindElement(By.ClassName(replyContainerCn));
                        var thinkContainer = driver.FindElement(By.ClassName(thinkContainerCN));

                        StringAssert.IsMatch("Thought for [0-9]* seconds", thinkContainer.Text);
                        return;
                    }
                    catch (Exception)
                    {
                        var ss = driver.GetScreenshot();
                        Console.WriteLine("Error Screenshot: \r\n" + ss.AsBase64EncodedString);
                        if (executionCounter == retries)
                        {
                            Console.WriteLine("Exec counter: " + executionCounter);
                            throw;
                        }
                    }
                    finally
                    {
                        driver.Quit();
                    }
                }
                executionCounter++;
                SetupNoReset();
            }
        }

        [TestCase(null, TestName = "Enforce Search Setting (WebApp)")]
        public async Task EnforceSearchSettingsTest(object? param)
        {
            while (executionCounter <= retries)
            {
                using (var driver = UndetectedChromeDriver.Create(options, commandTimeout: commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
                {
                    try
                    {
                        authenticate(driver);

                        //TODO actually enable search in UI
                        var searchButton = driver.FindElements(By.ClassName("_3172d9f"))[1];
                        searchButton.Click();
                        var textArea = driver.FindElement(By.ClassName(inputCN));
                        textArea.SendKeys(searchPrompt);
                        var sendButton = driver.FindElement(By.ClassName(sendPromptButtonCn));
                        sendButton.Click();

                        var reply = driver.FindElement(By.ClassName(replyContainerCn));

                        StringAssert.IsMatch(".*(can't|cannot|can not).*search.*", reply.Text);
                        return;
                    }
                    catch (Exception)
                    {
                        var ss = driver.GetScreenshot();
                        Console.WriteLine("Error Screenshot: \r\n" + ss.AsBase64EncodedString);
                        if (executionCounter == retries)
                        {
                            Console.WriteLine("Exec counter: " + executionCounter);
                            throw;
                        }
                    }
                    finally
                    {
                        driver.Quit();
                    }
                }
                executionCounter++;
                SetupNoReset();
            }

        }

        [TestCase(null, TestName = "Prevent pasting from clipboard (WebApp)")]
        public async Task PreventPastingFromClipBoardTest(object? param)
        {
            while (executionCounter <= retries)
            {
                using (var driver = UndetectedChromeDriver.Create(options, commandTimeout: commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
                {
                    try
                    {
                        authenticate(driver);

                        driver.ExecuteScript("navigator.clipboard.writeText(\"" + longTestString + "\");");
                        var clipboardContent = driver.ExecuteScript("return await navigator.clipboard.readText();");

                        var textArea = driver.FindElement(By.ClassName(inputCN));
                        textArea.SendKeys(Keys.Control + "v");

                        Assert.That(clipboardContent, Is.EqualTo(longTestString));
                        Assert.That(textArea.Text, Is.EqualTo(String.Empty));
                        return;
                    }
                    catch (Exception)
                    {
                        var ss = driver.GetScreenshot();
                        Console.WriteLine("Error Screenshot: \r\n" + ss.AsBase64EncodedString);
                        if (executionCounter == retries)
                        {
                            Console.WriteLine("Exec counter: " + executionCounter);
                            throw;
                        }
                    }
                    finally
                    {
                        driver.Quit();
                    }
                }
                executionCounter++;
                SetupNoReset();
            }

        }

        [TestCase(null, TestName = "Block File Upload (WebApp)")]
        public async Task BlockFileUploadsTest(object? param)
        {
            while (executionCounter <= retries)
            {
                using (var driver = UndetectedChromeDriver.Create(options, commandTimeout: commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
                {
                    try
                    {
                        authenticate(driver);

                        var fileInput = driver.FindElement(By.CssSelector("input[type=file]"));
                        fileInput.SendKeys(bitmapFilePath);
                        await Task.Delay(10000);
                        var errorElement = driver.FindElement(By.ClassName(uploadErrorContainerCn));

                        Assert.That(errorElement.Text, Is.EqualTo("Upload failed"));
                        return;
                    }
                    catch (Exception)
                    {
                        var ss = driver.GetScreenshot();
                        Console.WriteLine("Error Screenshot: \r\n" + ss.AsBase64EncodedString);
                        if (executionCounter == retries)
                        {
                            Console.WriteLine("Exec counter: " + executionCounter);
                            throw;
                        }
                    }
                    finally
                    {
                        driver.Quit();
                    }
                }
                executionCounter++;
                SetupNoReset();
            }

        }

        [TestCase(null, TestName = "Replace prompt (WebApp)")]
        public async Task ReplacePromptTest(object? param)
        {
            while (executionCounter <= retries)
            {
                using (var driver = UndetectedChromeDriver.Create(options, commandTimeout: commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
                {
                    try
                    {
                        authenticate(driver);

                        var textArea = driver.FindElement(By.ClassName(inputCN));
                        textArea.SendKeys(longTestString);
                        var sendButton = driver.FindElement(By.ClassName(sendPromptButtonCn));
                        sendButton.Click();

                        var reply = driver.FindElement(By.ClassName(replyContainerCn));
                        Assert.That(reply.Text, Is.EqualTo(policyWarning));
                        return;
                    }
                    catch (Exception)
                    {
                        var ss = driver.GetScreenshot();
                        Console.WriteLine("Error Screenshot: \r\n" + ss.AsBase64EncodedString);
                        if (executionCounter == retries)
                        {
                            Console.WriteLine("Exec counter: " + executionCounter);
                            throw;
                        }
                    }
                    finally
                    {
                        driver.Quit();
                    }
                }
                executionCounter++;
                SetupNoReset();
            }
        }

        //greenfield tests
        //this test verifies that the way we copy and paste data into the site would work under normal circumstances
        [TestCase(null, TestName = "_greenField_Prevent pasting from clipboard (WebApp)")]
        public async Task _greenField_PreventPastingFromClipBoardTest(object? param)
        {
            if (!String.IsNullOrEmpty(ci))
            {
                overWriteSCPPolicy();
            }
            using (var driver = UndetectedChromeDriver.Create(options, commandTimeout: commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
            {
                try
                {
                    //driver.Navigate().GoToUrl("https://manytools.org/http-html-text/http-request-headers/");
                    //await Task.Delay(5000);
                    authenticate(driver);

                    driver.ExecuteScript("navigator.clipboard.writeText(\"" + longTestString + "\");");
                    var clipboardContent = driver.ExecuteScript("return await navigator.clipboard.readText();");

                    var textArea = driver.FindElement(By.ClassName(inputCN));
                    textArea.SendKeys(Keys.Control + "v");

                    Assert.That(clipboardContent, Is.EqualTo(longTestString));
                    Assert.That(textArea.Text, Is.EqualTo(longTestString));
                }
                catch (Exception)
                {
                    var ss = driver.GetScreenshot();
                    Console.WriteLine("Error Screenshot: \r\n" + ss.AsBase64EncodedString);
                    if (executionCounter == retries)
                    {
                        Console.WriteLine("Exec counter: " + executionCounter);
                        throw;
                    }
                }
                finally
                {
                    await driver.Manage().Network.StopMonitoring();
                    driver.Quit();
                    if (!String.IsNullOrEmpty(ci))
                    {
                        restoreSCPPolicy();
                        await Task.Delay(10000);
                    }

                }
            }
        }
    }
}
