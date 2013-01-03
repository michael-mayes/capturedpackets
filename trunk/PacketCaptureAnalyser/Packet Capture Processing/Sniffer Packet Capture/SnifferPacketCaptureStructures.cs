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
    class SnifferPackageCaptureStructures
    {
        //
        //Sniffer packet capture global header - 41 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = SnifferPackageCaptureConstants.SnifferPackageCaptureGlobalHeaderLength)]

        public struct SnifferPackageCaptureGlobalHeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt64 MagicNumberHigh; //High bytes of the magic number (eight bytes)

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.UInt64 MagicNumberLow; //Low bytes of the magic number (eight bytes)

            [System.Runtime.InteropServices.FieldOffset(16)]
            public System.Byte MagicNumberTerminator; //Terminating byte for the magic number

            [System.Runtime.InteropServices.FieldOffset(17)]
            public System.Int16 RecordType; //Type of version record

            [System.Runtime.InteropServices.FieldOffset(19)]
            public System.Int32 RecordLength; //Length of version record - only the first two bytes are length, the latter two are "reserved" and so not processed

            [System.Runtime.InteropServices.FieldOffset(23)]
            public System.Int16 VersionMajor; //Major version number

            [System.Runtime.InteropServices.FieldOffset(25)]
            public System.Int16 VersionMinor; //Minor version number

            [System.Runtime.InteropServices.FieldOffset(27)]
            public System.Int16 Time; //DOS-format time

            [System.Runtime.InteropServices.FieldOffset(29)]
            public System.Int16 Date; //DOS-format date

            [System.Runtime.InteropServices.FieldOffset(31)]
            public System.SByte Type; //Type of records that follow the Sniffer packet capture global header in the packet capture

            [System.Runtime.InteropServices.FieldOffset(32)]
            public System.Byte NetworkEncapsulationType; //Type of network encapsulation for records that follow the Sniffer packet capture global header in the packet capture

            [System.Runtime.InteropServices.FieldOffset(33)]
            public System.SByte FormatVersion; //Format version

            [System.Runtime.InteropServices.FieldOffset(34)]
            public System.Byte TimestampUnits; //Timestamp units

            [System.Runtime.InteropServices.FieldOffset(35)]
            public System.SByte CompressionVersion; //Compression version

            [System.Runtime.InteropServices.FieldOffset(36)]
            public System.SByte CompressionLevel; //Compression level

            [System.Runtime.InteropServices.FieldOffset(37)]
            public System.Int32 Reserved; //Reserved
        }

        //
        //Sniffer packet capture record header - 6 bytes
        //
        
        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = SnifferPackageCaptureConstants.SnifferPackageCaptureRecordHeaderLength)]

        public struct SnifferPackageCaptureRecordHeaderStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt16 RecordType;

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt32 RecordLength;
        }

        //
        //Sniffer packet capture Sniffer type 2 data record - 14 bytes
        //

        [System.Runtime.InteropServices.StructLayout
            (System.Runtime.InteropServices.LayoutKind.Explicit,
            Size = SnifferPackageCaptureConstants.SnifferPackageCaptureSnifferType2RecordLength)]

        public struct SnifferPackageCaptureSnifferType2RecordStructure
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public System.UInt16 TimestampLow; //Low bytes of timestamp

            [System.Runtime.InteropServices.FieldOffset(2)]
            public System.UInt16 TimestampMiddle; //Middle bytes of timestamp

            [System.Runtime.InteropServices.FieldOffset(4)]
            public System.Byte TimestampHigh; //High bytes of the timestamp

            [System.Runtime.InteropServices.FieldOffset(5)]
            public System.Byte TimeDays; //Time in days since start of capture

            [System.Runtime.InteropServices.FieldOffset(6)]
            public System.Int16 Size; //Number of bytes of data

            [System.Runtime.InteropServices.FieldOffset(8)]
            public System.Byte FrameErrorStatusBits; //Frame error status bits

            [System.Runtime.InteropServices.FieldOffset(9)]
            public System.Byte Flags; //Buffer flags

            [System.Runtime.InteropServices.FieldOffset(10)]
            public System.Int16 TrueSize; //Size of original frame in bytes

            [System.Runtime.InteropServices.FieldOffset(12)]
            public System.Int16 Reserved; //Reserved
        }
    }
}