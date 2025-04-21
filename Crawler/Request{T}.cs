// <copyright file="Request{T}.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler
{
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
