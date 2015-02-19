// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.Analysis.LatencyAnalysis
{
    /// <summary>
    /// This class provides structures for use by the latency analysis processing
    /// </summary>
    public static class Structures
    {
        /// <summary>
        /// Dictionary Key
        /// </summary>
        public struct DictionaryKey : System.IEquatable<DictionaryKey>
        {
            /// <summary>
            /// The host Id for the message in the dictionary value looked up by this dictionary key
            /// </summary>
            public byte HostId;

            /// <summary>
            /// Boolean flag that indicates whether the message in the dictionary value looked up by this dictionary key was received reliably
            /// </summary>
            public bool IsReliable;

            /// <summary>
            /// The sequence number for the message in the dictionary value looked up by this dictionary key
            /// </summary>
            public ulong SequenceNumber;

            /// <summary>
            /// Initializes a new instance of the DictionaryKey struct
            /// </summary>
            /// <param name="theHostId">The host Id for the message in the dictionary value looked up by this dictionary key</param>
            /// <param name="isReliable">Boolean flag that indicates whether the message in the dictionary value looked up by this dictionary key was received reliably</param>
            /// <param name="theSequenceNumber">The sequence number for the message in the dictionary value looked up by this dictionary key</param>
            public DictionaryKey(byte theHostId, bool isReliable, ulong theSequenceNumber)
            {
                this.HostId = theHostId;
                this.IsReliable = isReliable;
                this.SequenceNumber = theSequenceNumber;
            }

            /// <summary>
            /// Determines whether the two supplied dictionary keys are equal
            /// </summary>
            /// <param name="a">The first dictionary key to be compared</param>
            /// <param name="b">The second dictionary key to be compared</param>
            /// <returns>Boolean flag that indicates whether the two dictionary keys are equal</returns>
            public static bool operator ==(DictionaryKey a, DictionaryKey b)
            {
                // Call the Equals operator to compare the two supplied dictionary keys
                return a.Equals(b);
            }

            /// <summary>
            /// Determines whether the two supplied dictionary keys are different
            /// </summary>
            /// <param name="a">The first dictionary key to be compared</param>
            /// <param name="b">The second dictionary key to be compared</param>
            /// <returns>Boolean flag that indicates whether the two dictionary keys are different</returns>
            public static bool operator !=(DictionaryKey a, DictionaryKey b)
            {
                // Call the Equals operator to compare the two supplied dictionary keys and take a ! of the returned value
                return !a.Equals(b);
            }

            /// <summary>
            /// Determines whether the current and supplied dictionary keys are equal
            /// </summary>
            /// <param name="other">The dictionary key to be compared with the current dictionary key</param>
            /// <returns>Boolean flag that indicates whether the two dictionary keys are equal</returns>
            public bool Equals(DictionaryKey other)
            {
                // The two dictionary keys are deemed identical if the fields of each are identical
                return
                    this.HostId == other.HostId &&
                    this.IsReliable == other.IsReliable &&
                    this.SequenceNumber == other.SequenceNumber;
            }

            /// <summary>
            /// Determines whether the current and supplied dictionary keys are equal
            /// </summary>
            /// <param name="obj">The dictionary key to be compared with the current dictionary key</param>
            /// <returns>Boolean flag that indicates whether the two dictionary keys are equal</returns>
            public override bool Equals(object obj)
            {
                // If the supplied dictionary key is null then return false
                if (obj == null)
                {
                    return false;
                }

                // If the supplied dictionary key has a different type to the current dictionary key then return false
                if (GetType() != obj.GetType())
                {
                    return false;
                }

                if (obj is DictionaryKey)
                {
                    return Equals((DictionaryKey)obj);
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Returns the hash code of the dictionary key
            /// </summary>
            /// <returns>The hash code of the dictionary key</returns>
            public override int GetHashCode()
            {
                // The hash code for the dictionary key is calculated as an exclusive OR of each of its fields
                return
                    this.HostId.GetHashCode() ^
                    this.IsReliable.GetHashCode() ^
                    this.SequenceNumber.GetHashCode();
            }
        }

        /// <summary>
        /// Dictionary Value
        /// </summary>
        public struct DictionaryValue : System.IEquatable<DictionaryValue>
        {
            /// <summary>
            /// The identifier for the type of message in the dictionary value
            /// </summary>
            public ulong MessageId;

            /// <summary>
            /// Boolean flag that indicates whether a first instance of the message has been found
            /// </summary>
            public bool FirstInstanceFound;

            /// <summary>
            /// Boolean flag that indicates whether a second instance of the message has been found
            /// </summary>
            public bool SecondInstanceFound;

            /// <summary>
            /// The packet number of the first instance of the message
            /// </summary>
            public ulong FirstInstancePacketNumber;

            /// <summary>
            /// The packet number of the second instance of the message
            /// </summary>
            public ulong SecondInstancePacketNumber;

            /// <summary>
            /// The packet timestamp for the first instance of the message
            /// </summary>
            public double FirstInstancePacketTimestamp;

            /// <summary>
            /// The packet timestamp for the second instance of the message
            /// </summary>
            public double SecondInstancePacketTimestamp;

            /// <summary>
            /// The difference between the timestamps for the first and second instances of the message
            /// </summary>
            public double TimestampDifference;

            /// <summary>
            /// Boolean flag that indicates whether the difference between the timestamps has been calculated
            /// </summary>
            public bool TimestampDifferenceCalculated;

            /// <summary>
            /// Initializes a new instance of the DictionaryValue struct
            /// </summary>
            /// <param name="theMessageId">The identifier for the type of message</param>
            /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
            /// <param name="thePacketTimestamp">The timestamp read from the packet capture</param>
            public DictionaryValue(ulong theMessageId, ulong thePacketNumber, double thePacketTimestamp)
            {
                this.MessageId = theMessageId;
                this.FirstInstanceFound = true;
                this.SecondInstanceFound = false;
                this.FirstInstancePacketNumber = thePacketNumber;
                this.SecondInstancePacketNumber = 0;
                this.FirstInstancePacketTimestamp = thePacketTimestamp;
                this.SecondInstancePacketTimestamp = 0.0;
                this.TimestampDifference = 0.0;
                this.TimestampDifferenceCalculated = false;
            }

            /// <summary>
            /// Determines whether the current and supplied dictionary values are equal
            /// </summary>
            /// <param name="a">The first dictionary value to be compared</param>
            /// <param name="b">The second dictionary value to be compared</param>
            /// <returns>Boolean flag that indicates whether the two dictionary values are equal</returns>
            public static bool operator ==(DictionaryValue a, DictionaryValue b)
            {
                // Call the Equals operator to compare the two supplied dictionary values
                return a.Equals(b);
            }

            /// <summary>
            /// Determines whether the current and supplied dictionary values are different
            /// </summary>
            /// <param name="a">The first dictionary value to be compared</param>
            /// <param name="b">The second dictionary value to be compared</param>
            /// <returns>Boolean flag that indicates whether the two dictionary values are different</returns>
            public static bool operator !=(DictionaryValue a, DictionaryValue b)
            {
                // Call the Equals operator to compare the two supplied dictionary values and take a ! of the returned value
                return !a.Equals(b);
            }

            /// <summary>
            /// Determines whether the current and supplied dictionary values are equal
            /// </summary>
            /// <param name="other">The dictionary value to be compared with the current dictionary value</param>
            /// <returns>Boolean flag that indicates whether the two dictionary values are equal</returns>
            public bool Equals(DictionaryValue other)
            {
                // The two dictionary values are deemed identical if the fields of each are identical
                return
                    this.MessageId == other.MessageId &&
                    this.FirstInstanceFound == other.FirstInstanceFound &&
                    this.SecondInstanceFound == other.SecondInstanceFound &&
                    this.FirstInstancePacketNumber == other.FirstInstancePacketNumber &&
                    this.SecondInstancePacketNumber == other.SecondInstancePacketNumber &&
                    this.FirstInstancePacketTimestamp == other.FirstInstancePacketTimestamp &&
                    this.SecondInstancePacketTimestamp == other.SecondInstancePacketTimestamp &&
                    this.TimestampDifference == other.TimestampDifference &&
                    this.TimestampDifferenceCalculated == other.TimestampDifferenceCalculated;
            }

            /// <summary>
            /// Determines whether the current and supplied dictionary values are equal
            /// </summary>
            /// <param name="obj">The dictionary value to be compared with the current dictionary value</param>
            /// <returns>Boolean flag that indicates whether the two dictionary values are equal</returns>
            public override bool Equals(object obj)
            {
                // If the dictionary value is null then return false
                if (obj == null)
                {
                    return false;
                }

                // If the supplied dictionary value has a different type to the current dictionary key then return false
                if (GetType() != obj.GetType())
                {
                    return false;
                }

                if (obj is DictionaryValue)
                {
                    return Equals((DictionaryValue)obj);
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// Returns the hash code of the dictionary value
            /// </summary>
            /// <returns>The hash code of the dictionary value</returns>
            public override int GetHashCode()
            {
                // The hash code for the dictionary value is calculated as an exclusive OR of each of its fields
                return
                    this.MessageId.GetHashCode() ^
                    this.FirstInstanceFound.GetHashCode() ^
                    this.SecondInstanceFound.GetHashCode() ^
                    this.FirstInstancePacketNumber.GetHashCode() ^
                    this.SecondInstancePacketNumber.GetHashCode() ^
                    this.FirstInstancePacketTimestamp.GetHashCode() ^
                    this.SecondInstancePacketTimestamp.GetHashCode() ^
                    this.TimestampDifference.GetHashCode() ^
                    this.TimestampDifferenceCalculated.GetHashCode();
            }
        }
    }
}