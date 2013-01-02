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

namespace EthernetFrameNamespace.IPv4PacketNamespace.IGMPv2PacketNamespace
{
    class IGMPv2PacketStructures
    {
        //
        //IGMPv2 packet header - 8 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = IGMPv2PacketConstants.IGMPv2PacketHeaderLength)]

        public struct IGMPv2PacketStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.Byte Type; //Type of IGMPv2 packet

            [System.Runtime.InteropServices.FieldOffset(1)]
            public System.Byte MaxResponseTime; //Maximum time allowed for response to the IGMPv2 packet - only non-zero for queries

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 Checksum; //Checksum for the IGMPv2 packet
            
            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.UInt32 GroupAddress; //The multicast address being queried by the IGMPv2 packet
        }
    }
}