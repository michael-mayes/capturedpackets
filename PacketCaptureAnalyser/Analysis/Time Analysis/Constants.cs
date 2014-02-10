//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace Analysis.TimeAnalysis
{
    class Constants
    {
        //Timestamps

        public const double MinTimestampDifference = 0.03; //Milliseconds

        //Timestamp and Time differences

        public const double ExpectedTimeDifference = 1000.0; //Milliseconds

        public const double MaxNegativeTimeDifference = -2.0; //Milliseconds
        public const double MaxPositiveTimeDifference = 2.0; //Milliseconds

        //Value bins

        public const int TimestampBinsPerMillisecond = 40; //Forty value bins for every millisecond of latency
        public const int TimestampNumberOfBins = ((int)(MaxPositiveTimeDifference - MaxNegativeTimeDifference) * TimestampBinsPerMillisecond);

        public const int TimeBinsPerMillisecond = 40; //Forty value bins for every millisecond of latency
        public const int TimeNumberOfBins = ((int)(MaxPositiveTimeDifference - MaxNegativeTimeDifference) * TimeBinsPerMillisecond);
    }
}