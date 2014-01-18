//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace Analysis.LatencyAnalysis
{
    class Structures
    {
        //Dictionary Key

        public struct LatencyAnalysisDictionaryKey
        {
            public byte HostId;
            public Constants.LatencyAnalysisProtocol Protocol;
            public ulong SequenceNumber;

            public LatencyAnalysisDictionaryKey(byte TheHostId, Constants.LatencyAnalysisProtocol TheProtocol, ulong TheSequenceNumber)
            {
                HostId = TheHostId;
                Protocol = TheProtocol;
                SequenceNumber = TheSequenceNumber;
            }

            public override bool Equals(object obj)
            {
                if (obj is LatencyAnalysisDictionaryKey)
                {
                    LatencyAnalysisDictionaryKey Key = (LatencyAnalysisDictionaryKey)obj;

                    return
                        (
                        HostId == Key.HostId &&
                        Protocol == Key.Protocol &&
                        SequenceNumber == Key.SequenceNumber
                        );
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return
                    HostId.GetHashCode() ^
                    Protocol.GetHashCode() ^
                    SequenceNumber.GetHashCode();
            }
        }

        //Dictionary Value

        public struct LatencyAnalysisDictionaryValue
        {
            public ulong MessageId;
            public bool FirstInstanceFound;
            public bool SecondInstanceFound;
            public ulong FirstInstancePacketNumber;
            public ulong SecondInstancePacketNumber;
            public double FirstInstanceTimestamp;
            public double SecondInstanceTimestamp;
            public double TimestampDifference;
            public bool TimestampDifferenceCalculated;

            public override bool Equals(object obj)
            {
                if (obj is LatencyAnalysisDictionaryValue)
                {
                    LatencyAnalysisDictionaryValue Value = (LatencyAnalysisDictionaryValue)obj;

                    return
                        (
                        MessageId == Value.MessageId &&
                        FirstInstanceFound == Value.FirstInstanceFound &&
                        SecondInstanceFound == Value.SecondInstanceFound &&
                        FirstInstancePacketNumber == Value.FirstInstancePacketNumber &&
                        SecondInstancePacketNumber == Value.SecondInstancePacketNumber &&
                        FirstInstanceTimestamp == Value.FirstInstanceTimestamp &&
                        SecondInstanceTimestamp == Value.SecondInstanceTimestamp &&
                        TimestampDifference == Value.TimestampDifference &&
                        TimestampDifferenceCalculated == Value.TimestampDifferenceCalculated
                        );
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return
                    MessageId.GetHashCode() ^
                    FirstInstanceFound.GetHashCode() ^
                    SecondInstanceFound.GetHashCode() ^
                    FirstInstancePacketNumber.GetHashCode() ^
                    SecondInstancePacketNumber.GetHashCode() ^
                    FirstInstanceTimestamp.GetHashCode() ^
                    SecondInstanceTimestamp.GetHashCode() ^
                    TimestampDifference.GetHashCode() ^
                    TimestampDifferenceCalculated.GetHashCode();
            }
        }
    }
}