// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.EthernetFrame.LoopbackPacket
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
        /// Processes a Loopback packet
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <returns>Boolean flag that indicates whether the Loopback packet could be processed</returns>
        public bool Process(long theEthernetFrameLength)
        {
            bool theResult = true;

            // Validate the Loopback packet
            theResult = this.ValidateLoopbackPacket(
                theEthernetFrameLength);

            if (theResult)
            {
                // Process the Loopback packet header
                this.ProcessLoopbackPacketHeader();

                // Process the payload of the Loopback packet
                this.ProcessLoopbackPacketPayload();
            }

            return theResult;
        }

        /// <summary>
        /// Processes the Loopback packet header
        /// </summary>
        private void ProcessLoopbackPacketHeader()
        {
            // Just read off the bytes for the Loopback packet header from the packet capture so we can move on
            this.theBinaryReader.ReadBytes(
                Constants.HeaderLength);
        }

        /// <summary>
        /// Processes the payload of the Loopback packet
        /// </summary>
        private void ProcessLoopbackPacketPayload()
        {
            // Just read off the remaining bytes of the Loopback packet from the packet capture so we can move on
            // The remaining length to read off is the length of the payload of the Loopback packet
            this.theBinaryReader.ReadBytes(
                Constants.PayloadLength);
        }

        /// <summary>
        /// Validates the Loopback packet
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <returns>Boolean flag that indicates whether the Loopback packet is valid</returns>
        private bool ValidateLoopbackPacket(long theEthernetFrameLength)
        {
            bool theResult = true;

            if (theEthernetFrameLength < (Constants.HeaderLength + Constants.PayloadLength))
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The length of the Ethernet frame is lower than the length of the Loopback packet!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
