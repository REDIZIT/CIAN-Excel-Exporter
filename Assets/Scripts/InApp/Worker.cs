using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

namespace InApp
{
    public class Worker : IDisposable
    {
        public int UrlsCount => urls.Count;

        public WorkerState state = new WorkerState();

        private List<string> urls = new List<string>();

        private Thread thread;
        private string folderPath;

        private readonly Pathes pathes;
        private readonly UrlsCreator urlsCreator;

        /// <summary>Delay between requests (in seconds)</summary>
        public const int DELAY_SECONDS = 10;

        public Worker(Pathes pathes)
        {
            this.pathes = pathes;
            urlsCreator = new UrlsCreator(pathes);
        }

        public void Start(string folderPath)
        {
            this.folderPath = folderPath;
            if (Directory.Exists(folderPath) == false)
                throw new System.Exception($"Can't start working because there is no folder at: '{folderPath}'");

            urls = urlsCreator.Create();

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

                WebClient c = new WebClient();

                for (int i = 1; i <= urls.Count; i++)
                {
                    state.currentUrlIndex = i;

                    string url = urls[i - 1];

                    // Export url
                    // https://spb.cian.ru//?deal_type=sale&district%5B0%5D=747&engine_version=2&object_type%5B0%5D=1&offer_type=flat&room7=1&room9=1&totime=864000
                    url = url.Replace("cat.php", "export/xls/offers");

                    state.type = WorkerState.Type.Downloading;
                    state.awaitTimeLeft = DELAY_SECONDS;

                    c.DownloadFile(url, folderPath + "/" + i + ".xlsx");

                    state.type = WorkerState.Type.Awaiting;

                    while (state.awaitTimeLeft > 0)
                    {
                        Thread.Sleep(1000);
                        state.awaitTimeLeft--;
                    }
                }

                state.type = WorkerState.Type.Done;
            }
            catch
            {
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