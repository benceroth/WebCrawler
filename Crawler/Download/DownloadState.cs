// <copyright file="DownloadState.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines download state of a request.
    /// </summary>
    public enum DownloadState
    {
        /// <summary>
        /// Not associated with the crawler.
        /// </summary>
        None,

        /// <summary>
        /// Queued to be downloaded.
        /// </summary>
        Queued,

        /// <summary>
        /// Under downloading.
        /// </summary>
        Downloading,

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
