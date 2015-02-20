// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.Analysis.TimeAnalysis
{
    /// <summary>
    /// This class provides constants for use by the time analysis processing
    /// </summary>
    public static class Constants
    {
        //// Timestamp differences

        /// <summary>
        /// The minimum absolute timestamp difference that will be accepted for successive time-supplying messages
        /// This should prevent the processing of duplicates of a time-supplying message (e.g. if port mirroring results in two copies of the time-supplying message)
        /// </summary>
        public const double MinAbsoluteTimestampDifference = 0.03; // Milliseconds

        /// <summary>
        /// The maximum absolute timestamp difference that will be accepted for successive time-supplying messages
        /// This should prevent a large gap in time-supplying messages unduly affecting the processing (e.g. if the host is in the process of being restarted or becomes available)
        /// </summary>
        public const double MaxAbsoluteTimestampDifference = 3000.0; // Milliseconds

        /// <summary>
        /// The minimum timestamp difference expected for successive time-supplying messages in the lower range
        /// The value chosen is one eighth of a second (to allow for time-supplying messages at a rate of 8 Hz) minus two milliseconds
        /// </summary>
        public const double MinLowerRangeTimestampDifference = 123.0; // Milliseconds

        /// <summary>
        /// The maximum timestamp difference expected for successive time-supplying messages in the lower range
        /// The value chosen is one eighth of a second (to allow for time-supplying messages at a rate of 8 Hz) plus two milliseconds
        /// </summary>
        public const double MaxLowerRangeTimestampDifference = 127.0; // Milliseconds

        /// <summary>
        /// The minimum timestamp difference expected for successive time-supplying messages in the middle range
        /// The value chosen is one eighth of a second (to allow for time-supplying messages at a rate of 8 Hz) minus two milliseconds
        /// </summary>
        public const double MinMiddleRangeTimestampDifference = 131.33333333333333333333333333333; // Milliseconds

        /// <summary>
        /// The maximum timestamp difference expected for successive time-supplying messages in the middle range
        /// The value chosen is one eighth of a second (to allow for time-supplying messages at a rate of 8 Hz) plus two milliseconds
        /// </summary>
        public const double MaxMiddleRangeTimestampDifference = 135.33333333333333333333333333333; // Milliseconds

        /// <summary>
        /// The minimum timestamp difference expected for successive time-supplying messages in the upper range
        /// The value chosen is one second (to allow for time-supplying messages at a rate of 1 Hz) plus two milliseconds
        /// </summary>
        public const double MinUpperRangeTimestampDifference = 998.0; // Milliseconds

        /// <summary>
        /// The maximum timestamp difference expected for successive time-supplying messages in the upper range
        /// The value chosen is one second (to allow for time-supplying messages at a rate of 1 Hz) plus two milliseconds
        /// </summary>
        public const double MaxUpperRangeTimestampDifference = 1002.0; // Milliseconds

        //// Time differences

        /// <summary>
        /// The minimum times difference expected for successive time-supplying messages in the lower range
        /// The value chosen is one eighth of a second (to allow for time-supplying messages at a rate of 8 Hz) minus two milliseconds
        /// </summary>
        public const double MinLowerRangeTimeDifference = 123.0; // Milliseconds

        /// <summary>
        /// The maximum time difference expected for successive time-supplying messages in the lower range
        /// The value chosen is one eighth of a second (to allow for time-supplying messages at a rate of 8 Hz) plus two milliseconds
        /// </summary>
        public const double MaxLowerRangeTimeDifference = 127.0; // Milliseconds

        /// <summary>
        /// The minimum times difference expected for successive time-supplying messages in the middle range
        /// The value chosen is one eighth of a second (to allow for time-supplying messages at a rate of 8 Hz) minus two milliseconds
        /// </summary>
        public const double MinMiddleRangeTimeDifference = 131.33333333333333333333333333333; // Milliseconds

        /// <summary>
        /// The maximum time difference expected for successive time-supplying messages in the middle range
        /// The value chosen is one eighth of a second (to allow for time-supplying messages at a rate of 8 Hz) plus two milliseconds
        /// </summary>
        public const double MaxMiddleRangeTimeDifference = 135.33333333333333333333333333333; // Milliseconds

        /// <summary>
        /// The minimum time difference expected for successive time-supplying messages in the upper range
        /// The value chosen is one second (to allow for time-supplying messages at a rate of 1 Hz) plus two milliseconds
        /// </summary>
        public const double MinUpperRangeTimeDifference = 998.0; // Milliseconds

        /// <summary>
        /// The maximum time difference expected for successive time-supplying messages in the upper range
        /// The value chosen is one second (to allow for time-supplying messages at a rate of 1 Hz) plus two milliseconds
        /// </summary>
        public const double MaxUpperRangeTimeDifference = 1002.0; // Milliseconds

        //// Value bins

        /// <summary>
        /// The number of bins for each millisecond to be used for the histogram of timestamp values
        /// </summary>
        public const uint TimestampBinsPerMillisecond = 40; // Forty value bins for every millisecond of timestamp difference

        /// <summary>
        /// The number of bins to be used for the histogram of timestamp values in the lower range - calculated from other constants
        /// </summary>
        public const uint LowerRangeTimestampNumberOfBins = (uint)(MaxLowerRangeTimestampDifference - MinLowerRangeTimestampDifference) * TimestampBinsPerMillisecond;

        /// <summary>
        /// The number of bins to be used for the histogram of timestamp values in the middle range - calculated from other constants
        /// </summary>
        public const uint MiddleRangeTimestampNumberOfBins = (uint)(MaxMiddleRangeTimestampDifference - MinMiddleRangeTimestampDifference) * TimestampBinsPerMillisecond;

        /// <summary>
        /// The number of bins to be used for the histogram of timestamp values in the upper range - calculated from other constants
        /// </summary>
        public const uint UpperRangeTimestampNumberOfBins = (uint)(MaxUpperRangeTimestampDifference - MinUpperRangeTimestampDifference) * TimestampBinsPerMillisecond;

        /// <summary>
        /// The number of bins for each millisecond to be used for the histogram of time values
        /// </summary>
        public const uint TimeBinsPerMillisecond = 40; // Forty value bins for every millisecond of time difference

        /// <summary>
        /// The number of bins to be used for the histogram of time values in the lower range - calculated from other constants
        /// </summary>
        public const uint LowerRangeTimeNumberOfBins = (uint)(MaxLowerRangeTimeDifference - MinLowerRangeTimeDifference) * TimeBinsPerMillisecond;

        /// <summary>
        /// The number of bins to be used for the histogram of time values in the middle range - calculated from other constants
        /// </summary>
        public const uint MiddleRangeTimeNumberOfBins = (uint)(MaxMiddleRangeTimeDifference - MinMiddleRangeTimeDifference) * TimeBinsPerMillisecond;

        /// <summary>
        /// The number of bins to be used for the histogram of time values in the upper range - calculated from other constants
        /// </summary>
        public const uint UpperRangeTimeNumberOfBins = (uint)(MaxUpperRangeTimeDifference - MinUpperRangeTimeDifference) * TimeBinsPerMillisecond;
    }
}
