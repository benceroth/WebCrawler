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
    public class Request<T> : Request
    {
        /// <summary>
        /// Gets or sets request ItemDataBound.
        /// </summary>
        public T ItemDataBound { get; set; }
    }
}
