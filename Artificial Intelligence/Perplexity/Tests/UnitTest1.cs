using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using SeleniumUndetectedChromeDriver;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Gmail;
using Google.Apis.Gmail.v1;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Internal;

namespace PerplexityTests
{
    public class Tests
    {
        private String baseUrl = "https://www.perplexity.ai";
        private String username = Environment.GetEnvironmentVariable("ppxUsername");
        private String password = Environment.GetEnvironmentVariable("ppxPassword");
        private String gmailPAT = Environment.GetEnvironmentVariable("gmailPAT");
        private String gmailRefreshToken = Environment.GetEnvironmentVariable("gmailRefreshToken");
        private String ci = Environment.GetEnvironmentVariable("CI");
        //private String ci = "";

        private String longTestString = "longlonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglonglong";
        private String searchPrompt = "do a web search for the price of tsla, if you cannot do so answer with \"cannot search\"";
        private String policyWarning = "\"Your last prompt was blocked by Web Policy for exceeding the maximum prompt length of 100 characters\"";
        private String bitmapFilePath = AppContext.BaseDirectory + "BitmapFile.bmp";

        private ChromeOptions options = new ChromeOptions();
        private Dictionary<string, object> prefs = new Dictionary<string, object>();
        private TimeSpan commandTimeout = TimeSpan.FromSeconds(600);


        //UI element classnames / ids
        private String loginButton1CN = "gap-sm";
        private String uploadErrorContainerCn = "tabler-icon-alert-circle";
        private String thinkContainerCN = "_19db599";
        private String inputId = "ask-input";
        private String replyContainerId = "markdown-content-0";
        private int executionCounter = 0;
        private int retries = 0;

        Regex signinCodePattern = new Regex("[\r\n]([a-z0-9]*-[a-z0-9]*)[\r\n]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        GmailService service;


        [OneTimeSetUp]
        public async Task oneTimeInit()
        {
            string traceId = "";
            await startTrace(traceId);

            CredStore cs = new CredStore();
            ClientSecrets secrets = GoogleClientSecrets.FromFile("client_secret_485623738455-tpaitl3hu5v2ikefdhdfo7l583fujlet.apps.googleusercontent.com.json").Secrets;

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = new[] { GmailService.Scope.GmailReadonly },
                DataStore = cs,
            });

            var token = new TokenResponse
            {
                AccessToken = gmailPAT,
                RefreshToken = gmailRefreshToken
            };

            UserCredential cred = new UserCredential(flow, username, token);

            service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = cred,
                ApplicationName = "automated tests"
            });
        }

        [OneTimeTearDown]
        public void oneTimeTearDown()
        {
            if (service != null) { service.Dispose(); }
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

            prefs.Add("profile.content_settings.exceptions.clipboard", new Dictionary<string, object>() { { "[*.]perplexity.ai,*", new Dictionary<string, object> { { "last_modfied", (int)t.TotalSeconds * 1000 }, { "setting", 1 } } } });
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

            prefs.Add("profile.content_settings.exceptions.clipboard", new Dictionary<string, object>() { { "[*.]perplexity.ai,*", new Dictionary<string, object> { { "last_modfied", (int)t.TotalSeconds * 1000 }, { "setting", 1 } } } });
        }

        private async Task restoreSCPPolicy()
        {
            Console.WriteLine("restoring scp policy");
            var usPolicyFile = AppContext.BaseDirectory + "ScpPolicy.opg";
            File.Copy(usPolicyFile, "C:\\ProgramData\\Skyhigh\\SCP\\Policy\\Temp\\ScpPolicy.opg", true);
            await Task.Delay(5000);
        }

        private void overWriteSCPPolicy()
        {
            Console.WriteLine("overwriting scp policy with us policy");
            var usPolicyFile = AppContext.BaseDirectory + "usPolicy.opg";
            File.Copy(usPolicyFile, "C:\\ProgramData\\Skyhigh\\SCP\\Policy\\Temp\\ScpPolicy.opg", true);
        }

        private async Task authenticate(UndetectedChromeDriver? driver)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(150000);

            driver.Navigate().GoToUrl(baseUrl);

            //confirm cookies
            await Task.Delay(500);
            var buttons = driver.FindElements(By.TagName("button"));
            var cookieConsentButton = buttons.FirstOrDefault((e) => { return e.Text.Equals("Necessary Cookies"); });
            if (cookieConsentButton != null) { cookieConsentButton.Click(); }

            var emailInput = driver.FindElement(By.CssSelector("input[type=email]"));
            emailInput.SendKeys(username);
            var loginbuttons = driver.FindElements(By.CssSelector("svg[data-icon='chevron-right']"));
            loginbuttons[0].Click();

            //wait a bit for email to arrive
            await Task.Delay(5000);

            String signinCode = await getSigninCode();
            var codeInput = driver.FindElement(By.TagName("input"));
            codeInput.SendKeys(signinCode);
            buttons = driver.FindElements(By.TagName("button"));
            buttons.FirstOrDefault((e) => { return e.Text.Equals("Continue"); }).Click();

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


        [TestCase(null, TestName = "Prevent pasting from clipboard (WebApp)")]
        public async Task PreventPastingFromClipBoardTest(object? param)
        {
            while (executionCounter <= retries)
            {
                using (var driver = UndetectedChromeDriver.Create(options, commandTimeout: commandTimeout, prefs: prefs, driverExecutablePath: await new ChromeDriverInstaller().Auto()))
                {
                    try
                    {
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(150000);
                        driver.Navigate().GoToUrl(baseUrl);
                        var textArea = driver.FindElement(By.Id(inputId));
                        textArea.Click();

                        var wt = driver.ExecuteScript("navigator.clipboard.writeText(\"" + longTestString + "\");");
                        var clipboardContent = driver.ExecuteScript("return await navigator.clipboard.readText();");
                        textArea.SendKeys(Keys.Control + "v");


                        Assert.That(clipboardContent, Is.EqualTo(longTestString));
                        Assert.That(textArea.Text, Is.EqualTo(String.Empty));
                        return;
                    }
                    catch (Exception)
                    {
                        await Task.Delay(500);
                        var ss = driver.GetScreenshot();
                        driver.SwitchTo().Window(driver.CurrentWindowHandle);
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
                        await authenticate(driver);

                        var fileInput = driver.FindElement(By.CssSelector("input[type=file]"));
                        fileInput.SendKeys(bitmapFilePath);

                        //the div with the upload failed text has no proper identifier, using the error icon instead as it´s unique
                        var errorElement = driver.FindElement(By.ClassName(uploadErrorContainerCn));
                        return;
                    }
                    catch (Exception)
                    {
                        await Task.Delay(500);
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
                        await authenticate(driver);

                        var textArea = driver.FindElement(By.Id(inputId));
                        textArea.SendKeys(longTestString);
                        textArea.SendKeys(Keys.Enter);

                        var reply = driver.FindElement(By.Id(replyContainerId));
                        Assert.That(reply.Text, Is.EqualTo(policyWarning));
                        return;
                    }
                    catch (Exception)
                    {
                        await Task.Delay(500);
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
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(150000);
                    driver.Navigate().GoToUrl(baseUrl);
                    var textArea = driver.FindElement(By.Id(inputId));
                    textArea.Click();
                    driver.ExecuteScript("navigator.clipboard.writeText(\"" + longTestString + "\");");
                    var clipboardContent = driver.ExecuteScript("return await navigator.clipboard.readText();");

                    textArea.SendKeys(Keys.Control + "v");

                    Assert.That(clipboardContent, Is.EqualTo(longTestString));
                    Assert.That(textArea.Text, Is.EqualTo(longTestString));
                }
                catch (Exception)
                {
                    await Task.Delay(500);
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
                        await restoreSCPPolicy();
                    }

                }
            }
        }


        private async Task<String> getSigninCode()
        {
            var emailRequest = service.Users.Messages.List("me");
            emailRequest.MaxResults = 20;
            var emailIds = await emailRequest.ExecuteAsync();
            String body = "";
            foreach (var message in emailIds.Messages)
            {
                var email = service.Users.Messages.Get("me", message.Id).Execute();
                var from = email.Payload.Headers.FirstOrDefault((e) => { return e.Name.Equals("From"); });
                var subject = email.Payload.Headers.FirstOrDefault((e) => { return e.Name.Equals("Subject"); });
                if (subject is not null && from is not null && from.Value.Equals("Perplexity <team@mail.perplexity.ai>") && subject.Value.Equals("Sign in to Perplexity"))
                {
                    body = Encoding.UTF8.GetString(Base64UrlEncoder.DecodeBytes(email.Payload.Parts[0].Body.Data));
                    break;
                }
            }

            var matches = signinCodePattern.Matches(body);
            return matches.First().Value;
        }
    }
}
