// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace Analysis.TimeAnalysis
{
    /// <summary>
    /// This class provides constants for use by the time analysis processing
    /// </summary>
    public class Constants
    {
        //// Timestamps

        /// <summary>
        /// The minimum timestamp difference be accepted - should prevent the processing of duplicates of a time message (e.g. if port mirroring results in two copies of the time message)
        /// </summary>
        public const double MinTimestampDifference = 0.03; // Milliseconds

        //// Timestamp and Time differences

        /// <summary>
        /// The expected difference between the timestamp of two successive time messages
        /// The expected difference between the time of two successive time messages
        /// </summary>
        public const double ExpectedTimeDifference = 1000.0; // Milliseconds

        /// <summary>
        /// The maximum negative difference from the expected for two successive time messages
        /// </summary>
        public const double MaxNegativeTimeDifference = -2.0; // Milliseconds

        /// <summary>
        /// The maximum positive difference from the expected for two successive time messages
        /// </summary>
        public const double MaxPositiveTimeDifference = 2.0; // Milliseconds

        //// Value bins

        /// <summary>
        /// The number of bins for each millisecond to be used for the histogram of timestamp values
        /// </summary>
        public const uint TimestampBinsPerMillisecond = 40; // Forty value bins for every millisecond of timestamp difference

        /// <summary>
        /// The number of bins to be used for the histogram of timestamp values - calculated from other constants
        /// </summary>
        public const uint TimestampNumberOfBins = (uint)(MaxPositiveTimeDifference - MaxNegativeTimeDifference) * TimestampBinsPerMillisecond;

        /// <summary>
        /// The number of bins for each millisecond to be used for the histogram of time values
        /// </summary>
        public const uint TimeBinsPerMillisecond = 40; // Forty value bins for every millisecond of time difference

        /// <summary>
        /// The number of bins to be used for the histogram of time values - calculated from other constants
        /// </summary>
        public const uint TimeNumberOfBins = (uint)(MaxPositiveTimeDifference - MaxNegativeTimeDifference) * TimeBinsPerMillisecond;
    }
}
