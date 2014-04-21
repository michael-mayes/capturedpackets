// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.UDPDatagram
{
    class Constants
    {
        //// UDP datagram header - 8 bytes

        // Length
        public const ushort HeaderLength = 8;

        /// <summary>
        /// Port number
        /// </summary>
        public enum PortNumber : ushort
        {
            /// <summary>
            /// Minimum value
            /// </summary>
            DummyValueMin = 0,

            /// <summary>
            /// Maximum value
            /// </summary>
            DummyValueMax = 65535
        }
    }
}