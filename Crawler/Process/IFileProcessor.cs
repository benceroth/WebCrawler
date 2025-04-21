// <copyright file="IFileProcessor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines interface for file processor.
    /// </summary>
    public interface IFileProcessor
    {
        /// <summary>
        /// Processes a request.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>Completed task.</returns>
        Task Process(Request request);
    }
}
