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

namespace RecordingProcessingNamespace
{
    class ENCRecordingConstants
    {
        //
        //ENC recording global header - 41 bytes
        //

        //Length

        public const int ENCRecordingGlobalHeaderLength = 41;

        //Magic number - provided in little endian representation

        public const System.UInt64 ENCRecordingExpectedMagicNumberHigh = 0x5452534E49464620; //High bytes containing ASCII characters "TRSNIFF " start the magic number
        public const System.UInt64 ENCRecordingExpectedMagicNumberLow = 0x6461746120202020;  //Low bytes containing ASCII characters "data    " continue the magic number

        public const System.Byte ENCRecordingExpectedMagicNumberTerminator = 0x1A; //Terminating byte ASCII control character 'SUB' completes the magic number

        //Version numbers

        public const System.Int16 ENCRecordingExpectedVersionMajor = 4;
        public const System.Int16 ENCRecordingExpectedVersionMinor = 0;

        //Type of records that follow the ENC recording global header in the recording

        public const System.SByte ENCRecordingExpectedType = 4;

        //Network encapsulation type

        public const System.UInt32 ENCRecordingExpectedNetworkEncapsulationType = 1; //Ethernet

        //Format version number

        public const System.SByte ENCRecordingExpectedFormatVersion = 1; //Uncompressed

        //
        //ENC recording record header - 6 bytes
        //

        //Length

        public const int ENCRecordingRecordHeaderLength = 6;

        //Record type

        public const System.UInt16 ENCRecordingRecordHeaderSnifferType2RecordType = 4; //Sniffer type 2 data record type
        public const System.UInt16 ENCRecordingRecordHeaderSnifferEndOfFileRecordType = 3; //Sniffer end of file record type
        
        //
        //ENC recording Sniffer type 2 data record - 14 bytes
        //

        public const int ENCRecordingSnifferType2RecordLength = 14;
    }
}