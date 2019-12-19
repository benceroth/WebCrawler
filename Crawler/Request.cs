// <copyright file="Request.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Request that can be downloaded and processed.
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Gets or sets request name. Used for file name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets absolute file path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets url associated with the request.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets process state.
        /// </summary>
        public ProcessState ProcessState { get; set; }

        /// <summary>
        /// Gets or sets download state.
        /// </summary>
        public DownloadState DownloadState { get; set; }
    }
}
