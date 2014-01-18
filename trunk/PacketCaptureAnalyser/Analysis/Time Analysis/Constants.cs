//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace AnalysisNamespace.TimeAnalysisNamespace
{
    class Constants
    {
        //Timestamps

        public const double TimeAnalysisMinTimestampDifference = 0.03; //Milliseconds

        //Timestamp and Time differences

        public const double TimeAnalysisExpectedDifference = 1000.0; //Milliseconds

        public const double TimeAnalysisMaxNegativeDifference = -2.0; //Milliseconds
        public const double TimeAnalysisMaxPositiveDifference = 2.0; //Milliseconds

        //Value bins

        public const int TimeAnalysisTimestampBinsPerMs = 200; //Two hundred value bins for every millisecond of latency
        public const int TimeAnalysisTimestampNumberOfBins = ((int)(TimeAnalysisMaxPositiveDifference - TimeAnalysisMaxNegativeDifference) * TimeAnalysisTimestampBinsPerMs);

        public const int TimeAnalysisTimeBinsPerMs = 200; //Two hundred value bins for every millisecond of latency
        public const int TimeAnalysisTimeNumberOfBins = ((int)(TimeAnalysisMaxPositiveDifference - TimeAnalysisMaxNegativeDifference) * TimeAnalysisTimeBinsPerMs);
    }
}