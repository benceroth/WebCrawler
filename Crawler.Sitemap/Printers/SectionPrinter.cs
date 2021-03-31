// <copyright file="SectionPrinter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler.Sitemap
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <inheritdoc />
    internal abstract class SectionPrinter : Printer
    {
        protected readonly IEnumerable<Request> requests;

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionPrinter"/> class.
        /// </summary>
        /// <param name="requests">Requests to be printed.</param>
        protected SectionPrinter(IEnumerable<Request> requests)
        {
            this.requests = requests;
        }

        /// <summary>
        /// Write printer header.
        /// </summary>
        protected abstract void WriteHeader();

        /// <summary>
        /// Writes a request.
        /// </summary>
        /// <param name="request">Request.</param>
        protected abstract void WriteRequest(Request request);

        /// <summary>
        /// Sets color for corresponding request.
        /// </summary>
        /// <param name="request">Request.</param>
        protected abstract void SelectColor(Request request);

        /// <inheritdoc />
        protected override void Write()
        {
            this.ClearLine();
            this.WriteHeader();

            int i = 0;
            foreach (var request in this.requests)
            {
                this.ClearLine();
                this.SelectColor(request);
                this.WriteRequest(request);

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

        protected void ClearLine()
        {
            Console.WriteLine(new string(' ', Console.BufferWidth));
            Console.CursorTop--;
        }
    }
}
