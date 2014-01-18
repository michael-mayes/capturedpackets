﻿//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureProcessing
{
    class SnifferPackageCaptureConstants
    {
        //
        //Sniffer packet capture global header - 41 bytes
        //

        //Length

        public const ushort SnifferPackageCaptureGlobalHeaderLength = 41;

        //Magic numbers - provided in big endian (network) representation

        //Highest four bytes containing ASCII characters "TRSN" to start the magic number (used for indentifying the packet capture type)
        public const System.UInt32 SnifferPackageCaptureExpectedMagicNumberHighest = 0x4E535254;

        //High eight bytes containing ASCII characters "TRSNIFF " to start the magic number
        public const System.UInt64 SnifferPackageCaptureExpectedMagicNumberHigh = 0x204646494E535254;

        //Low eight bytes containing ASCII characters "data    " to continue the magic number
        public const System.UInt64 SnifferPackageCaptureExpectedMagicNumberLow = 0x2020202061746164;

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

        public enum SnifferPackageCaptureRecordHeaderSnifferRecordType : ushort
        {
            VersionRecordType = 1, //Version record type
            Type2RecordType = 4, //Type 2 data record type
            EndOfFileRecordType = 3 //End of file record type
        }

        //
        //Sniffer packet capture Sniffer type 2 data record - 14 bytes
        //

        public const ushort SnifferPackageCaptureSnifferType2RecordLength = 14;
    }
}