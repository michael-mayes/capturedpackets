// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.LoopbackPacket
{
    class Constants
    {
        //// Loopback packet header - 6 bytes 

        // Length
        public const ushort HeaderLength = 6;

        //// Loopback packet payload

        // Length
        public const ushort PayloadLength = 40;
    }
}