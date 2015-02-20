// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.PacketCapture.PCAPPackageCapture
{
    /// <summary>
    /// This class provides constants for use by the PCAP packet capture processing
    /// </summary>
    public static class Constants
    {
        //// PCAP packet capture global header

        /// <summary>
        /// PCAP packet capture global header length
        /// </summary>
        public const ushort GlobalHeaderLength = 24;

        /// <summary>
        /// PCAP packet capture global header magic number (little endian representation)
        /// </summary>
        public const uint LittleEndianMagicNumber = 0xa1b2c3d4;

        /// <summary>
        /// PCAP packet capture global header magic number (big endian representation)
        /// </summary>
        public const uint BigEndianMagicNumber = 0xd4c3b2a1;

        /// <summary>
        /// PCAP packet capture global header major version number
        /// </summary>
        public const ushort ExpectedVersionMajor = 2;

        /// <summary>
        /// PCAP packet capture global header minor version number
        /// </summary>
        public const ushort ExpectedVersionMinor = 4;

        //// PCAP packet header

        /// <summary>
        /// PCAP packet header length
        /// </summary>
        public const ushort HeaderLength = 16;
    }
}