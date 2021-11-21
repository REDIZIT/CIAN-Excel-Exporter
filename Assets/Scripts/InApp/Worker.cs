using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Opera;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace InApp
{
    public class Worker : IDisposable
    {
        public WorkerState state = new WorkerState();

        private List<string> urls = new List<string>();

        private Thread thread, watcherThread;
        private string folderPath;
        private HttpClient client;

        private readonly Pathes pathes;
        private readonly UrlsCreator urlsCreator;

        /// <summary>Delay between requests (in seconds)</summary>
        public const int DELAY_SECONDS = 5;
        /// <summary>Count of urls to check before have a chill</summary>
        public const int CHILL_URLS_COUNT = 100;
        /// <summary>Chill time in seconds</summary>
        public const int CHILL_DELAY = 60;

        public Worker(Pathes pathes)
        {
            this.pathes = pathes;
            urlsCreator = new UrlsCreator(pathes);
        }

        public void Start(string folderPath)
        {
            this.folderPath = folderPath;
            if (Directory.Exists(folderPath) == false)
                throw new Exception($"Can't start working because there is no folder at: '{folderPath}'");

            urls = urlsCreator.Create();
            
            var handler = new HttpClientHandler();
            if (File.Exists(pathes.Data + "/noproxy.txt") == false)
            {
                handler.Proxy = new WebProxy
                {
                    Address = new Uri("http://proxy.ko.wan:808"),
                    BypassProxyOnLocal = false
                };
            }
            client = new HttpClient(handler);

            thread = new Thread(new ThreadStart(HandleUrls));
            thread.Start();

            UnityEngine.Debug.Log(pathes.TempDownload);
        }
        public void Stop()
        {
            thread.Abort();
            state = new WorkerState();
        }

        private void MoveDownloadedFile()
        {
            string targetFileName = folderPath + "/" + Directory.GetFiles(folderPath).Length + ".xlsx";
            string sourceFileName = Directory.GetFiles(pathes.TempDownload)[0];

            File.Move(sourceFileName, targetFileName);
        }

        public void HandleUrls()
        {
            ChromeDriver driver = null;
            try
            {
                state = new WorkerState
                {
                    type = WorkerState.Type.Downloading,
                    urlsCount = urls.Count,
                    startTime = DateTime.Now
                };

                driver = GetDriver();
                System.Random rnd = new System.Random();

                for (int i = 1; i <= urls.Count; i++)
                {
                    state.currentUrlIndex = i - 1;

                    string url = urls[i - 1];

                    // Export url
                    // https://spb.cian.ru/export/xls/offers/?deal_type=sale&district%5B0%5D=747&engine_version=2&object_type%5B0%5D=1&offer_type=flat&room7=1&room9=1&totime=864000
                    //url = url.Replace("cat.php", "export/xls/offers");

                    if (i % CHILL_URLS_COUNT == 0 && i > 0)
                    {
                        state.type = WorkerState.Type.Awaiting;
                        state.awaitTimeLeft = CHILL_DELAY;
                        while (state.awaitTimeLeft > 0)
                        {
                            Thread.Sleep(1000);
                            state.awaitTimeLeft--;
                        }
                    }

                    state.type = WorkerState.Type.Downloading;

                    state.awaitTimeLeft = DELAY_SECONDS;

                    driver.Navigate().GoToUrl(url);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(driver.PageSource);

                    var elemets = driver.FindElementsByClassName("_93444fe79c--light--366j7");
                    IWebElement button = null;

                    while (button == null)
                    {
                        button = elemets.FirstOrDefault(e => e.Text == "Сохранить файл в Excel");
                        if (button == null)
                        {
                            Thread.Sleep(1000);
                        }
                    }

                    button.Click();

                    while(Directory.GetFiles(pathes.TempDownload).Length == 0)
                    {
                        Thread.Sleep(50);
                    }
                    MoveDownloadedFile();
                }

                state.type = WorkerState.Type.Done;
            }
            catch (Exception err)
            {
                if (err is ThreadAbortException)
                {
                    state.type = WorkerState.Type.Idle;
                    return;
                }
                state.type = WorkerState.Type.Error;
                throw;
            }
            finally
            {
                state.finishTime = DateTime.Now;
                if (driver != null)
                {
                    driver.Close();
                }
            }
        }

        public void Dispose()
        {
            thread?.Abort();
            watcherThread?.Abort();
        }

        private byte[] Download(string url)
        {
            using HttpResponseMessage resp = client.GetAsync(url).Result;
            if (resp.IsSuccessStatusCode)
            {
                return resp.Content.ReadAsByteArrayAsync().Result;
            }
            else
            {
                throw new Exception($"Webpage ({url}) can't be downloaded. Response code is {resp.StatusCode}");
            }
        }

        private ChromeDriver GetDriver()
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(pathes.DataPath + "/StreamingAssets");
            service.HideCommandPromptWindow = true;
            service.EnableVerboseLogging = true;

            var chromeOptions = new ChromeOptions();

            chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
            chromeOptions.AddArgument("--headless");

            chromeOptions.AddUserProfilePreference("download.default_directory", pathes.TempDownload);
            chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
            chromeOptions.AddUserProfilePreference("download.directory_upgrade", true);
            chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);
            chromeOptions.AddUserProfilePreference("intl.accept_languages", "ru");
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");

            var driver = new ChromeDriver(service, chromeOptions);

            driver.Manage().Window.Maximize();

            driver.ExecuteScript(@"
Object.defineProperty(navigator, 'languages', {
        get: function() {
                return ['en-US', 'en', 'es'];
            }
        });

    // Overwrite the `plugins` property to use a custom getter.
    Object.defineProperty(navigator, 'plugins', {
        get: () => [1, 2, 3, 4, 5],
    });

    // Pass the Webdriver test
    Object.defineProperty(navigator, 'webdriver', {
      get: () => false,
    });
");
            string startUrl = "https://spb.cian.ru";
            driver.Navigate().GoToUrl(startUrl);

            //Thread.Sleep(5000);

            return driver;
        }
    }

    public class WorkerState
    {
        public Type type;
        public int currentUrlIndex, urlsCount;
        public int awaitTimeLeft;
        public DateTime startTime, finishTime;

        public enum Type
        {
            Idle,
            Downloading,
            Awaiting,
            Error,
            Done
        }
    }
}