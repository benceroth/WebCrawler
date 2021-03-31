// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler.Sitemap
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
        }

        /// <summary>
        /// Starts a test crawler.
        /// </summary>
        private static void Start()
        {
            var source = new CancellationTokenSource();
            var requests = new ConcurrentBag<Request<SitemapItem>>();

            var crawler = CreateCrawler(requests);
            crawler.OnProcessCompleted += Crawler_OnProcessCompleted;
            crawler.OnDownloadCompleted += Crawler_OnDownloadCompleted;

            CreateRequests(crawler, requests);
            CreatePrinters(requests, source.Token);
            crawler.Start(source.Token);

            Console.Read();
            source.Cancel();
            (crawler.FileProcessor as FileProcessor).Finish(true);
            Console.WriteLine("Done!");
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
        private static Crawler CreateCrawler(IProducerConsumerCollection<Request<SitemapItem>> requests)
        {
            var crawler = new Crawler()
            {
                MaxParallelProcess = 10,
                MaxParallelDownload = 3,
                Agent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0",
            }.LinkDownloadWithProcess();
            crawler.FileProcessor = new FileProcessor(crawler, requests);
            return crawler;
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
            new SumPrinter(requests);
            Printer.Start(token);
        }

        /// <summary>
        /// Create enqueued requests on crawler and printers.
        /// </summary>
        /// <param name="crawler">Crawler.</param>
        /// <param name="requests">ConcurrentBag storing requests for printers.</param>
        private static void CreateRequests(Crawler crawler, ConcurrentBag<Request<SitemapItem>> requests)
        {
            Uri uri = null;
            while (!Uri.TryCreate(Console.ReadLine(), UriKind.Absolute, out uri));

            var url = uri.ToString();
            url = url.Remove(url.Length - 1);
            Request<SitemapItem> request = new Request<SitemapItem>()
            {
                Name = url,
                FileName = "Main",
                Url = url,
                ItemDataBound = new SitemapItem { Priority = 1 },
            };

            requests.Add(request);
            crawler.EnqueueDownloadRequest(request);
        }
    }
}
