// <copyright file="DictionaryValue.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.Analysis.LatencyAnalysis
{
    /// <summary>
    /// Dictionary Value
    /// </summary>
    public class DictionaryValue : System.IEquatable<DictionaryValue>
    {
        /// <summary>
        /// The identifier for the type of message in the dictionary value
        /// </summary>
        private ulong theMessageId;

        /// <summary>
        /// Boolean flag that indicates whether a first instance of the message has been found
        /// </summary>
        private bool theFirstInstanceFound;

        /// <summary>
        /// Boolean flag that indicates whether a second instance of the message has been found
        /// </summary>
        private bool theSecondInstanceFound;

        /// <summary>
        /// The packet number of the first instance of the message
        /// </summary>
        private ulong theFirstInstancePacketNumber;

        /// <summary>
        /// The packet number of the second instance of the message
        /// </summary>
        private ulong theSecondInstancePacketNumber;

        /// <summary>
        /// The packet timestamp for the first instance of the message
        /// </summary>
        private double theFirstInstancePacketTimestamp;

        /// <summary>
        /// The packet timestamp for the second instance of the message
        /// </summary>
        private double theSecondInstancePacketTimestamp;

        /// <summary>
        /// The difference between the timestamps for the first and second instances of the message
        /// </summary>
        private double theTimestampDifference;

        /// <summary>
        /// Boolean flag that indicates whether the difference between the timestamps has been calculated
        /// </summary>
        private bool theTimestampDifferenceCalculated;

        /// <summary>
        /// Initializes a new instance of the DictionaryValue class
        /// </summary>
        /// <param name="theMessageId">The identifier for the type of message</param>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp read from the packet capture</param>
        public DictionaryValue(ulong theMessageId, ulong thePacketNumber, double thePacketTimestamp)
        {
            this.theMessageId = theMessageId;
            this.theFirstInstanceFound = true;
            this.theSecondInstanceFound = false;
            this.theFirstInstancePacketNumber = thePacketNumber;
            this.theSecondInstancePacketNumber = 0;
            this.theFirstInstancePacketTimestamp = thePacketTimestamp;
            this.theSecondInstancePacketTimestamp = 0.0;
            this.theTimestampDifference = 0.0;
            this.theTimestampDifferenceCalculated = false;
        }

        /// <summary>
        /// Gets or sets the identifier for the type of message in the dictionary value
        /// </summary>
        public ulong MessageId
        {
            get
            {
                return this.theMessageId;
            }

            set
            {
                this.theMessageId = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a first instance of the message has been found
        /// </summary>
        public bool FirstInstanceFound
        {
            get
            {
                return this.theFirstInstanceFound;
            }

            set
            {
                this.theFirstInstanceFound = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a second instance of the message has been found
        /// </summary>
        public bool SecondInstanceFound
        {
            get
            {
                return this.theSecondInstanceFound;
            }

            set
            {
                this.theSecondInstanceFound = value;
            }
        }

        /// <summary>
        /// Gets or sets the packet number of the first instance of the message
        /// </summary>
        public ulong FirstInstancePacketNumber
        {
            get
            {
                return this.theFirstInstancePacketNumber;
            }

            set
            {
                this.theFirstInstancePacketNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the packet number of the second instance of the message
        /// </summary>
        public ulong SecondInstancePacketNumber
        {
            get
            {
                return this.theSecondInstancePacketNumber;
            }

            set
            {
                this.theSecondInstancePacketNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the packet timestamp for the first instance of the message
        /// </summary>
        public double FirstInstancePacketTimestamp
        {
            get
            {
                return this.theFirstInstancePacketTimestamp;
            }

            set
            {
                this.theFirstInstancePacketTimestamp = value;
            }
        }

        /// <summary>
        /// Gets or sets the packet timestamp for the second instance of the message
        /// </summary>
        public double SecondInstancePacketTimestamp
        {
            get
            {
                return this.theSecondInstancePacketTimestamp;
            }

            set
            {
                this.theSecondInstancePacketTimestamp = value;
            }
        }

        /// <summary>
        /// Gets or sets the difference between the timestamps for the first and second instances of the message
        /// </summary>
        public double TimestampDifference
        {
            get
            {
                return this.theTimestampDifference;
            }

            set
            {
                this.theTimestampDifference = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the difference between the timestamps has been calculated
        /// </summary>
        public bool TimestampDifferenceCalculated
        {
            get
            {
                return this.theTimestampDifferenceCalculated;
            }

            set
            {
                this.theTimestampDifferenceCalculated = value;
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
            //// If either of the supplied dictionary values is null then return false

            if (object.ReferenceEquals(a, null))
            {
                return false;
            }

            if (object.ReferenceEquals(b, null))
            {
                return false;
            }

            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

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
            //// If either of the supplied dictionary values is null then return false

            if (object.ReferenceEquals(a, null))
            {
                return false;
            }

            if (object.ReferenceEquals(b, null))
            {
                return false;
            }

            if (object.ReferenceEquals(a, b))
            {
                return false;
            }

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
            // If the supplied dictionary value is null then return false
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }
            else
            {
                // The two dictionary values are deemed identical if the fields of each are identical
                return
                    this.theMessageId == other.theMessageId &&
                    this.theFirstInstanceFound == other.theFirstInstanceFound &&
                    this.theSecondInstanceFound == other.theSecondInstanceFound &&
                    this.theFirstInstancePacketNumber == other.theFirstInstancePacketNumber &&
                    this.theSecondInstancePacketNumber == other.theSecondInstancePacketNumber &&
                    this.theFirstInstancePacketTimestamp == other.theFirstInstancePacketTimestamp &&
                    this.theSecondInstancePacketTimestamp == other.theSecondInstancePacketTimestamp &&
                    this.theTimestampDifference == other.theTimestampDifference &&
                    this.theTimestampDifferenceCalculated == other.theTimestampDifferenceCalculated;
            }
        }

        /// <summary>
        /// Determines whether the current and supplied dictionary values are equal
        /// </summary>
        /// <param name="obj">The dictionary value to be compared with the current dictionary value</param>
        /// <returns>Boolean flag that indicates whether the two dictionary values are equal</returns>
        public override bool Equals(object obj)
        {
            // If the supplied dictionary value is null then return false
            if (object.ReferenceEquals(obj, null))
            {
                return false;
            }

            // If the supplied dictionary value has a different type to the current dictionary key then return false
            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            if (obj is DictionaryValue)
            {
                return this.Equals(obj);
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
                this.theMessageId.GetHashCode() ^
                this.theFirstInstanceFound.GetHashCode() ^
                this.theSecondInstanceFound.GetHashCode() ^
                this.theFirstInstancePacketNumber.GetHashCode() ^
                this.theSecondInstancePacketNumber.GetHashCode() ^
                this.theFirstInstancePacketTimestamp.GetHashCode() ^
                this.theSecondInstancePacketTimestamp.GetHashCode() ^
                this.theTimestampDifference.GetHashCode() ^
                this.theTimestampDifferenceCalculated.GetHashCode();
        }
    }
}