//$Header: https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Ethernet%20Frame/Constants.cs 212 2014-04-17 18:01:00Z michaelmayes $

//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//SVN revision information:
//File:    : $URL$
//Revision : $Revision$
//Date     : $Date$
//Author   : $Author$

namespace EthernetFrame
{
    class Constants
    {
        //
        //Ethernet frame header - 14 bytes
        //

        //Length

        public const ushort HeaderLength = 14;

        //Ether Type - provided in little endian representation

        public enum HeaderEtherType : ushort
        {
            MinimumValue = 0x0600, //Minimum value for Ether Type - lower values indicate length for an IEEE 802.3 Ethernet frame
            ARP = 0x0806, //Ethernet frame containing an ARP packet
            IPv4 = 0x0800, //Ethernet frame containing an IPv4 packet
            IPv6 = 0x86DD, //Ethernet frame containing an IPv6 packet
            LLDP = 0x88CC, //Ethernet frame containing an LLDP packet (IEEE 802.1AB)
            Loopback = 0x9000, //Configuration Test Protocol (Loopback)
            VLANTagged = 0x8100 //Ethernet frame with a VLAN tag (IEEE 802.1Q)
        }
    }
}