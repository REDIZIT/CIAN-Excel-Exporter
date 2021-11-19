using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using UnityEngine;

namespace InApp
{
    public class Worker : IDisposable
    {
        public WorkerState state = new WorkerState();

        private List<string> urls = new List<string>();

        private Thread thread;
        private string folderPath;
        private HttpClient client;

        private readonly Pathes pathes;
        private readonly UrlsCreator urlsCreator;

        /// <summary>Delay between requests (in seconds)</summary>
        public const int DELAY_SECONDS = 5;

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
        }
        public void Stop()
        {
            thread.Abort();
            state = new WorkerState();
        }

        public void HandleUrls()
        {
            try
            {
                state = new WorkerState();
                state.type = WorkerState.Type.Downloading;
                state.urlsCount = urls.Count;
                state.startTime = DateTime.Now;

                for (int i = 1; i <= urls.Count; i++)
                {
                    state.currentUrlIndex = i;

                    string url = urls[i - 1];

                    // Export url
                    // https://spb.cian.ru//?deal_type=sale&district%5B0%5D=747&engine_version=2&object_type%5B0%5D=1&offer_type=flat&room7=1&room9=1&totime=864000
                    url = url.Replace("cat.php", "export/xls/offers");

                    state.type = WorkerState.Type.Downloading;
                    state.awaitTimeLeft = DELAY_SECONDS;

                    //client.diwn(url, folderPath + "/" + i + ".xlsx");
                    using (HttpResponseMessage resp = client.GetAsync(url).Result)
                    {
                        if (resp.IsSuccessStatusCode)
                        {
                            File.WriteAllBytes(folderPath + "/" + i + ".xlsx", resp.Content.ReadAsByteArrayAsync().Result);
                        }
                        else
                        {
                            throw new Exception($"Webpage ({url}) can't be downloaded. Response code is {resp.StatusCode}");
                        }
                    }


                    state.type = WorkerState.Type.Awaiting;

                    while (state.awaitTimeLeft > 0)
                    {
                        Thread.Sleep(1000);
                        state.awaitTimeLeft--;
                    }
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
            
        }

        public void Dispose()
        {
            thread?.Abort();
        }
    }

    public class WorkerState
    {
        public Type type;
        public int currentUrlIndex, urlsCount;
        public int awaitTimeLeft;
        public DateTime startTime;

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