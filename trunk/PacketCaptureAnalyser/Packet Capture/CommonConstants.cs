// $Id$
// $URL$
// <copyright file="CommonConstants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace PacketCapture
{
    class CommonConstants
    {
        /// <summary>
        /// Network data link type/Network encapsulation type
        /// </summary>
        public enum NetworkDataLinkType : ushort
        {
            /// <summary>
            /// Null/Loopback network data link type
            /// </summary>
            NullLoopBack = 0,

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