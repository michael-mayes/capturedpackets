// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.LoopbackPacket
{
    /// <summary>
    /// This class provides structures for use by the Loopback packet processing
    /// </summary>
    public class Structures
    {
        /// <summary>
        /// Loopback packet header
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]
        public struct HeaderStructure
        {
            /// <summary>
            /// Skip Count
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public ushort SkipCount;

            /// <summary>
            /// Function of the Loopback packet
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort Function;

            /// <summary>
            /// Receipt Number
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public ushort ReceiptNumber;
        }
    }
}