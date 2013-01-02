﻿//This is free and unencumbered software released into the public domain.

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
    class IPv4PacketConstants
    {
        //
        //IPv4 packet header - 20 bytes
        //

        //Length

        public const int IPv4PacketHeaderLength = 20;

        //Protocol

        public const System.Byte IPv4PacketHeaderProtocolICMP = 0x01; //ICMP - Internet Control Message Protocol (RFC 792)
        public const System.Byte IPv4PacketHeaderProtocolIGMP = 0x02; //IGMP - Internet Group Management Protocol (RFC 1112)
        public const System.Byte IPv4PacketHeaderProtocolTCP = 0x06; //TCP - Transmission Control Protocol (RFC 793)
        public const System.Byte IPv4PacketHeaderProtocolUDP = 0x11; //UDP - User Datagram Protocol (RFC 768)
    }
}