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
        public static System.Collections.ObjectModel.ReadOnlyCollection<string> ExpectedPCAPNextGenerationPacketCaptureFileExtensions
        {
            get
            {
                //// Set up the expected set of fixed extensions for a PCAP Next Generation packet capture
                
                System.Collections.ObjectModel.ReadOnlyCollection<string>
                    theExpectedPCAPNextGenerationPacketCaptureFileExtensions;

                System.Collections.Generic.List<string> theExpectedPCAPNextGenerationPacketCaptureFileExtensionsList =
                    new System.Collections.Generic.List<string>();

                theExpectedPCAPNextGenerationPacketCaptureFileExtensionsList.Add(".pcapng");
                theExpectedPCAPNextGenerationPacketCaptureFileExtensionsList.Add(".ntar");

                theExpectedPCAPNextGenerationPacketCaptureFileExtensions =
                    new System.Collections.ObjectModel.ReadOnlyCollection<string>(
                        theExpectedPCAPNextGenerationPacketCaptureFileExtensionsList);

                return theExpectedPCAPNextGenerationPacketCaptureFileExtensions;
            }
        }

        /// <summary>
        /// Gets the expected set of fixed extensions for a PCAP packet capture
        /// </summary>
        public static System.Collections.ObjectModel.ReadOnlyCollection<string> ExpectedPCAPPacketCaptureFileExtensions
        {
            get
            {
                //// Set up the expected set of fixed extensions for a PCAP packet capture
                
                System.Collections.ObjectModel.ReadOnlyCollection<string>
                    theExpectedPCAPPacketCaptureFileExtensions;

                System.Collections.Generic.List<string> theExpectedPCAPPacketCaptureFileExtensionsList =
                    new System.Collections.Generic.List<string>();

                theExpectedPCAPPacketCaptureFileExtensionsList.Add(".pcap");
                theExpectedPCAPPacketCaptureFileExtensionsList.Add(".libpcap");
                theExpectedPCAPPacketCaptureFileExtensionsList.Add(".cap");

                theExpectedPCAPPacketCaptureFileExtensions =
                    new System.Collections.ObjectModel.ReadOnlyCollection<string>(
                        theExpectedPCAPPacketCaptureFileExtensionsList);

                return theExpectedPCAPPacketCaptureFileExtensions;
            }
        }

        /// <summary>
        /// Gets the expected set of fixed extensions for an NA Sniffer (DOS) packet capture
        /// </summary>
        public static System.Collections.ObjectModel.ReadOnlyCollection<string> ExpectedNASnifferDOSPacketCaptureFileExtensions
        {
            get
            {
                //// Set up the expected set of fixed extensions for a NA Sniffer (DOS) packet capture
                
                System.Collections.ObjectModel.ReadOnlyCollection<string>
                    theExpectedNASnifferDOSPacketCaptureFileExtensions;

                System.Collections.Generic.List<string> theExpectedNASnifferDOSPacketCaptureFileExtensionsList =
                    new System.Collections.Generic.List<string>();

                theExpectedNASnifferDOSPacketCaptureFileExtensionsList.Add(".enc");

                theExpectedNASnifferDOSPacketCaptureFileExtensions =
                    new System.Collections.ObjectModel.ReadOnlyCollection<string>(
                        theExpectedNASnifferDOSPacketCaptureFileExtensionsList);

                return theExpectedNASnifferDOSPacketCaptureFileExtensions;
            }
        }
    }
}