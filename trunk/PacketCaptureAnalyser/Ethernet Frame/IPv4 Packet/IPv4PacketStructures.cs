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

namespace EthernetFrameNamespace.IPv4PacketNamespace
{
    class IPv4PacketStructures
    {
        //
        //IPv4 packet header - 20 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = IPv4PacketConstants.IPv4PacketHeaderLength)]

        public struct IPv4PacketHeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.Byte VersionAndHeaderLength; //IP version and IPv4 packet header length

            [System.Runtime.InteropServices.FieldOffset(1)]
            public System.Byte TypeOfService; //Type of IP service

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 TotalLength; //Total length of the IPv4 packet

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt16 Identifier; //IP unique identifier

            [System.Runtime.InteropServices.FieldOffset(6)]
            public System.UInt16 FlagsAndOffset; //IP header flags and fragment offset

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.Byte TimeToLive; //IP Time To Live

            [System.Runtime.InteropServices.FieldOffset(9)]
            public System.Byte Protocol; //IPv4 packet protocol (ICMP, IGMP, TCP, UDP etc)

            [System.Runtime.InteropServices.FieldOffset(10)]
            public System.UInt16 HeaderChecksum; //IPv4 packet header checksum - includes IPv4 packet header only

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.Int32 SourceAddress; //Source IP address

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.Int32 DestinationAddress; //Destination IP address
        }
    }
}