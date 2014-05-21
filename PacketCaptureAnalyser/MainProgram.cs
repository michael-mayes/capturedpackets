// $Id$
// $URL$
// <copyright file="MainProgram.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser
{
    /// <summary>
    /// This class provides the main entry point for the application
    /// </summary>
    public static class MainProgram
    {
        /// <summary>
        /// The main entry point for the application
        /// </summary>
        [System.STAThread]
        private static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new MainWindowForm());
        }
    }
}
