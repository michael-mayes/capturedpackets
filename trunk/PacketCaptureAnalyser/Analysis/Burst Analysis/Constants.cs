// %PCMS_HEADER_SUBSTITUTION_START% %PM%    %PR%  %PRT% %PCMS_HEADER_SUBSTITUTION_END% //
// $Id: Constants.cs 231 2014-07-06 23:05:41Z michaelmayes $
// $URL: https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Analysis/Time%20Analysis/Constants.cs $
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace Analysis.BurstAnalysis
{
    /// <summary>
    /// This class provides constants for use by the burst analysis processing
    /// </summary>
    public class Constants
    {
        //// Timestamps

        /// <summary>
        /// The minimum timestamp difference to be shown in the histogram
        /// </summary>
        public const double MinTimestampDifference = 0.0; // Milliseconds

        /// <summary>
        /// The minimum timestamp difference to be shown in the histogram
        /// </summary>
        public const double MaxTimestampDifference = 1200.0; // Milliseconds

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
