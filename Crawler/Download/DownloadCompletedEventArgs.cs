// <copyright file="DownloadCompletedEventArgs.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler
{
    using System;

    /// <summary>
    /// Defines EventArgs for OnCompleted downloader event.
    /// </summary>
    public class DownloadCompletedEventArgs : EventArgs
    {
        private readonly Request request;

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="request">Request associated with the event.</param>
        internal DownloadCompletedEventArgs(Request request)
        {
            this.request = request;
        }

        /// <summary>
        /// Gets request associated with the event.
        /// </summary>
        public Request Request => this.request;
    }
}
