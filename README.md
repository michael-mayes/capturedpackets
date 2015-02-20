# capturedpackets
Analysis in C# of packets captured from network using libpcap/WinPcap

It is free and unencumbered software released into the public domain as detailed in the UNLICENSE file in the top level directory of the distribution

Developed under Microsoft Visual Studio 2010/12/13 and .Net Framework 4.0/4.5 for Windows XP and later, but also successfully demonstrated under Mono for Linux

No warnings from StyleCop. Large number of warnings from FxCop addressed, but an equally large number remain

The application can fully process a wide selection of PCAP NG, PCAP and NA Sniffer packet captures and can fully unpack a range of frames, packets and datagrams therein, but the unpacked data is not utilised in the configured version of the code

The set of structures and constants for different types of Ethernet frames and IP packets are no longer required by this application, but have been preserved in the new [packetstructures](https://github.com/michael-mayes/packetstructures) repository

Without additional knowledge of the structure of the messages, the application cannot handle multiple messages within a TCP packet

The code can perform latency analysis for packet round trips across a network, time and burst analysis for time messages on a network, and burst analysis for message on a network, but you must add in RegisterMessageReceipt and RegisterTimeMessageReceipt calls, respectively, into specific message handling to utilise this functionality.

Histograms can be output for the latency, time and burst analysis results

# Features

* Parses PCAP Next Generation packet captures (version 1.0)
* Parses PCAP packet captures (version 2.4)
* Parses NA Sniffer packet captures (version 4.0)
* Can perform latency analysis for packet roundtrips across a network
* Can perform analysis of consistency of time messages across a network
* Can perform analysis of bursting of messages across a network
* Outputs histograms for latency, time and burst analysis results
* Extracts IEEE 802.3 Ethernet frames
* Extracts Ethernet II frames
* Extracts ARP packets
* Extracts TCP packets
* Extracts UDP datagrams
* Extracts ICMPv4 packets
* Extracts IGMPv2 packets
* Extracts EIGRP packets
* Extracts Configuration Test Protocol (Loopback) packets
* Extracts LLDP packets
* Handles spuriously exposed trailer bytes in packet capture
