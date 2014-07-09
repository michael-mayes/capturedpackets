// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCapture.PCAPPackageCapture
{
    /// <summary>
    /// This class provides structures for use by the PCAP packet capture processing
    /// </summary>
    public class Structures
    {
        /// <summary>
        /// PCAP packet capture global header
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.GlobalHeaderLength)]
        public struct GlobalHeaderStructure
        {
            /// <summary>
            /// Magic number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint MagicNumber;

            /// <summary>
            /// Major version number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public ushort VersionMajor;

            /// <summary>
            /// Minor version number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(6)]
            public ushort VersionMinor;

            /// <summary>
            /// GMT to local correction
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public int ThisTimeZone;

            /// <summary>
            /// Accuracy of timestamps
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public uint SignificantFigures;

            /// <summary>
            /// Max length of packet capture, in octets
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(16)]
            public uint SnapshotLength;

            /// <summary>
            /// Network data link type
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(20)]
            public uint NetworkDataLinkType;
        }

        /// <summary>
        /// PCAP packet header
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]
        public struct HeaderStructure
        {
            /// <summary>
            /// Timestamp seconds
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint TimestampSeconds;

            /// <summary>
            /// Timestamp microseconds
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public uint TimestampMicroseconds;

            /// <summary>
            /// Number of octets of packet saved in packet capture
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public uint SavedLength;

            /// <summary>
            /// Actual length of packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public uint ActualLength;
        }
    }
}
