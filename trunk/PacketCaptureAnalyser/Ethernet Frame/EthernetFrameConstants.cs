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

namespace EthernetFrameNamespace
{
    class EthernetFrameConstants
    {
        //
        //Ethernet frame header - 14 bytes
        //

        //Length

        public const int EthernetFrameHeaderLength = 14;

        //Ether Type - provided in little endian representation

        public enum EthernetFrameHeaderEtherTypeEnumeration
        {
            MinimumValue = 0x0600, //Minimum value for Ether Type - lower values indicate length of the Ethernet frame
            ARP = 0x0806, //Ethernet frame containing an ARP packet
            IPv4 = 0x0800, //Ethernet frame containing an IPv4 packet
            IPv6 = 0x86DD, //Ethernet frame containing an IPv6 packet
            LLDP = 0x88CC, //Ethernet frame containing an LLDP packet (IEEE 802.1AB)
            VLANTagged = 0x8100 //Ethernet frame with a VLAN tag (IEEE 802.1Q)
        }
    }
}