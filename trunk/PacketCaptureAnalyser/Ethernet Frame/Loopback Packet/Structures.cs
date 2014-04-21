// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.LoopbackPacket
{
    class Structures
    {
        /// <summary>
        /// Loopback packet header - 6 bytes
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.HeaderLength)]
        public struct HeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public ushort SkipCount;

            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort Function;

            [System.Runtime.InteropServices.FieldOffset(4)]
            public ushort ReceiptNumber;
        }
    }
}