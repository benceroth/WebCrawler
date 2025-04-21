// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler.ConsoleApp
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point of program.
        /// </summary>
        public static void Main()
        {
            Start();
            Console.Read();
        }

        /// <summary>
        /// Starts a test crawler.
        /// </summary>
        private static void Start()
        {
            var source = new CancellationTokenSource();
            var requests = new ConcurrentBag<Request>();

            var crawler = CreateCrawler();
            crawler.OnProcessCompleted += Crawler_OnProcessCompleted;
            crawler.OnDownloadCompleted += Crawler_OnDownloadCompleted;

            CreatePrinters(requests, source.Token);
            CreateRequests(crawler, requests);
            crawler.Start(source.Token);
        }

        private static void Crawler_OnProcessCompleted(object sender, ProcessCompletedEventArgs e)
        {
            Console.Beep(1000, 100);
        }

        private static void Crawler_OnDownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            Console.Beep(5000, 100);
        }

        /// <summary>
        /// Creates a new crawler with linking.
        /// </summary>
        /// <returns>Crawler.</returns>
        private static Crawler CreateCrawler()
        {
            return new Crawler()
            {
                MaxParallelProcess = 3,
                MaxParallelDownload = 2,
                FileProcessor = new FakeProcessor(),
            }.LinkDownloadWithProcess();
        }

        /// <summary>
        /// Create printers and starts printing process that can be cancelled.
        /// </summary>
        /// <param name="requests">Requests to be printed.</param>
        /// <param name="token">CancellationToken.</param>
        private static void CreatePrinters(IEnumerable<Request> requests, CancellationToken token)
        {
            new DownloaderPrinter(requests);
            new ProcessorPrinter(requests);
            Printer.Start(token);
        }

        /// <summary>
        /// Create enqueued requests on crawler and printers.
        /// </summary>
        /// <param name="crawler">Crawler.</param>
        /// <param name="requests">ConcurrentBag storing requests for printers.</param>
        private static void CreateRequests(Crawler crawler, ConcurrentBag<Request> requests)
        {
            Parallel.For(0, 12, i =>
            {
                Request request = new Request()
                {
                    Name = "Google_" + i,
                    FileName = "Google_" + i,
                    Url = "https://bing.com",
                };

                requests.Add(request);
                crawler.EnqueueDownloadRequest(request);
            });
        }
    }
}
