//This is free and unencumbered software released into the public domain.

//Anyone is free to copy, modify, publish, use, compile, sell, or
//distribute this software, either in source code form or as a compiled
//binary, for any purpose, commercial or non-commercial, and by any
//means.

//In jurisdictions that recognize copyright laws, the author or authors
//of this software dedicate any and all copyright interest in the
//software to the public domain. We make this dedication for the benefit
//of the public at large and to the detriment of our heirs and
//successors. We intend this dedication to be an overt act of
//relinquishment in perpetuity of all present and future rights to this
//software under copyright law.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.

//For more information, please refer to <http://unlicense.org/>

namespace EthernetFrameNamespace.IPv4PacketNamespace.TCPPacketNamespace
{
    class TCPPacketStructures
    {
        //
        //TCP packet header - 20 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = TCPPacketConstants.TCPPacketHeaderMinimumLength)]

        public struct TCPPacketHeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt16 SourcePort; //Source port number

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 DestinationPort; //Destination port number

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 SequenceNumber; //Sequence number

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt32 AcknowledgmentNumber; //Acknowledgment number

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.Byte DataOffsetAndReservedAndNSFlag; //TCP packet header length, reserved fields and NS flag

            [System.Runtime.InteropServices.FieldOffset(13)]
            public System.Byte Flags; //Other flags (including ACK, PSH, RST, SYN and FIN)

            [System.Runtime.InteropServices.FieldOffset(14)]
            public System.UInt16 WindowSize; //Window size

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.UInt16 Checksum; //Checksum - includes TCP packer header and TCP packet data payload as well as an IPv4 "pseudo header"

            [System.Runtime.InteropServices.FieldOffset(18)]
            public System.UInt16 UrgentPointer; //Urgent pointer

            //There may be a options field of 0 – 40 bytes at the end of the structure dependent on the value of the TCP packet header length field
        }
    }
}