// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.EthernetFrame.LLDPPacket
{
    /// <summary>
    /// This class provides the LLDP packet processing
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
        /// Processes an LLDP packet
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <returns>Boolean flag that indicates whether the LLDP packet could be processed</returns>
        public bool ProcessLLDPPacket(long theEthernetFrameLength)
        {
            bool theResult = true;

            // Validate the LLDP packet
            theResult = this.ValidateLLDPPacket(
                theEthernetFrameLength);

            if (theResult)
            {
                // There is no separate header for the LLDP packet

                // Process the payload of the LLDP packet
                this.ProcessLLDPPacketPayload();
            }

            return theResult;
        }

        /// <summary>
        /// Processes the payload of the LLDP packet
        /// </summary>
        private void ProcessLLDPPacketPayload()
        {
            // Just read off the bytes for the LLDP packet from the packet capture so we can move on
            this.theBinaryReader.ReadBytes(
			    Constants.PacketLength);
        }

        /// <summary>
        /// Validates the LLDP packet
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <returns>Boolean flag that indicates whether the LLDP packet is valid</returns>
        private bool ValidateLLDPPacket(long theEthernetFrameLength)
        {
            bool theResult = true;

            if (theEthernetFrameLength < Constants.PacketLength)
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The length of the Ethernet frame is lower than the length of the LLDP packet!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
