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

namespace PacketCaptureProcessingNamespace
{
    class PCAPPackageCaptureConstants
    {
        //
        //PCAP packet capture global header - 24 bytes
        //

        //Length

        public const ushort PCAPPackageCaptureGlobalHeaderLength = 24;

        //Magic number

        public const System.UInt32 PCAPPackageCaptureLittleEndianMagicNumber = 0xa1b2c3d4;
        public const System.UInt32 PCAPPackageCaptureBigEndianMagicNumber = 0xd4c3b2a1;

        //Version numbers

        public const System.UInt16 PCAPPackageCaptureExpectedVersionMajor = 2;
        public const System.UInt16 PCAPPackageCaptureExpectedVersionMinor = 4;

        //Network data link type

        public const System.UInt32 PCAPPackageCaptureExpectedNetworkDataLinkType = 1; //Ethernet

        //
        //PCAP packet capture packet header - 16 bytes
        //

        //Length

        public const ushort PCAPPackageCapturePacketHeaderLength = 16;
    }
}