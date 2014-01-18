//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace EthernetFrame.IPPacket.TCPPacket
{
    class Processing
    {
        private System.IO.BinaryReader TheBinaryReader;

        private Structures.HeaderStructure TheHeader;

        private bool PerformLatencyAnalysisProcessing;
        private Analysis.LatencyAnalysis.Processing TheLatencyAnalysisProcessing;

        private bool PerformTimeAnalysisProcessing;
        private Analysis.TimeAnalysis.Processing TheTimeAnalysisProcessing;

        public Processing(System.IO.BinaryReader TheBinaryReader, bool PerformLatencyAnalysisProcessing, Analysis.LatencyAnalysis.Processing TheLatencyAnalysisProcessing, bool PerformTimeAnalysisProcessing, Analysis.TimeAnalysis.Processing TheTimeAnalysisProcessing)
        {
            this.TheBinaryReader = TheBinaryReader;

            //Create an instance of the TCP packet header
            TheHeader = new Structures.HeaderStructure();

            this.PerformLatencyAnalysisProcessing = PerformLatencyAnalysisProcessing;
            this.TheLatencyAnalysisProcessing = TheLatencyAnalysisProcessing;

            this.PerformTimeAnalysisProcessing = PerformTimeAnalysisProcessing;
            this.TheTimeAnalysisProcessing = TheTimeAnalysisProcessing;
        }

        public bool Process(ulong ThePacketNumber, double TheTimestamp, ushort TheLength)
        {
            bool TheResult = true;

            ushort ThePayloadLength = 0;

            ushort TheSourcePort = 0;
            ushort TheDestinationPort = 0;

            //Process the TCP packet header
            TheResult = ProcessHeader(TheLength, out ThePayloadLength, out TheSourcePort, out TheDestinationPort);

            if (TheResult)
            {
                //Process the payload of the TCP packet, supplying the length of the payload and the values for the source port and the destination port as returned by the processing of the TCP packet header
                TheResult = ProcessPayload(ThePacketNumber, TheTimestamp, ThePayloadLength, TheSourcePort, TheDestinationPort);
            }

            return TheResult;
        }

        private bool ProcessHeader(ushort TheLength, out ushort ThePayloadLength, out ushort TheSourcePort, out ushort TheDestinationPort)
        {
            bool TheResult = true;

            //Provide a default value for the output parameter for the length of the payload of the TCP packet
            ThePayloadLength = 0;

            //Provide default values for the output parameters for source port and destination port
            TheSourcePort = 0;
            TheDestinationPort = 0;

            //Read the values for the TCP packet header from the packet capture
            TheHeader.SourcePort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.DestinationPort = (System.UInt16)System.Net.IPAddress.NetworkToHostOrder(TheBinaryReader.ReadInt16());
            TheHeader.SequenceNumber = TheBinaryReader.ReadUInt32();
            TheHeader.AcknowledgmentNumber = TheBinaryReader.ReadUInt32();
            TheHeader.DataOffsetAndReservedAndNSFlag = TheBinaryReader.ReadByte();
            TheHeader.Flags = TheBinaryReader.ReadByte();
            TheHeader.WindowSize = TheBinaryReader.ReadUInt16();
            TheHeader.Checksum = TheBinaryReader.ReadUInt16();
            TheHeader.UrgentPointer = TheBinaryReader.ReadUInt16();

            //Determine the length of the TCP packet header
            //Need to first extract the length value from the combined TCP packet header length, reserved fields and NS flag field
            //We want the higher four bits from the combined TCP packet header length, reserved fields and NS flag field (as it's in a big endian representation) so do a bitwise OR with 0xF0 (i.e. 11110000 in binary) and shift down by four bits
            //The extracted length value is the length of the TCP packet header in 32-bit words so multiply by four to get the actual length in bytes of the TCP packet header
            ushort TheHeaderLength = (ushort)(((TheHeader.DataOffsetAndReservedAndNSFlag & 0xF0) >> 4) * 4);

            //Validate the TCP packet header
            TheResult = ValidateHeader(TheHeader, TheHeaderLength);

            if (TheResult)
            {
                //Set up the output parameter for the length of the payload of the TCP packet, which is the total length of the TCP packet minus the length of the TCP packet header just calculated
                ThePayloadLength = (ushort)(TheLength - TheHeaderLength);

                //Set up the output parameters for source port and destination port using the value read from the TCP packet header
                TheSourcePort = TheHeader.SourcePort;
                TheDestinationPort = TheHeader.DestinationPort;

                if (TheHeaderLength > Constants.HeaderMinimumLength)
                {
                    //The TCP packet contains a header length which is greater than the minimum and so contains extra Options bytes at the end (e.g. timestamps from the capture application)

                    //Just read off these remaining Options bytes of the TCP packet header from the packet capture so we can move on
                    TheBinaryReader.ReadBytes(TheHeaderLength - Constants.HeaderMinimumLength);
                }
            }
            else
            {
                ThePayloadLength = 0;
            }

            return TheResult;
        }

        private bool ProcessPayload(ulong ThePacketNumber, double TheTimestamp, ushort ThePayloadLength, ushort TheSourcePort, ushort TheDestinationPort)
        {
            bool TheResult = true;

            //Only process this TCP packet if the payload has a non-zero payload length i.e. it actually includes data so is not part of the three-way handshake or a plain acknowledgement
            if (ThePayloadLength > 0)
            {
                //Change this logic statement to allow identification and processing of specific messages within the TCP packet
                if (false)
                {
                    //Put code here to identify and process specific messages within the TCP packet
                }
                else
                {
                    //Just read off the remaining bytes of the TCP packet from the packet capture so we can move on
                    //The remaining length is the supplied length of the TCP packet payload
                    TheBinaryReader.ReadBytes(ThePayloadLength);
                }
            }

            return TheResult;
        }

        private bool ValidateHeader(Structures.HeaderStructure TheHeader, ushort TheHeaderLength)
        {
            bool TheResult = true;

            if (TheHeaderLength > Constants.HeaderMaximumLength ||
                TheHeaderLength < Constants.HeaderMinimumLength)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The TCP packet contains a header length " +
                    TheHeaderLength.ToString() +
                    " which is outside the range " +
                    Constants.HeaderMinimumLength.ToString() +
                    " to " +
                    Constants.HeaderMaximumLength.ToString()
                    );

                TheResult = false;
            }

            return TheResult;
        }
    }
}