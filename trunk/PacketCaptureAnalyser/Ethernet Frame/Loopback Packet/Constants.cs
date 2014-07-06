// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.LoopbackPacket
{
    /// <summary>
    /// This class provides constants for use by the Loopback packet processing
    /// </summary>
    public class Constants
    {
        //// Loopback packet header

        /// <summary>
        /// Loopback packet header length
        /// </summary>
        public const ushort HeaderLength = 6;

        //// Loopback packet payload

        /// <summary>
        /// Loopback packet payload length
        /// </summary>
        public const ushort PayloadLength = 40;
    }
}
