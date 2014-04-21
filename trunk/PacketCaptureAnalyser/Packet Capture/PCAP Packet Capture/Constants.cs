// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace PacketCapture.PCAPPackageCapture
{
    class Constants
    {
        //// PCAP packet capture global header - 24 bytes

        // Length
        public const ushort GlobalHeaderLength = 24;

        // Magic number
        public const uint LittleEndianMagicNumber = 0xa1b2c3d4;
        public const uint BigEndianMagicNumber = 0xd4c3b2a1;

        // Version numbers
        public const ushort ExpectedVersionMajor = 2;
        public const ushort ExpectedVersionMinor = 4;

        //// PCAP packet capture packet header - 16 bytes

        // Length
        public const ushort HeaderLength = 16;
    }
}