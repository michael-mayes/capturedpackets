// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.EthernetFrame.IPPacket.UDPDatagram
{
    /// <summary>
    /// This class provides constants for use by the UDP datagram processing
    /// </summary>
    public static class Constants
    {
        //// UDP datagram header

        /// <summary>
        /// UDP datagram header length
        /// </summary>
        public const ushort HeaderLength = 8;

        /// <summary>
        /// Port number
        /// </summary>
        public enum PortNumber
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