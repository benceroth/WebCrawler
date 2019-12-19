// <copyright file="RunFlags.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Crawler
{
    using System;

    /// <summary>
    /// Flags that define crawler running features.
    /// </summary>
    [Flags]
    public enum RunFlags
    {
        /// <summary>
        /// Does nothing,
        /// </summary>
        None = 0,

        /// <summary>
        /// Processes requests.
        /// </summary>
        Process = 1,

        /// <summary>
        /// Downloads requests.
        /// </summary>
        Download = 2,
    }
}
