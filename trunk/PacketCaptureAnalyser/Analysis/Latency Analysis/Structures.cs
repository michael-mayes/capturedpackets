// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace Analysis.LatencyAnalysis
{
    /// <summary>
    /// This class provides structures for use by the latency analysis processing
    /// </summary>
    public class Structures
    {
        /// <summary>
        /// Dictionary Key
        /// </summary>
        public struct DictionaryKey
        {
            /// <summary>
            /// The host Id for the message in the dictionary value looked up by this dictionary key
            /// </summary>
            public byte HostId;

            /// <summary>
            /// The protocol for the message in the dictionary value looked up by this dictionary key
            /// </summary>
            public Constants.Protocol Protocol;

            /// <summary>
            /// The sequence number for the message in the dictionary value looked up by this dictionary key
            /// </summary>
            public ulong SequenceNumber;

            /// <summary>
            /// Initializes a new instance of the DictionaryKey struct
            /// </summary>
            /// <param name="theHostId">The host Id for the message in the dictionary value looked up by this dictionary key</param>
            /// <param name="theProtocol">The protocol for the message in the dictionary value looked up by this dictionary key</param>
            /// <param name="theSequenceNumber">The sequence number for the message in the dictionary value looked up by this dictionary key</param>
            public DictionaryKey(byte theHostId, Constants.Protocol theProtocol, ulong theSequenceNumber)
            {
                this.HostId = theHostId;
                this.Protocol = theProtocol;
                this.SequenceNumber = theSequenceNumber;
            }

            /// <summary>
            /// Determines whether the current and supplied dictionary keys are equal
            /// </summary>
            /// <param name="obj">The dictionary key to be compared with the current dictionary key</param>
            /// <returns>Boolean flag that indicates whether the two dictionary keys are equal</returns>
            public override bool Equals(object obj)
            {
                if (obj is DictionaryKey)
                {
                    DictionaryKey key = (DictionaryKey)obj;

                    // The two dictionary keys are deemed identical if the fields of each are identical
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

            /// <summary>
            /// Determines whether the two supplied dictionary keys are equal
            /// </summary>
            /// <param name="a">The first dictionary key to be compared</param>
            /// <param name="b">The second dictionary key to be compared</param>
            /// <returns>Boolean flag that indicates whether the two dictionary keys are equal</returns>
            public static bool operator ==(DictionaryKey a, DictionaryKey b)
            {
                // If both dictionary keys are null, or both are same instance, return true
                if (System.Object.ReferenceEquals(a, b))
                {
                    return true;
                }

                // If one dictionary key is null, but not both, return false
                if (((object)a == null) || ((object)b == null))
                {
                    return false;
                }

                // Otherwise the two dictionary keys are deemed identical if the fields of each are identical
                return
                    a.HostId == b.HostId &&
                    a.Protocol == b.Protocol &&
                    a.SequenceNumber == b.SequenceNumber;
            }

            /// <summary>
            /// Determines whether the two supplied dictionary keys are different
            /// </summary>
            /// <param name="a">The first dictionary key to be compared</param>
            /// <param name="b">The second dictionary key to be compared</param>
            /// <returns>Boolean flag that indicates whether the two dictionary keys are different</returns>
            public static bool operator !=(DictionaryKey a, DictionaryKey b)
            {
                //Call the == operator and take a ! of the returned value
                return !(a == b);
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
                    this.Protocol.GetHashCode() ^
                    this.SequenceNumber.GetHashCode();
            }
        }

        /// <summary>
        /// Dictionary Value
        /// </summary>
        public struct DictionaryValue
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
            /// The frame number of the first instance of the message
            /// </summary>
            public ulong FirstInstanceFrameNumber;

            /// <summary>
            /// The frame number of the second instance of the message
            /// </summary>
            public ulong SecondInstanceFrameNumber;

            /// <summary>
            /// The timestamp for the first instance of the message
            /// </summary>
            public double FirstInstanceTimestamp;

            /// <summary>
            /// The timestamp for the second instance of the message
            /// </summary>
            public double SecondInstanceTimestamp;

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
                this.FirstInstanceFrameNumber = thePacketNumber;
                this.SecondInstanceFrameNumber = 0;
                this.FirstInstanceTimestamp = thePacketTimestamp;
                this.SecondInstanceTimestamp = 0.0;
                this.TimestampDifference = 0.0;
                this.TimestampDifferenceCalculated = false;
            }

            /// <summary>
            /// Determines whether the current and supplied dictionary values are equal
            /// </summary>
            /// <param name="obj">The dictionary value to be compared with the current dictionary value</param>
            /// <returns>Boolean flag that indicates whether the two dictionary values are equal</returns>
            public override bool Equals(object obj)
            {
                if (obj is DictionaryValue)
                {
                    DictionaryValue value = (DictionaryValue)obj;

                    // The two dictionary values are deemed identical if the fields of each are identical
                    return
                        this.MessageId == value.MessageId &&
                        this.FirstInstanceFound == value.FirstInstanceFound &&
                        this.SecondInstanceFound == value.SecondInstanceFound &&
                        this.FirstInstanceFrameNumber == value.FirstInstanceFrameNumber &&
                        this.SecondInstanceFrameNumber == value.SecondInstanceFrameNumber &&
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

            /// <summary>
            /// Determines whether the current and supplied dictionary values are equal
            /// </summary>
            /// <param name="a">The first dictionary value to be compared</param>
            /// <param name="b">The second dictionary value to be compared</param>
            /// <returns>Boolean flag that indicates whether the two dictionary values are equal</returns>
            public static bool operator ==(DictionaryValue a, DictionaryValue b)
            {
                // If both dictionary values are null, or both are same instance, return true
                if (System.Object.ReferenceEquals(a, b))
                {
                    return true;
                }

                // If one dictionary value is null, but not both, return false
                if (((object)a == null) || ((object)b == null))
                {
                    return false;
                }

                // Otherwise the two dictionary values are deemed identical if the fields of each are identical
                return
                    a.MessageId == b.MessageId &&
                    a.FirstInstanceFound == b.FirstInstanceFound &&
                    a.SecondInstanceFound == b.SecondInstanceFound &&
                    a.FirstInstanceFrameNumber == b.FirstInstanceFrameNumber &&
                    a.SecondInstanceFrameNumber == b.SecondInstanceFrameNumber &&
                    a.FirstInstanceTimestamp == b.FirstInstanceTimestamp &&
                    a.SecondInstanceTimestamp == b.SecondInstanceTimestamp &&
                    a.TimestampDifference == b.TimestampDifference &&
                    a.TimestampDifferenceCalculated == b.TimestampDifferenceCalculated;
            }

            /// <summary>
            /// Determines whether the current and supplied dictionary values are different
            /// </summary>
            /// <param name="a">The first dictionary value to be compared</param>
            /// <param name="b">The second dictionary value to be compared</param>
            /// <returns>Boolean flag that indicates whether the two dictionary values are different</returns>
            public static bool operator !=(DictionaryValue a, DictionaryValue b)
            {
                //Call the == operator and take a ! of the returned value
                return !(a == b);
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
                    this.FirstInstanceFrameNumber.GetHashCode() ^
                    this.SecondInstanceFrameNumber.GetHashCode() ^
                    this.FirstInstanceTimestamp.GetHashCode() ^
                    this.SecondInstanceTimestamp.GetHashCode() ^
                    this.TimestampDifference.GetHashCode() ^
                    this.TimestampDifferenceCalculated.GetHashCode();
            }
        }
    }
}