// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.EthernetFrame.DECDNARemoteConsolePacket
{
    /// <summary>
    /// This class provides the DEC DNA Remote Console packet processing
    /// </summary>
    public class Processing
    {
        /// <summary>
        /// The object that provides for the logging of debug information
        /// </summary>
        private Analysis.DebugInformation theDebugInformation;

        /// <summary>
        /// The object that provides for binary reading from the packet capture
        /// </summary>
        private System.IO.BinaryReader theBinaryReader;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        public Processing(Analysis.DebugInformation theDebugInformation, System.IO.BinaryReader theBinaryReader)
        {
            this.theDebugInformation = theDebugInformation;

            this.theBinaryReader = theBinaryReader;
        }

        /// <summary>
        /// Processes a DEC DNA Remote Console packet
        /// </summary>
        /// <returns>Boolean flag that indicates whether the DEC DNA Remote Console packet could be processed</returns>
        public bool ProcessDECDNARemoteConsolePacket()
        {
            bool theResult = true;

            // Process the DEC DNA Remote Console packet header
            ushort theDECDNARemoteConsolePacketPayloadLength =
                this.ProcessDECDNARemoteConsolePacketHeader();

            // Process the payload of the DEC DNA Remote Console packet
            this.ProcessDECDNARemoteConsolePacketPayload(
                theDECDNARemoteConsolePacketPayloadLength);

            return theResult;
        }

        /// <summary>
        /// Processes a DEC DNA Remote Console packet header
        /// </summary>
        /// <returns>The length of the payload of the DEC DNA Remote Console packet</returns>
        private ushort ProcessDECDNARemoteConsolePacketHeader()
        {
            // Read off and return the DEC DNA Remote Console packet header from the packet capture so we can indicate the length of the payload
            return this.theBinaryReader.ReadUInt16();
        }

        /// <summary>
        /// Processes the payload of the DEC DNA Remote Console packet
        /// </summary>
        /// <param name="theDECDNARemoteConsolePacketPayloadLength">The length of the payload of the DEC DNA Remote Console packet</param>
        private void ProcessDECDNARemoteConsolePacketPayload(ushort theDECDNARemoteConsolePacketPayloadLength)
        {
            // Just read off the remaining bytes of the DEC DNA Remote Console packet from the packet capture so we can move on
            this.theBinaryReader.ReadBytes(
                theDECDNARemoteConsolePacketPayloadLength);
        }
    }
}