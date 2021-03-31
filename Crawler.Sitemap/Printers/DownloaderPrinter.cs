// <copyright file="DownloaderPrinter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler.Sitemap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <inheritdoc />
    internal class DownloaderPrinter : SectionPrinter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloaderPrinter"/> class.
        /// </summary>
        /// <param name="requests">Requests to be printed.</param>
        internal DownloaderPrinter(IEnumerable<Request> requests)
            : base(requests
            .Where(
                x => x.DownloadState == DownloadState.Queued ||
                x.DownloadState == DownloadState.Downloading ||
                x.DownloadState == DownloadState.Error)
            .OrderByDescending(x => x.DownloadStateChanged)
            .ThenByDescending(x => x.DownloadState)
            .Take(10))
        {
        }

        /// <inheritdoc />
        protected override void WriteHeader()
        {
            Console.WriteLine("*************   DOWNLOADER PRINTER   *************");
        }

        /// <inheritdoc />
        protected override void WriteRequest(Request request)
        {
            Console.WriteLine($"{request.Name} is in {request.DownloadState} state.");
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

            Console.ForegroundColor = color;
        }
    }
}
