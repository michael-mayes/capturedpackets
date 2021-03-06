// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.PacketCapture.SnifferPackageCapture
{
    /// <summary>
    /// This class provides constants for use by the Sniffer packet capture processing
    /// </summary>
    public static class Constants
    {
        //// Sniffer packet capture global header

        /// <summary>
        /// Sniffer packet capture global header length
        /// </summary>
        public const ushort GlobalHeaderLength = 41;

        //// Magic numbers - provided in big endian (network) representation

        /// <summary>
        /// Highest four bytes containing ASCII characters "TRSN" to start the magic number (used for identifying the packet capture type)
        /// </summary>
        public const uint ExpectedMagicNumberHighest = 0x4E535254;

        /// <summary>
        /// High eight bytes containing ASCII characters "TRSNIFF " to start the magic number
        /// </summary>
        public const ulong ExpectedMagicNumberHigh = 0x204646494E535254;

        /// <summary>
        /// Low eight bytes containing ASCII characters "data    " to continue the magic number
        /// </summary>
        public const ulong ExpectedMagicNumberLow = 0x2020202061746164;

        /// <summary>
        /// Terminating byte ASCII control character 'SUB' completes the magic number
        /// </summary>
        public const byte ExpectedMagicNumberTerminator = 0x1A;

        /// <summary>
        /// Sniffer packet capture global header major version number
        /// </summary>
        public const short ExpectedVersionMajor = 4;

        /// <summary>
        /// Sniffer packet capture global header major version number
        /// </summary>
        public const short ExpectedVersionMinor = 0;

        /// <summary>
        /// Type of records that follow the Sniffer packet capture global header in the packet capture
        /// </summary>
        public const sbyte ExpectedType = 4; // Sniffer type 2 data records

        /// <summary>
        /// Sniffer packet capture global header format version number
        /// </summary>
        public const sbyte ExpectedFormatVersion = 1; // Uncompressed

        //// Sniffer packet capture record header

        /// <summary>
        /// Sniffer packet capture record header length
        /// </summary>
        public const ushort RecordHeaderLength = 6;

        //// Sniffer packet capture Sniffer type 2 data record

        /// <summary>
        /// Sniffer packet capture Sniffer type 2 data record length
        /// </summary>
        public const ushort SnifferType2RecordLength = 14;

        /// <summary>
        /// Record type
        /// </summary>
        public enum RecordHeaderSnifferRecordType
        {
            /// <summary>
            /// Default value
            /// </summary>
            None = 0,

            /// <summary>
            /// Version record type
            /// </summary>
            VersionRecordType = 1,

            /// <summary>
            /// Type 2 data record type
            /// </summary>
            Type2RecordType = 4,

            /// <summary>
            /// End of file record type
            /// </summary>
            EndOfFileRecordType = 3
        }
    }
}