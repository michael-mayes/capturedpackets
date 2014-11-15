// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame
{
    /// <summary>
    /// This class provides structures for use by the Ethernet frame processing
    /// </summary>
    public class Structures
    {
        /// <summary>
        /// Ethernet frame header
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "SourceMACAddressHigh", Justification = "Only applies to the IA-64 target architecture which is not used by this application"), System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]
        public struct HeaderStructure
        {
            /// <summary>
            /// The high bytes of the media address of the intended receiver (four bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public uint DestinationMACAddressHigh;

            /// <summary>
            /// The low bytes of the media address of the intended receiver (two bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public ushort DestinationMACAddressLow;

            /// <summary>
            /// The high bytes of the media address of the sender (four bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(6)]
            public uint SourceMACAddressHigh;

            /// <summary>
            /// The low bytes of the media address of the sender (two bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(10)]
            public ushort SourceMACAddressLow;

            /// <summary>
            /// The type of the Ethernet frame
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public ushort EthernetFrameType;
        }
    }
}