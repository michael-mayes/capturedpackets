// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.PacketCapture.SnifferPackageCapture
{
    /// <summary>
    /// This class provides structures for use by the Sniffer packet capture processing
    /// </summary>
    public static class Structures
    {
        /// <summary>
        /// Sniffer packet capture global header
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "Reserved", Justification = "Only applies to the IA-64 target architecture which is not used by this application"),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "VersionMajor", Justification = "Only applies to the IA-64 target architecture which is not used by this application"),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "VersionMinor", Justification = "Only applies to the IA-64 target architecture which is not used by this application"),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "Time", Justification = "Only applies to the IA-64 target architecture which is not used by this application"),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "RecordType", Justification = "Only applies to the IA-64 target architecture which is not used by this application"),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "RecordLength", Justification = "Only applies to the IA-64 target architecture which is not used by this application"),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "Date", Justification = "Only applies to the IA-64 target architecture which is not used by this application"),
        System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.GlobalHeaderLength)]
        public struct GlobalHeaderStructure
        {
            /// <summary>
            /// High bytes of the magic number (eight bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public ulong MagicNumberHigh;

            /// <summary>
            /// Low bytes of the magic number (eight bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public ulong MagicNumberLow;

            /// <summary>
            /// Terminating byte for the magic number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(16)]
            public byte MagicNumberTerminator;

            /// <summary>
            /// Type of version record
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(17)]
            public ushort RecordType;

            /// <summary>
            /// Length of version record - only the first two bytes are length, the latter two are "reserved" and so not processed
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(19)]
            public uint RecordLength;

            /// <summary>
            /// Major version number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(23)]
            public short VersionMajor;

            /// <summary>
            /// Minor version number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(25)]
            public short VersionMinor;

            /// <summary>
            /// DOS-format time
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(27)]
            public short Time;

            /// <summary>
            /// DOS-format date
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(29)]
            public short Date;

            /// <summary>
            /// Type of records that follow the Sniffer packet capture global header in the packet capture
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(31)]
            public sbyte Type;

            /// <summary>
            /// Type of network encapsulation for records that follow the Sniffer packet capture global header in the packet capture
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(32)]
            public byte NetworkEncapsulationType;

            /// <summary>
            /// Format version
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(33)]
            public sbyte FormatVersion;

            /// <summary>
            /// Timestamp units
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(34)]
            public byte TimestampUnits;

            /// <summary>
            /// Compression version
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(35)]
            public sbyte CompressionVersion;

            /// <summary>
            /// Compression level
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(36)]
            public sbyte CompressionLevel;

            /// <summary>
            /// Reserved field
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(37)]
            public int Reserved;
        }

        /// <summary>
        /// Sniffer packet capture record header
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "RecordLength", Justification = "Only applies to IA-64 which is not used by this application"), System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.RecordHeaderLength)]
        public struct RecordHeaderStructure
        {
            /// <summary>
            /// Record type
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public ushort RecordType;

            /// <summary>
            /// Record length
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(2)]
            public uint RecordLength;
        }

        /// <summary>
        /// Sniffer packet capture Sniffer type 2 data record
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.SnifferType2RecordLength)]
        public struct SnifferType2RecordStructure
        {
            /// <summary>
            /// Low bytes of timestamp
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public ushort TimestampLow;

            /// <summary>
            /// Middle bytes of timestamp
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort TimestampMiddle;

            /// <summary>
            /// High bytes of the timestamp
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public byte TimestampHigh;

            /// <summary>
            /// Time in days since start of capture
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(5)]
            public byte TimeDays;

            /// <summary>
            /// Number of bytes of data
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(6)]
            public short Size;

            /// <summary>
            /// Frame error status bits
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public byte FrameErrorStatusBits;

            /// <summary>
            /// Buffer flags
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(9)]
            public byte Flags;

            /// <summary>
            /// Size of original frame in bytes
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(10)]
            public short TrueSize;

            /// <summary>
            /// Reserved field
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public short Reserved;
        }
    }
}