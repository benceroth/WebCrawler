// <copyright file="Processor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a queue based file processor abstraction.
    /// </summary>
    internal class Processor
    {
        private readonly BlockingCollection<Request> queue = new BlockingCollection<Request>();

        private SemaphoreSlim semaphore;

        /// <summary>
        /// Event that fires on process completion.
        /// </summary>
        internal event EventHandler<ProcessCompletedEventArgs> OnCompleted;

        /// <summary>
        /// Gets or sets maximum amount of parallel processes.
        /// </summary>
        internal int MaxParallelProcess { get; set; } = 5;

        /// <summary>
        /// Gets or sets file processor.
        /// </summary>
        internal IFileProcessor FileProcessor { get; set; }

        /// <summary>
        /// Starts queue based process until cancellation.
        /// </summary>
        /// <param name="token">CancellationToken.</param>
        internal void Start(CancellationToken token)
        {
            this.semaphore = new SemaphoreSlim(this.MaxParallelProcess);
            Task.Run(() => this.Dequeuer(token));
        }

        /// <summary>
        /// Enqueues a request.
        /// </summary>
        /// <param name="request">Request.</param>
        internal void Enqueue(Request request)
        {
            request.ProcessState = ProcessState.Queued;
            this.queue.Add(request);
        }

        private void Dequeuer(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var request = this.queue.Take();
                Task.Run(async () => await this.Process(request)).ConfigureAwait(false);
            }
        }

        private async Task Process(Request request)
        {
            await this.semaphore.WaitAsync();
            request.ProcessState = ProcessState.Processing;

            try
            {
                this.Move(request, "./Processing/" + request.FileName);
                await this.FileProcessor.Process(request);
                request.ProcessState = ProcessState.Completed;
                this.Move(request, "./Processed/" + request.FileName);

                this.OnCompleted?.Invoke(this, new ProcessCompletedEventArgs(request));
                this.semaphore.Release();
            }
            catch (Exception exception)
            {
                this.semaphore.Release();
                request.ProcessState = ProcessState.Error;
                throw exception;
            }
        }

        private void Move(Request request, string to)
        {
            if (File.Exists(to))
            {
                File.Delete(to);
            }

            var sourcePath = request.Path;
            request.Path = to;
            File.Move(sourcePath, request.Path);
        }
    }
}
