// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace Analysis.LatencyAnalysis
{
    class Structures
    {
        /// <summary>
        /// Dictionary Key
        /// </summary>
        public struct DictionaryKey
        {
            public byte HostId;
            public Constants.Protocol Protocol;
            public ulong SequenceNumber;

            /// <summary>
            /// Initializes a new instance of the DictionaryKey struct
            /// </summary>
            /// <param name="theHostId"></param>
            /// <param name="theProtocol"></param>
            /// <param name="theSequenceNumber"></param>
            public DictionaryKey(byte theHostId, Constants.Protocol theProtocol, ulong theSequenceNumber)
            {
                this.HostId = theHostId;
                this.Protocol = theProtocol;
                this.SequenceNumber = theSequenceNumber;
            }

            public override bool Equals(object obj)
            {
                if (obj is DictionaryKey)
                {
                    DictionaryKey key = (DictionaryKey)obj;

                    return
                        this.HostId == key.HostId &&
                        this.Protocol == key.Protocol &&
                        this.SequenceNumber == key.SequenceNumber;
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return
                    this.HostId.GetHashCode() ^
                    this.Protocol.GetHashCode() ^
                    this.SequenceNumber.GetHashCode();
            }
        }

        /// <summary>
        /// Dictionary Value
        /// </summary>
        public struct DictionaryValue
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
                if (obj is DictionaryValue)
                {
                    DictionaryValue value = (DictionaryValue)obj;

                    return
                        this.MessageId == value.MessageId &&
                        this.FirstInstanceFound == value.FirstInstanceFound &&
                        this.SecondInstanceFound == value.SecondInstanceFound &&
                        this.FirstInstancePacketNumber == value.FirstInstancePacketNumber &&
                        this.SecondInstancePacketNumber == value.SecondInstancePacketNumber &&
                        this.FirstInstanceTimestamp == value.FirstInstanceTimestamp &&
                        this.SecondInstanceTimestamp == value.SecondInstanceTimestamp &&
                        this.TimestampDifference == value.TimestampDifference &&
                        this.TimestampDifferenceCalculated == value.TimestampDifferenceCalculated;
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return
                    this.MessageId.GetHashCode() ^
                    this.FirstInstanceFound.GetHashCode() ^
                    this.SecondInstanceFound.GetHashCode() ^
                    this.FirstInstancePacketNumber.GetHashCode() ^
                    this.SecondInstancePacketNumber.GetHashCode() ^
                    this.FirstInstanceTimestamp.GetHashCode() ^
                    this.SecondInstanceTimestamp.GetHashCode() ^
                    this.TimestampDifference.GetHashCode() ^
                    this.TimestampDifferenceCalculated.GetHashCode();
            }
        }
    }
}