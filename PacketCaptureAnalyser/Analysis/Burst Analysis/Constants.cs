// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.Analysis.BurstAnalysis
{
    /// <summary>
    /// This class provides constants for use by the burst analysis processing
    /// </summary>
    public static class Constants
    {
        //// Timestamp differences

        /// <summary>
        /// The minimum absolute timestamp difference that will be accepted for successive messages
        /// This should prevent the processing of duplicates of a message (e.g. if port mirroring results in two copies of the time-supplying message)
        /// </summary>
        public const double MinAbsoluteTimestampDifference = 0.03; // Milliseconds

        /// <summary>
        /// The minimum timestamp difference to be shown in the histogram
        /// </summary>
        public const double MinTimestampDifference = 0.0; // Milliseconds

        /// <summary>
        /// The minimum timestamp difference to be shown in the histogram
        /// </summary>
        public const double MaxTimestampDifference = 15000.0; // Milliseconds

        //// Value bins

        /// <summary>
        /// The number of bins for each millisecond to be used for the histogram of timestamp values
        /// </summary>
        public const uint BinsPerMillisecond = 10; // Ten value bins for every millisecond between messages

        /// <summary>
        /// The number of bins to be used for the histogram of timestamp values - calculated from other constants
        /// </summary>
        public const uint NumberOfBins = (uint)(MaxTimestampDifference - MinTimestampDifference) * BinsPerMillisecond;
    }
}
