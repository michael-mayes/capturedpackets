//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureProcessingNamespace
{
    class SnifferPackageCaptureConstants
    {
        //
        //Sniffer packet capture global header - 41 bytes
        //

        //Length

        public const ushort SnifferPackageCaptureGlobalHeaderLength = 41;

        //Magic number - provided in little endian representation

        public const System.UInt64 SnifferPackageCaptureExpectedMagicNumberHigh = 0x5452534E49464620; //High bytes containing ASCII characters "TRSNIFF " start the magic number
        public const System.UInt64 SnifferPackageCaptureExpectedMagicNumberLow = 0x6461746120202020;  //Low bytes containing ASCII characters "data    " continue the magic number

        public const System.Byte SnifferPackageCaptureExpectedMagicNumberTerminator = 0x1A; //Terminating byte ASCII control character 'SUB' completes the magic number

        //Version numbers

        public const System.Int16 SnifferPackageCaptureExpectedVersionMajor = 4;
        public const System.Int16 SnifferPackageCaptureExpectedVersionMinor = 0;

        //Type of records that follow the Sniffer packet capture global header in the packet capture

        public const System.SByte SnifferPackageCaptureExpectedType = 4; //Sniffer type 2 data records

        //Format version number

        public const System.SByte SnifferPackageCaptureExpectedFormatVersion = 1; //Uncompressed

        //
        //Sniffer packet capture record header - 6 bytes
        //

        //Length

        public const ushort SnifferPackageCaptureRecordHeaderLength = 6;

        //Record type

        public const System.UInt16 SnifferPackageCaptureRecordHeaderSnifferVersionRecordType = 1; //Sniffer version record type
        public const System.UInt16 SnifferPackageCaptureRecordHeaderSnifferType2RecordType = 4; //Sniffer type 2 data record type
        public const System.UInt16 SnifferPackageCaptureRecordHeaderSnifferEndOfFileRecordType = 3; //Sniffer end of file record type

        //
        //Sniffer packet capture Sniffer type 2 data record - 14 bytes
        //

        public const ushort SnifferPackageCaptureSnifferType2RecordLength = 14;
    }
}