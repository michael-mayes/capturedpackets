// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.LoopbackPacket
{
    /// <summary>
    /// This class provides the Loopback packet processing
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
        /// The reusable instance of the Loopback packet header
        /// </summary>
        private Structures.HeaderStructure theLoopbackPacketHeader;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        public Processing(Analysis.DebugInformation theDebugInformation, System.IO.BinaryReader theBinaryReader)
        {
            this.theDebugInformation = theDebugInformation;

            this.theBinaryReader = theBinaryReader;

            // Create an instance of the Loopback packet header
            this.theLoopbackPacketHeader = new Structures.HeaderStructure();
        }

        /// <summary>
        /// Processes a Loopback packet
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <returns>Boolean flag that indicates whether the Loopback packet could be processed</returns>
        public bool Process(long theEthernetFrameLength)
        {
            if (theEthernetFrameLength < (Constants.HeaderLength + Constants.PayloadLength))
            {
                this.theDebugInformation.WriteErrorEvent("The length of the Ethernet frame is lower than the length of the Loopback packet!!!");

                return false;
            }

            // Process the Loopback packet header
            this.ProcessLoopbackPacketHeader();

            // Just read off the remaining bytes of the Loopback packet from the packet capture so we can move on
            // The remaining length to read off is the length of the payload of the Loopback packet
            this.theBinaryReader.ReadBytes(Constants.PayloadLength);

            return true;
        }

        /// <summary>
        /// Processes the Loopback packet header
        /// </summary>
        private void ProcessLoopbackPacketHeader()
        {
            // Read the values for the Loopback packet header from the packet capture
            this.theLoopbackPacketHeader.SkipCount = this.theBinaryReader.ReadUInt16();
            this.theLoopbackPacketHeader.Function = this.theBinaryReader.ReadUInt16();
            this.theLoopbackPacketHeader.ReceiptNumber = this.theBinaryReader.ReadUInt16();
        }
    }
}
