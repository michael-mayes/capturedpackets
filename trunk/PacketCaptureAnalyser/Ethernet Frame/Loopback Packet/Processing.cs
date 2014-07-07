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
        /// 
        /// </summary>
        private Analysis.DebugInformation theDebugInformation;

        /// <summary>
        /// 
        /// </summary>
        private System.IO.BinaryReader theBinaryReader;

        /// <summary>
        /// 
        /// </summary>
        private Structures.HeaderStructure theHeader;

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
            this.theHeader = new Structures.HeaderStructure();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theEthernetFrameLength">The length read from the packet capture for the Ethernet frame</param>
        /// <returns></returns>
        public bool Process(long theEthernetFrameLength)
        {
            bool theResult = true;

            if (theEthernetFrameLength < (Constants.HeaderLength + Constants.PayloadLength))
            {
                this.theDebugInformation.WriteErrorEvent("The length of the Ethernet frame is lower than the length of the Loopback packet!!!");

                return false;
            }

            // Process the Loopback packet header
            theResult = this.ProcessHeader();

            // Just read off the remaining bytes of the Loopback packet from the packet capture so we can move on
            // The remaining length is the length for the Loopback packet payload
            this.theBinaryReader.ReadBytes(Constants.PayloadLength);

            return theResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ProcessHeader()
        {
            bool theResult = true;

            // Read the values for the Loopback packet header from the packet capture
            this.theHeader.SkipCount = this.theBinaryReader.ReadUInt16();
            this.theHeader.Function = this.theBinaryReader.ReadUInt16();
            this.theHeader.ReceiptNumber = this.theBinaryReader.ReadUInt16();

            return theResult;
        }
    }
}
