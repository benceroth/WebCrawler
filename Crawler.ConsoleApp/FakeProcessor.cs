// <copyright file="FakeProcessor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler.ConsoleApp
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Fake file processor.
    /// </summary>
    public class FakeProcessor : IFileProcessor
    {
        private static readonly Random Random = new Random();

        /// <inheritdoc />
        public async Task Process(Request request)
        {
            await Task.Delay(Random.Next(500, 5000));
        }
    }
}
