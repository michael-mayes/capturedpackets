// <copyright file="MainWindowFormConstants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer
{
    /// <summary>
    /// This class provides constants for use by the main window form processing
    /// </summary>
    public static class MainWindowFormConstants
    {
        /// <summary>
        /// The expected set of fixed extensions for a PCAP Next Generation packet capture
        /// </summary>
        private static string[] theExpectedPCAPNextGenerationPacketCaptureFileExtensions = { ".pcapng", ".ntar" };

        /// <summary>
        /// The expected set of fixed extensions for a PCAP packet capture
        /// </summary>
        private static string[] theExpectedPCAPPacketCaptureFileExtensions = { ".pcap", ".libpcap", ".cap" };

        /// <summary>
        /// The expected set of fixed extensions for an NA Sniffer (DOS) packet capture
        /// </summary>
        private static string[] theExpectedNASnifferDOSPacketCaptureFileExtensions = { ".enc" };

        /// <summary>
        /// Enumerated list of the types of packet captures supported by the main window form
        /// </summary>
        public enum PacketCaptureType
        {
            /// <summary>
            /// PCAP Next Generation capture
            /// </summary>
            PCAPNextGeneration = 0,

            /// <summary>
            /// PCAP packet capture
            /// </summary>
            PCAP = 1,

            /// <summary>
            /// NA Sniffer (DOS) packet capture
            /// </summary>
            NASnifferDOS = 2,

            /// <summary>
            /// Invalid value for type of packet capture
            /// </summary>
            Incorrect = 3,

            /// <summary>
            /// Unknown value for type of packet capture
            /// </summary>
            Unknown = 4
        }

        /// <summary>
        /// Gets the expected set of fixed extensions for a PCAP Next Generation packet capture
        /// </summary>
        public static string[] ExpectedPCAPNextGenerationPacketCaptureFileExtensions
        {
            get
            {
                return theExpectedPCAPNextGenerationPacketCaptureFileExtensions;
            }
        }

        /// <summary>
        /// Gets the expected set of fixed extensions for a PCAP packet capture
        /// </summary>
        public static string[] ExpectedPCAPPacketCaptureFileExtensions
        {
            get
            {
                return theExpectedPCAPPacketCaptureFileExtensions;
            }
        }

        /// <summary>
        /// Gets the expected set of fixed extensions for an NA Sniffer (DOS) packet capture
        /// </summary>
        public static string[] ExpectedNASnifferDOSPacketCaptureFileExtensions
        {
            get
            {
                return theExpectedNASnifferDOSPacketCaptureFileExtensions;
            }
        }
    }
}