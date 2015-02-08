// $Id$
// $URL$
// <copyright file="Constants.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.EthernetFrame.IPPacket.TCPPacket
{
    /// <summary>
    /// This class provides constants for use by the TCP packet processing
    /// </summary>
    public static class Constants
    {
        //// TCP packet header

        /// <summary>
        /// TCP packet header minimum length
        /// </summary>
        public const ushort HeaderMinimumLength = 20;

        /// <summary>
        /// TCP packet header maximum length
        /// </summary>
        public const ushort HeaderMaximumLength = 60;

        /// <summary>
        /// TCP packet header port number
        /// </summary>
        public enum PortNumber : ushort
        {
            /// <summary>
            /// Minimum value
            /// </summary>
            DummyValueMin = 0,

            /// <summary>
            /// Maximum value
            /// </summary>
            DummyValueMax = 65535
        }

        /// <summary>
        /// TCP packet header flags
        /// </summary>
        public enum Flags : byte
        {
            /// <summary>
            /// Congestion Window Reduced (CWR) flag
            /// </summary>
            CWR = 0,

            /// <summary>
            /// ECN-Echo flag
            /// </summary>
            ECE,

            /// <summary>
            /// Urgent flag
            /// </summary>
            URG,

            /// <summary>
            /// Acknowledgment flag
            /// </summary>
            ACK,

            /// <summary>
            /// Push flag
            /// </summary>
            PSH,

            /// <summary>
            /// Reset flag
            /// </summary>
            RST,

            /// <summary>
            /// Synchronize sequence numbers flag
            /// </summary>
            SYN,

            /// <summary>
            /// Finish the connection flag
            /// </summary>
            FIN
        }

        /// <summary>
        /// Examines the supplied byte to determine whether the supplied flag is set
        /// </summary>
        /// <param name="theByte">The byte to be examined</param>
        /// <param name="theFlag">The flag to be examined within the byte</param>
        /// <returns>Boolean flag which indicates whether the supplied flag is set in the supplied byte</returns>
        public static bool IsFlagSet(byte theByte, Flags theFlag)
        {
            int theShift = Flags.FIN - theFlag;

            // Get a single bit in the proper position
            byte theBitMask = (byte)(1 << theShift);

            // Mask out the appropriate bit
            byte theMaskedByte = (byte)(theByte & theBitMask);

            // If masked != 0, then the masked out bit is 1
            // Otherwise, masked will be 0
            return theMaskedByte != 0;
        }
    }
}