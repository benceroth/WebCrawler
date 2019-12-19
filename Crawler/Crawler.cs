// <copyright file="Crawler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Defines crawler as a facade.
    /// </summary>
    public partial class Crawler
    {
        private readonly Processor processor;
        private readonly Downloader downloader;

        static Crawler()
        {
            Directory.CreateDirectory("Downloaded");
            Directory.CreateDirectory("Processed");
            Directory.CreateDirectory("Processing");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crawler"/> class.
        /// </summary>
        public Crawler()
        {
            this.processor = new Processor();
            this.downloader = new Downloader();

            this.processor.OnCompleted += (sender, args) => this.OnProcessCompleted?.Invoke(this, args);
            this.downloader.OnCompleted += (sender, args) => this.OnDownloadCompleted?.Invoke(this, args);
        }

        /// <summary>
        /// Event that fires on request processing completion.
        /// </summary>
        public event EventHandler<ProcessCompletedEventArgs> OnProcessCompleted;

        /// <summary>
        /// Event that fires on request download completion.
        /// </summary>
        public event EventHandler<DownloadCompletedEventArgs> OnDownloadCompleted;

        /// <summary>
        /// Gets or sets crawler run flags.
        /// </summary>
        public RunFlags RunFlags { get; set; } = RunFlags.Process | RunFlags.Download;

        /// <summary>
        /// Gets or sets maximum amount of parallel downloads.
        /// </summary>
        public int MaxParallelDownload
        {
            get => this.downloader.MaxParallelDownload;
            set => this.downloader.MaxParallelDownload = value;
        }

        /// <summary>
        /// Gets or sets maximum amount of parallel processes.
        /// </summary>
        public int MaxParallelProcess
        {
            get => this.processor.MaxParallelProcess;
            set => this.processor.MaxParallelProcess = value;
        }

        /// <summary>
        /// Gets or sets browser agent for downloading.
        /// </summary>
        public string Agent
        {
            get => this.downloader.Agent;
            set => this.downloader.Agent = value;
        }

        /// <summary>
        /// Gets or sets file processor.
        /// </summary>
        public IFileProcessor FileProcessor
        {
            get => this.processor.FileProcessor;
            set => this.processor.FileProcessor = value;
        }

        /// <summary>
        /// Enqueues a request to download.
        /// </summary>
        /// <param name="request">Request to be downloaded.</param>
        /// <returns>Self.</returns>
        public Crawler EnqueueDownloadRequest(Request request)
        {
            this.downloader.Enqueue(request);
            return this;
        }

        /// <summary>
        /// Enqueues a request to process.
        /// </summary>
        /// <param name="request">Request to be processed.</param>
        /// <returns>Self.</returns>
        public Crawler EnqueueProcessRequest(Request request)
        {
            this.processor.Enqueue(request);
            return this;
        }

        /// <summary>
        /// Links download with processing, so it will automatically trigger processing after OnDownloadCompleted event.
        /// </summary>
        /// <returns>Self.</returns>
        public Crawler LinkDownloadWithProcess()
        {
            this.downloader.OnCompleted += (sender, args) => this.processor.Enqueue(args.Request);
            return this;
        }

        /// <summary>
        /// Starts crawler that can be cancelled.
        /// </summary>
        /// <param name="token">CancellationToken.</param>
        /// <returns>Self.</returns>
        public Crawler Start(CancellationToken token)
        {
            if (this.RunFlags.HasFlag(RunFlags.Process))
            {
                this.processor.Start(token);
            }

            if (this.RunFlags.HasFlag(RunFlags.Download))
            {
                this.downloader.Start(token);
            }

            return this;
        }
    }
}
