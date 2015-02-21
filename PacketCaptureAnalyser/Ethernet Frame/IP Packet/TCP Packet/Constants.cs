// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.EthernetFrame.IPPacket.TCPPacket
{
    /// <summary>
    /// This class provides constants for use by the TCP packet processing
    /// </summary>
    public static class Constants
    {
        //// TCP packet header

        /// <summary>
        /// TCP packet header minimum length
        /// </summary>
        public const ushort HeaderMinimumLength = 20;

        /// <summary>
        /// TCP packet header maximum length
        /// </summary>
        public const ushort HeaderMaximumLength = 60;

        //// TODO Put extra port numbers in the following enumeration to
        //// identify and process specific messages within the TCP packet

        /// <summary>
        /// TCP packet header port number
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