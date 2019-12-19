// <copyright file="Printer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler.ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines printer logic.
    /// </summary>
    internal abstract class Printer
    {
        private static readonly List<Printer> Printers = new List<Printer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Printer"/> class.
        /// </summary>
        protected Printer()
        {
            Printers.Add(this);
        }

        /// <summary>
        /// Starts printing logic that can be cancelled.
        /// </summary>
        /// <param name="token">CancellationToken.</param>
        internal static void Start(CancellationToken token)
        {
            Task.Run(async () => await Writer(token));
        }

        /// <summary>
        /// Writes a "frame".
        /// </summary>
        protected abstract void Write();

        private static async Task Writer(CancellationToken token)
        {
            var sw = new Stopwatch();
            sw.Start();

            while (!token.IsCancellationRequested)
            {
                long start = sw.ElapsedMilliseconds;

                Console.SetCursorPosition(0, 0);
                foreach (Printer printer in Printers)
                {
                    printer.Write();
                }

                long stop = sw.ElapsedMilliseconds;
                int delay = (int)(start + 33 - stop);
                await Task.Delay(delay < 0 ? 0 : delay);
            }
        }
    }
}
