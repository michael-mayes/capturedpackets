// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.LLDPPacket
{
    /// <summary>
    /// This class provides structures for use by the LLDP packet processing
    /// </summary>
    public class Structures
    {
        /// <summary>
        /// LLDP packet - 46 bytes
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.PacketLength)]
        public struct PacketStructure
        {
            /// <summary>
            /// Unused field
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public ulong UnusedField1;

            /// <summary>
            /// Unused field
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public ulong UnusedField2;

            /// <summary>
            /// Unused field
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(16)]
            public ulong UnusedField3;

            /// <summary>
            /// Unused field
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(24)]
            public ulong UnusedField4;

            /// <summary>
            /// Unused field
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(32)]
            public ulong UnusedField5;

            /// <summary>
            /// Unused field
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(40)]
            public uint UnusedField6;

            /// <summary>
            /// Unused field
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(44)]
            public ushort UnusedField7;
        }
    }
}