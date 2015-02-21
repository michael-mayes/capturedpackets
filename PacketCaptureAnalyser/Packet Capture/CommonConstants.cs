// <copyright file="CommonConstants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.PacketCapture
{
    /// <summary>
    /// This class provides constants for use by the common packet capture processing
    /// </summary>
    public static class CommonConstants
    {        
        /// <summary>
        /// Network data link type/Network encapsulation type
        /// </summary>
        public enum NetworkDataLinkType
        {
            /// <summary>
            /// Null/Loopback network data link type
            /// </summary>
            NullLoopback = 0,

            /// <summary>
            /// Ethernet network data link type
            /// </summary>
            Ethernet = 1,

            /// <summary>
            /// Cisco HDLC (Proprietary) network data link type
            /// </summary>
            CiscoHDLC = 104,

            /// <summary>
            /// Invalid network data link type
            /// </summary>
            Invalid = ushort.MaxValue
        }
    }
}