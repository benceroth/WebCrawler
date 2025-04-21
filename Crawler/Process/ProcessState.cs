// <copyright file="ProcessState.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler
{
    /// <summary>
    /// Defines processing state of a request.
    /// </summary>
    public enum ProcessState
    {
        /// <summary>
        /// Not associated with the crawler.
        /// </summary>
        None,

        /// <summary>
        /// Queued to be processed.
        /// </summary>
        Queued,

        /// <summary>
        /// Under processing.
        /// </summary>
        Processing,

        /// <summary>
        /// Processing completed.
        /// </summary>
        Completed,

        /// <summary>
        /// Error thrown during processing.
        /// </summary>
        Error,
    }
}
