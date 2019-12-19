// <copyright file="ProcessCompletedEventArgs.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines EventArgs for OnCompleted processor event.
    /// </summary>
    public class ProcessCompletedEventArgs : EventArgs
    {
        private readonly Request request;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="request">Request associated with the event.</param>
        internal ProcessCompletedEventArgs(Request request)
        {
            this.request = request;
        }

        /// <summary>
        /// Gets request associated with the event.
        /// </summary>
        public Request Request => this.request;
    }
}
