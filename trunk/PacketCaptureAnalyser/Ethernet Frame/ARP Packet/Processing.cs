// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.EthernetFrame.ARPPacket
{
    /// <summary>
    /// This class provides the ARP packet processing
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
        /// The reusable instance of the ARP packet
        /// </summary>
        private Structures.PacketStructure theARPPacket;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="theBinaryReader">The object that provides for binary reading from the packet capture</param>
        public Processing(Analysis.DebugInformation theDebugInformation, System.IO.BinaryReader theBinaryReader)
        {
            this.theDebugInformation = theDebugInformation;

            this.theBinaryReader = theBinaryReader;

            // Create an instance of the ARP packet
            this.theARPPacket = new Structures.PacketStructure();
        }

        /// <summary>
        /// Process an ARP packet
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <returns>Boolean flag that indicates whether the ARP packet could be processed</returns>
        public bool ProcessARPPacket(long theEthernetFrameLength)
        {
            bool theResult = true;

            // Validate the ARP packet
            theResult = this.ValidateARPPacket(
                theEthernetFrameLength);

            if (theResult)
            {
                // There is no separate header for the ARP packet

                // Processes the payload of the ARP packet
                this.ProcessARPPacketPayload();
            }

            return theResult;
        }

        /// <summary>
        /// Processes the payload of the ARP packet
        /// </summary>
        private void ProcessARPPacketPayload()
        {
            // Just read off the bytes for the ARP packet from the packet capture so we can move on
            this.theARPPacket.HardwareType = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theARPPacket.ProtocolType = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theARPPacket.HardwareAddressLength = this.theBinaryReader.ReadByte();
            this.theARPPacket.ProtocolAddressLength = this.theBinaryReader.ReadByte();
            this.theARPPacket.Operation = (ushort)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt16());
            this.theARPPacket.SenderHardwareAddressHigh = this.theBinaryReader.ReadUInt32();
            this.theARPPacket.SenderHardwareAddressLow = this.theBinaryReader.ReadUInt16();
            this.theARPPacket.SenderProtocolAddress = (uint)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt32());
            this.theARPPacket.TargetHardwareAddressHigh = this.theBinaryReader.ReadUInt32();
            this.theARPPacket.TargetHardwareAddressLow = this.theBinaryReader.ReadUInt16();
            this.theARPPacket.TargetProtocolAddress = (uint)System.Net.IPAddress.NetworkToHostOrder(this.theBinaryReader.ReadInt32());
        }

        /// <summary>
        /// Validates the ARP packet
        /// </summary>
        /// <param name="theEthernetFrameLength">The length of the Ethernet frame</param>
        /// <returns>Boolean flag that indicates whether the ARP packet is valid</returns>
        private bool ValidateARPPacket(long theEthernetFrameLength)
        {
            bool theResult = true;

            if (theEthernetFrameLength < Constants.PacketLength)
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The length of the Ethernet frame is lower than the length of the ARP packet!!!");

                theResult = false;
            }

            return theResult;
        }
    }
}
