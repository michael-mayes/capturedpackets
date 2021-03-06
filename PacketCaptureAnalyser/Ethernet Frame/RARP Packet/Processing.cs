// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.EthernetFrame.RARPPacket
{
    /// <summary>
    /// This class provides the RARP packet processing
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
        /// Process a RARP packet
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <returns>Boolean flag that indicates whether the RARP packet could be processed</returns>
        public bool ProcessRARPPacket(long theEthernetFrameLength)
        {
            bool theResult = true;

            // Validate the RARP packet
            theResult = this.ValidateRARPPacket(
                theEthernetFrameLength);

            if (theResult)
            {
                // There is no separate header for the RARP packet

                // Processes the payload of the RARP packet
                this.ProcessRARPPacketPayload();
            }

            return theResult;
        }

        /// <summary>
        /// Processes the payload of the RARP packet
        /// </summary>
        private void ProcessRARPPacketPayload()
        {
            // Just read off the bytes for the RARP packet from the packet capture so we can move on
            this.theBinaryReader.ReadBytes(
                Constants.PacketLength);
        }

        /// <summary>
        /// Validates the RARP packet
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <returns>Boolean flag that indicates whether the RARP packet is valid</returns>
        private bool ValidateRARPPacket(long theEthernetFrameLength)
        {
            bool theResult = true;

            if (theEthernetFrameLength < Constants.PacketLength)
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The length of the Ethernet frame is lower than the length of the RARP packet!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
