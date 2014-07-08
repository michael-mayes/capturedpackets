// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace Analysis.LatencyAnalysis
{
    /// <summary>
    /// This class provides constants for use by the latency analysis processing
    /// </summary>
    public class Constants
    {
        //// Latencies

        /// <summary>
        /// The best case (lowest) latency to be shown in the histogram
        /// </summary>
        public const double BestCaseLatency = 0.0; // Milliseconds

        /// <summary>
        /// The worst case (highest) latency to be shown in the histogram
        /// </summary>
        public const double WorstCaseLatency = 50.0; // Milliseconds

        //// Value bins

        /// <summary>
        /// The number of bins for each millisecond to be used for the histogram of latency values
        /// </summary>
        public const uint BinsPerMillisecond = 10; // Ten value bins for every millisecond of latency

        /// <summary>
        /// The number of bins to be used for the histogram of latency values - calculated from other constants
        /// </summary>
        public const uint NumberOfBins = (uint)(WorstCaseLatency - BestCaseLatency) * BinsPerMillisecond;

        /// <summary>
        /// Protocol for the analyzed message
        /// </summary>
        public enum Protocol : byte
        {
            /// <summary>
            /// TCP - Transmission Control Protocol (RFC 793)
            /// </summary>
            TCP,

            /// <summary>
            /// UDP - User Datagram Protocol (RFC 768)
            /// </summary>
            UDP
        }
    }
}