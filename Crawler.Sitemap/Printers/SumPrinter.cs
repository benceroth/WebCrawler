// <copyright file="SumPrinter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler.Sitemap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc />
    internal class SumPrinter : SectionPrinter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SumPrinter"/> class.
        /// </summary>
        /// <param name="requests">Requests to be printed.</param>
        internal SumPrinter(IEnumerable<Request> requests)
            : base(requests)
        {
        }

        /// <inheritdoc />
        protected override void WriteHeader()
        {
            Console.WriteLine("*************   SUM PRINTER   *************");
        }

        /// <inheritdoc />
        protected override void Write()
        {
            this.ClearLine();
            this.WriteHeader();
            var groups = this.requests
                .GroupBy(x => new { x.DownloadState, x.ProcessState })
                .OrderBy(x => x.Key.DownloadState)
                .ThenBy(x => x.Key.ProcessState);

            int i = 0;
            foreach (var group in groups)
            {
                this.ClearLine();
                this.SelectColor(group.First());
                if (group.Key.ProcessState == ProcessState.None)
                {
                    Console.WriteLine($"There are {group.Count()} requests with {group.Key.DownloadState} DownloadState");
                }
                else
                {
                    Console.WriteLine($"There are {group.Count()} requests with {group.Key.ProcessState} ProcessState");
                }

                if (++i >= 10)
                {
                    break;
                }
            }

            while (++i < 11)
            {
                Console.WriteLine();
                this.ClearLine();
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <inheritdoc />
        protected override void WriteRequest(Request request)
        {
        }

        /// <inheritdoc />
        protected override void SelectColor(Request request)
        {
            ConsoleColor color = ConsoleColor.White;
            switch (request.DownloadState)
            {
                case DownloadState.Queued:
                    color = ConsoleColor.Yellow;
                    break;
                case DownloadState.Downloading:
                    color = ConsoleColor.Blue;
                    break;
                case DownloadState.Completed:
                    color = ConsoleColor.Green;
                    break;
                case DownloadState.Error:
                    color = ConsoleColor.Red;
                    break;
            }

            switch (request.ProcessState)
            {
                case ProcessState.Queued:
                    color = ConsoleColor.Yellow;
                    break;
                case ProcessState.Processing:
                    color = ConsoleColor.Blue;
                    break;
                case ProcessState.Completed:
                    color = ConsoleColor.Green;
                    break;
                case ProcessState.Error:
                    color = ConsoleColor.Red;
                    break;
            }

            Console.ForegroundColor = color;
        }
    }
}
