//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace Analysis.LatencyAnalysis
{
    class Constants
    {
        //Protocol

        public enum LatencyAnalysisProtocol : byte
        {
            TCP, //TCP - Transmission Control Protocol (RFC 793)
            UDP //UDP - User Datagram Protocol (RFC 768)
        }

        //Latencies

        public const double LatencyAnalysisBestCaseLatency = 0.0; //Milliseconds
        public const double LatencyAnalysisWorstCaseLatency = 50.0; //Milliseconds

        //Value bins

        public const int LatencyAnalysisBinsPerMs = 10; //Ten value bins for every millisecond of latency
        public const int LatencyAnalysisNumberOfBins = ((int)(LatencyAnalysisWorstCaseLatency - LatencyAnalysisBestCaseLatency) * LatencyAnalysisBinsPerMs);
    }
}