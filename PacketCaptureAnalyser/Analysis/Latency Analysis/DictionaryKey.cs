// <copyright file="DictionaryKey.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.Analysis.LatencyAnalysis
{
    /// <summary>
    /// Dictionary Key
    /// </summary>
    public class DictionaryKey : System.IEquatable<DictionaryKey>
    {
        /// <summary>
        /// The host Id for the message in the dictionary value looked up by this dictionary key
        /// </summary>
        private byte theHostId;

        /// <summary>
        /// Boolean flag that indicates whether the message in the dictionary value looked up by this dictionary key was received reliably
        /// </summary>
        private bool isReliable;

        /// <summary>
        /// The sequence number for the message in the dictionary value looked up by this dictionary key
        /// </summary>
        private ulong theSequenceNumber;

        /// <summary>
        /// Initializes a new instance of the DictionaryKey class
        /// </summary>
        /// <param name="theHostId">The host Id for the message in the dictionary value looked up by this dictionary key</param>
        /// <param name="isReliable">Boolean flag that indicates whether the message in the dictionary value looked up by this dictionary key was received reliably</param>
        /// <param name="theSequenceNumber">The sequence number for the message in the dictionary value looked up by this dictionary key</param>
        public DictionaryKey(byte theHostId, bool isReliable, ulong theSequenceNumber)
        {
            this.theHostId = theHostId;
            this.isReliable = isReliable;
            this.theSequenceNumber = theSequenceNumber;
        }

        /// <summary>
        /// Gets or sets the host Id for the message in the dictionary value looked up by this dictionary key
        /// </summary>
        public byte HostId
        {
            get
            {
                return this.theHostId;
            }

            set
            {
                this.theHostId = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the message in the dictionary value looked up by this dictionary key was received reliably
        /// </summary>
        public bool IsReliable
        {
            get
            {
                return this.isReliable;
            }

            set
            {
                this.isReliable = value;
            }
        }

        /// <summary>
        /// Gets or sets the sequence number for the message in the dictionary value looked up by this dictionary key
        /// </summary>
        public ulong SequenceNumber
        {
            get
            {
                return this.theSequenceNumber;
            }

            set
            {
                this.theSequenceNumber = value;
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
            //// If either of the supplied dictionary keys is null then return false

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
            //// If either of the supplied dictionary keys is null then return false

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
            // If the supplied dictionary key is null then return false
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }
            else
            {
                // The two dictionary keys are deemed identical if the fields of each are identical
                return
                    this.theHostId == other.theHostId &&
                    this.isReliable == other.isReliable &&
                    this.theSequenceNumber == other.theSequenceNumber;
            }
        }

        /// <summary>
        /// Determines whether the current and supplied dictionary keys are equal
        /// </summary>
        /// <param name="obj">The dictionary key to be compared with the current dictionary key</param>
        /// <returns>Boolean flag that indicates whether the two dictionary keys are equal</returns>
        public override bool Equals(object obj)
        {
            // If the supplied dictionary key is null then return false
            if (object.ReferenceEquals(obj, null))
            {
                return false;
            }

            // If the supplied dictionary key has a different type to the current dictionary key then return false
            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            if (obj is DictionaryKey)
            {
                return this.Equals(obj);
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
                this.theHostId.GetHashCode() ^
                this.isReliable.GetHashCode() ^
                this.theSequenceNumber.GetHashCode();
        }
    }
}