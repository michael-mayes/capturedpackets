// $Id$
// $URL$
// <copyright file="Structures.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.ARPPacket
{
    /// <summary>
    /// This class provides structures for use by the ARP packet processing
    /// </summary>
    public class Structures
    {
        /// <summary>
        /// ARP packet - 28 bytes
        /// </summary>
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = Constants.PacketLength)]
        public struct PacketStructure
        {
            /// <summary>
            /// The network protocol type (e.g. Ethernet is 1)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(0)]
            public ushort HardwareType;

            /// <summary>
            /// The internetwork protocol for which the ARP request is intended (e.g. IP v4 is 0x0800) - same possible values as for Ether Type in Ethernet frame header
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(2)]
            public ushort ProtocolType;

            /// <summary>
            /// Length (in octets) of a hardware address (e.g. Ethernet addresses size is 6)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(4)]
            public byte HardwareAddressLength;

            /// <summary>
            /// Length (in octets) of addresses used in the upper layer internetwork protocol (e.g. IP v4 address size is 4)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(5)]
            public byte ProtocolAddressLength;

            /// <summary>
            /// The operation that the sender is performing - 1 for request and 2 for reply
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(6)]
            public ushort Operation;

            /// <summary>
            /// The high bytes of the media address of the sender (four bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(8)]
            public uint SenderHardwareAddressHigh;

            /// <summary>
            /// The low bytes of the media address of the sender (two bytes)
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(12)]
            public ushort SenderHardwareAddressLow;

            /// <summary>
            /// The internetwork address of the sender
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(14)]
            public uint SenderProtocolAddress;

            /// <summary>
            /// The high bytes of the media address of the intended receiver (four bytes) - ignored in requests
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(18)]
            public uint TargetHardwareAddressHigh;

            /// <summary>
            /// The low bytes of the media address of the intended receiver (two bytes) - ignored in requests
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(22)]
            public ushort TargetHardwareAddressLow;

            /// <summary>
            /// The internetwork address of the intended receiver
            /// </summary>
            [System.Runtime.InteropServices.FieldOffset(24)]
            public uint TargetProtocolAddress;
        }
    }
}