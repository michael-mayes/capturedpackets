// $Id: Constants.cs 231 2014-07-06 23:05:41Z michaelmayes $
// $URL: https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/RARP%20Packet/Constants.cs $
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.RARPPacket
{
    /// <summary>
    /// This class provides constants for use by the RARP packet processing
    /// </summary>
    public class Constants
    {
        //// RARP packet

        /// <summary>
        /// RARP packet length
        /// </summary>
        public const ushort PacketLength = 28;
    }
}
