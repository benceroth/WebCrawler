// <copyright file="SectionPrinter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler.ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <inheritdoc />
    internal abstract class SectionPrinter : Printer
    {
        private readonly IEnumerable<Request> requests;

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

            foreach (var request in this.requests)
            {
                this.ClearLine();
                this.SelectColor(request);
                this.WriteRequest(request);
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void ClearLine()
        {
            Console.WriteLine(new string(' ', Console.BufferWidth));
            Console.CursorTop--;
        }
    }
}
