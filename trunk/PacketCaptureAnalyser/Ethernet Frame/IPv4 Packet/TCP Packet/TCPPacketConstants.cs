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
    class TCPPacketConstants
    {
        //
        //TCP packet header - 20 bytes
        //

        //Length

        public const int TCPPacketHeaderMinimumLength = 20;
        public const int TCPPacketHeaderMaximumLength = 60;

        //Port number

        public enum TCPPacketPortNumberEnumeration
        {
          DummyValueMin = 0,
          DummyValueMax = 65535
        }

        //Flags

        public enum TCPPacketFlags
        {
            CWR = 0,
            ECE,
            URG,
            ACK,
            PSH,
            RST,
            SYN,
            FIN
        }

        public bool IsTCPPacketFlagSet(System.Byte TheByte, TCPPacketFlags TheTCPPacketFlag)
        {
            if (TheTCPPacketFlag < TCPPacketFlags.CWR ||
                TheTCPPacketFlag > TCPPacketFlags.FIN)
            {
                throw new System.ArgumentOutOfRangeException();
            }

            int Shift = TCPPacketFlags.FIN - TheTCPPacketFlag;

            // Get a single bit in the proper position
            byte BitMask = (byte)(1 << Shift);

            // Mask out the appropriate bit
            byte MaskedByte = (byte)(TheByte & BitMask);

            // If masked != 0, then the masked out bit is 1
            // Otherwise, masked will be 0
            return (MaskedByte != 0);
        }
    }
}