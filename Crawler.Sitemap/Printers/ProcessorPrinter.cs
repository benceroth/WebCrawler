// <copyright file="ProcessorPrinter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler.Sitemap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <inheritdoc />
    internal class ProcessorPrinter : SectionPrinter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorPrinter"/> class.
        /// </summary>
        /// <param name="requests">Requests to be printed.</param>
        internal ProcessorPrinter(IEnumerable<Request> requests)
            : base(requests
            .Where(x =>
                x.ProcessState == ProcessState.Queued ||
                x.ProcessState == ProcessState.Processing ||
                x.ProcessState == ProcessState.Error)
            .OrderByDescending(x => x.ProcessStateChanged)
            .ThenByDescending(x => x.ProcessState)
            .Take(10))
        {
        }

        /// <inheritdoc />
        protected override void WriteHeader()
        {
            Console.WriteLine("*************   PROCESSOR PRINTER   *************");
        }

        /// <inheritdoc />
        protected override void WriteRequest(Request request)
        {
            Console.WriteLine($"{request.Name} is in {request.ProcessState} state.");
        }

        /// <inheritdoc />
        protected override void SelectColor(Request request)
        {
            ConsoleColor color = ConsoleColor.White;
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
