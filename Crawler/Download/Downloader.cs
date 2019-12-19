// <copyright file="Downloader.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a queue based file downloader abstraction.
    /// </summary>
    internal class Downloader
    {
        private readonly BlockingCollection<Request> queue = new BlockingCollection<Request>();

        private SemaphoreSlim semaphore;

        /// <summary>
        /// Event that fires on download completion.
        /// </summary>
        internal event EventHandler<DownloadCompletedEventArgs> OnCompleted;

        /// <summary>
        /// Gets or sets maximum amount of parallel downloads.
        /// </summary>
        internal int MaxParallelDownload { get; set; } = 5;

        /// <summary>
        /// Gets or sets browser agent for downloading.
        /// </summary>
        internal string Agent { get; set; } = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";

        /// <summary>
        /// Starts queue based process until cancellation.
        /// </summary>
        /// <param name="token">CancellationToken.</param>
        internal void Start(CancellationToken token)
        {
            this.semaphore = new SemaphoreSlim(this.MaxParallelDownload);
            Task.Run(() => this.Dequeuer(token));
        }

        /// <summary>
        /// Enqueues a request.
        /// </summary>
        /// <param name="request">Request.</param>
        internal void Enqueue(Request request)
        {
            request.DownloadState = DownloadState.Queued;
            this.queue.Add(request);
        }

        private void Dequeuer(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var request = this.queue.Take();
                Task.Run(async () => await this.Download(request)).ConfigureAwait(false);
            }
        }

        private async Task Download(Request request)
        {
            var task = this.semaphore.WaitAsync();

            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, this.Agent);
            client.DownloadFileCompleted += (sender, e) => this.Client_DownloadFileCompleted(request, e);

            await task;
            request.Path = "./Downloaded/" + request.Name;
            request.DownloadState = DownloadState.Downloading;

            try
            {
                await client.DownloadFileTaskAsync(request.Url, request.Path);
            }
            catch (Exception exception)
            {
                this.semaphore.Release();
                request.DownloadState = DownloadState.Error;
                throw exception;
            }
        }

        private void Client_DownloadFileCompleted(Request request, AsyncCompletedEventArgs e)
        {
            this.semaphore.Release();

            if (e.Cancelled || e.Error != null)
            {
                request.DownloadState = DownloadState.Error;
            }
            else
            {
                request.DownloadState = DownloadState.Completed;
                this.OnCompleted?.Invoke(this, new DownloadCompletedEventArgs(request));
            }
        }
    }
}
