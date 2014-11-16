// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyser.Analysis.BurstAnalysis
{
    using System.Data; // Required to be able to use AsEnumerable method
    using System.Linq; // Required to be able to use Any method

    /// <summary>
    /// This class provides the burst analysis processing
    /// This class will implement the Disposable class so as to be able to clean up after the data tables it creates which themselves implement the Disposable class
    /// </summary>
    public class Processing : System.IDisposable
    {
        /// <summary>
        /// The object that provides for the logging of debug information
        /// </summary>
        private Analysis.DebugInformation theDebugInformation;

        /// <summary>
        /// Boolean flag that indicates whether to output the histogram
        /// </summary>
        private bool outputHistogram;

        /// <summary>
        /// Boolean flag that indicates whether to output additional information
        /// </summary>
        private bool outputAdditionalInformation;

        /// <summary>
        /// The path of the selected packet capture
        /// </summary>
        private string theSelectedPacketCaptureFile;

        /// <summary>
        /// The data table to hold the timestamp values for messages
        /// </summary>
        private System.Data.DataTable theTimestampValuesTable;

        /// <summary>
        /// The data table to hold the set of host Ids encountered during the burst analysis
        /// </summary>
        private System.Data.DataTable theHostIdsTable;

        /// <summary>
        /// The data table to hold the set of message Ids encountered during the burst analysis
        /// </summary>
        private System.Data.DataTable theMessageIdsTable;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="outputHistogram">Boolean flag that indicates whether to output the histogram</param>
        /// <param name="outputAdditionalInformation">Boolean flag that indicates whether to output additional information</param>
        /// <param name="theSelectedPacketCaptureFile">The path of the selected packet capture</param>
        public Processing(Analysis.DebugInformation theDebugInformation, bool outputHistogram, bool outputAdditionalInformation, string theSelectedPacketCaptureFile)
        {
            this.theDebugInformation = theDebugInformation;

            this.outputHistogram = outputHistogram;

            this.outputAdditionalInformation = outputAdditionalInformation;

            this.theSelectedPacketCaptureFile = theSelectedPacketCaptureFile;

            // Create a datatable to hold the timestamp values for messages
            this.theTimestampValuesTable = new System.Data.DataTable();

            // Create a datatable to hold the set of host Ids encountered during the burst analysis
            this.theHostIdsTable = new System.Data.DataTable();

            // Create a datatable to hold the set of message Ids encountered during the burst analysis
            this.theMessageIdsTable = new System.Data.DataTable();
        }

        /// <summary>
        /// Creates the data tables necessary to perform the burst analysis processing
        /// </summary>
        public void Create()
        {
            // Add the required columns to the datatable to hold the timestamp values for messages
            this.theTimestampValuesTable.Columns.Add("HostId", typeof(byte));
            this.theTimestampValuesTable.Columns.Add("IsReliable", typeof(bool));
            this.theTimestampValuesTable.Columns.Add("IsOutgoing", typeof(bool));
            this.theTimestampValuesTable.Columns.Add("SequenceNumber", typeof(ulong));
            this.theTimestampValuesTable.Columns.Add("MessageId", typeof(ulong));
            this.theTimestampValuesTable.Columns.Add("PacketNumber", typeof(ulong));
            this.theTimestampValuesTable.Columns.Add("PacketTimestamp", typeof(double));

            // Add the required column to the datatable to hold the set of host Ids encountered during the burst analysis
            this.theHostIdsTable.Columns.Add("HostId", typeof(byte));

            // Set the primary key to be the only column
            // The primary key is needed to allow for use of the Find method against the datatable
            this.theHostIdsTable.PrimaryKey =
                new System.Data.DataColumn[]
                {
                    this.theHostIdsTable.Columns["HostId"]
                };

            // Add the required columns to the datatable to hold the set of message Ids encountered during the burst analysis
            this.theMessageIdsTable.Columns.Add("HostId", typeof(byte));
            this.theMessageIdsTable.Columns.Add("MessageId", typeof(ulong));

            // Set a multi-column primary key
            // The primary key is needed to allow for use of the Find method against the datatable
            this.theMessageIdsTable.PrimaryKey =
                new System.Data.DataColumn[]
                {
                    this.theMessageIdsTable.Columns["HostId"],
                    this.theMessageIdsTable.Columns["MessageId"]
                };
        }

        /// <summary>
        /// Clean up any resources used by the burst analysis class
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Supplies a set of data for a message to register its receipt for the purposes of burst analysis
        /// </summary>
        /// <param name="theHostId">The host Id for the message</param>
        /// <param name="isReliable">Boolean flag that indicates whether the message was received reliably (true indicates reliably, false indicates non-reliably)</param>
        /// <param name="isOutgoing">Boolean flag that indicates whether the message was received on its outgoing journey or its incoming journey (true indicates outgoing, false indicates incoming)</param>
        /// <param name="theSequenceNumber">The sequence number for the message</param>
        /// <param name="theMessageId">The identifier for the type of message</param>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        public void RegisterMessageReceipt(byte theHostId, bool isReliable, bool isOutgoing, ulong theSequenceNumber, ulong theMessageId, ulong thePacketNumber, double thePacketTimestamp)
        {
            // Do not process messages where the message Id is not populated (or is invalid)
            if (theHostId == 0)
            {
                return;
            }

            // Do not process messages where the message Id is not populated (or is invalid)
            if (theMessageId == 0)
            {
                return;
            }

            //// Add the supplied timestamp and time to the datatable

            System.Data.DataRow theTimestampValuesRowToAdd =
                this.theTimestampValuesTable.NewRow();

            theTimestampValuesRowToAdd["HostId"] = theHostId;
            theTimestampValuesRowToAdd["IsReliable"] = isReliable;
            theTimestampValuesRowToAdd["IsOutgoing"] = isOutgoing;
            theTimestampValuesRowToAdd["SequenceNumber"] = theSequenceNumber;
            theTimestampValuesRowToAdd["MessageId"] = theMessageId;
            theTimestampValuesRowToAdd["PacketNumber"] = thePacketNumber;
            theTimestampValuesRowToAdd["PacketTimestamp"] = thePacketTimestamp;

            this.theTimestampValuesTable.Rows.Add(
                theTimestampValuesRowToAdd);

            // Add the supplied Host Id to the set of those encountered during the burst analysis if not already in there
            this.RegisterEncounteredHostId(
                theHostId);

            // Add the supplied message Id to the set of those encountered during the burst analysis if not already in there
            this.RegisterEncounteredMessageId(
                theHostId,
                theMessageId);
        }

        /// <summary>
        /// Finalize the time analysis
        /// </summary>
        public void Finalise()
        {
            // Obtain the set of Host Ids encountered during the burst analysis in ascending order
            EnumerableRowCollection<System.Data.DataRow>
                theHostIdRowsFound =
                from r in this.theHostIdsTable.AsEnumerable()
                orderby r.Field<byte>("HostId") ascending
                select r;

            //// Loop across all the timestamp values for each of these Host Ids in turn

            this.theDebugInformation.WriteBlankLine();
            this.theDebugInformation.WriteTextLine("====================");
            this.theDebugInformation.WriteTextLine("== Burst Analysis ==");
            this.theDebugInformation.WriteTextLine("====================");
            this.theDebugInformation.WriteBlankLine();

            foreach (System.Data.DataRow theHostIdRow in theHostIdRowsFound)
            {
                this.theDebugInformation.WriteTextLine(
                    "Host Id " +
                    string.Format("{0,3}", theHostIdRow.Field<byte>("HostId").ToString()));

                this.theDebugInformation.WriteTextLine("===========");
                this.theDebugInformation.WriteBlankLine();

                this.FinaliseForHostId(theHostIdRow.Field<byte>("HostId"));
            }
        }

        /// <summary>
        /// Clean up any resources used by the burst analysis class
        /// </summary>
        /// <param name="disposing">Boolean flag that indicates whether the method call comes from a Dispose method (its value is true) or from the garbage collector (its value is false)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose any resources allocated to the datatables if instructed
                this.theTimestampValuesTable.Dispose();
                this.theHostIdsTable.Dispose();
                this.theMessageIdsTable.Dispose();
            }
        }

        /// <summary>
        /// Adds the supplied host Id to the set of those encountered during the burst analysis if not already in there
        /// </summary>
        /// <param name="theHostId">The host Id encountered</param>
        private void RegisterEncounteredHostId(byte theHostId)
        {
            object[] theHostIdRowFindObject = new object[1];

            theHostIdRowFindObject[0] = theHostId.ToString(); // Primary key

            System.Data.DataRow theHostIdDataRowFound =
                this.theHostIdsTable.Rows.Find(
                theHostIdRowFindObject);

            if (theHostIdDataRowFound == null)
            {
                this.theDebugInformation.WriteInformationEvent(
                    "Found a data-supplying message for a Host Id " +
                    string.Format("{0,3}", theHostId) +
                    " - adding this Host Id to the burst analysis");

                System.Data.DataRow theHostIdRowToAdd =
                    this.theHostIdsTable.NewRow();

                theHostIdRowToAdd["HostId"] = theHostId;

                this.theHostIdsTable.Rows.Add(
                    theHostIdRowToAdd);
            }
        }

        /// <summary>
        /// Adds the supplied message Id to the set of those encountered during the burst analysis if not already in there
        /// </summary>
        /// <param name="theHostId">The host Id for the message encountered</param>
        /// <param name="theMessageId">The identifier for the type of message encountered</param>
        private void RegisterEncounteredMessageId(byte theHostId, ulong theMessageId)
        {
            object[] theMessageIdRowFindObject = new object[2];

            theMessageIdRowFindObject[0] = theHostId.ToString(); // Primary key (part one)
            theMessageIdRowFindObject[1] = theMessageId.ToString(); // Primary key (part two)

            System.Data.DataRow theMessageIdDataRowFound =
                this.theMessageIdsTable.Rows.Find(
                theMessageIdRowFindObject);

            if (theMessageIdDataRowFound == null)
            {
                this.theDebugInformation.WriteInformationEvent(
                    "Found a data-supplying message with a Message Id " +
                    string.Format("{0,5}", theMessageId) +
                    " for a Host Id " +
                    string.Format("{0,3}", theHostId) +
                    " - adding this Message Id/Host Id combination to the burst analysis");

                System.Data.DataRow theMessageIdRowToAdd =
                    this.theMessageIdsTable.NewRow();

                theMessageIdRowToAdd["HostId"] = theHostId;
                theMessageIdRowToAdd["MessageId"] = theMessageId;

                this.theMessageIdsTable.Rows.Add(
                    theMessageIdRowToAdd);
            }
        }

        /// <summary>
        /// Finalize burst analysis for the supplied host Id
        /// </summary>
        /// <param name="theHostId">The host Id for which burst analysis is to be finalized</param>
        private void FinaliseForHostId(byte theHostId)
        {
            //// Obtain the set of message Ids encountered for this host Id during the burst analysis in ascending order

            EnumerableRowCollection<System.Data.DataRow>
                theMessageIdRowsFound =
                from r in this.theMessageIdsTable.AsEnumerable()
                where r.Field<byte>("HostId") == theHostId
                orderby r.Field<ulong>("MessageId") ascending
                select r;

            // Finalize the burst analysis separately for reliable and non-reliable messages
            foreach (bool isReliable in new bool[] { true, false })
            {
                bool theFirstReliableRowProcessed = false;

                //// Loop across all the timestamp values using each of the message Ids in turn

                foreach (System.Data.DataRow theMessageIdRow in theMessageIdRowsFound)
                {
                    ulong theMessageId = theMessageIdRow.Field<ulong>("MessageId");

                    // Finalize the burst analysis separately for outgoing and incoming messages
                    foreach (bool isOutgoing in new bool[] { true, false })
                    {
                        EnumerableRowCollection<System.Data.DataRow>
                            theTimestampValueRowsFound =
                            from s in this.theTimestampValuesTable.AsEnumerable()
                            where s.Field<byte>("HostId") == theHostId &&
                            s.Field<ulong>("MessageId") == theMessageId &&
                            s.Field<bool>("IsReliable") == isReliable &&
                            s.Field<bool>("IsOutgoing") == isOutgoing
                            select s;

                        if (theTimestampValueRowsFound.Any())
                        {
                            if (!theFirstReliableRowProcessed)
                            {
                                theFirstReliableRowProcessed = true;

                                if (isReliable)
                                {
                                    this.theDebugInformation.WriteTextLine("Reliable Messages");
                                    this.theDebugInformation.WriteTextLine("-----------------");
                                }
                                else
                                {
                                    this.theDebugInformation.WriteTextLine("Non-Reliable Messages");
                                    this.theDebugInformation.WriteTextLine("---------------------");
                                }

                                this.theDebugInformation.WriteBlankLine();
                            }

                            this.FinaliseForMessageId(
                                theHostId,
                                isOutgoing,
                                theMessageId,
                                theTimestampValueRowsFound);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finalize burst analysis for the supplied type of message
        /// </summary>
        /// <param name="theHostId">The host Id for which burst analysis is to be finalized</param>
        /// <param name="isOutgoing">Boolean flag that indicates whether the messages to be finalized were received on its outgoing journey or its incoming journey (true indicates outgoing, false indicates incoming)</param>
        /// <param name="theMessageId">The identifier for the type of message for which burst analysis is to be finalized</param>
        /// <param name="theRows">The data table rows on which burst analysis is to be finalized</param>
        private void FinaliseForMessageId(byte theHostId, bool isOutgoing, ulong theMessageId, EnumerableRowCollection<System.Data.DataRow> theRows)
        {
            CommonHistogram theTimestampHistogram =
                new CommonHistogram(
                    this.theDebugInformation,
                    Constants.NumberOfBins,
                    Constants.MinTimestampDifference,
                    Constants.MaxTimestampDifference);

            ulong theNumberOfMessages = 0;

            double theMinTimestampDifference = double.MaxValue;
            double theMaxTimestampDifference = double.MinValue;

            ulong theMinTimestampPacketNumber = 0;
            ulong theMaxTimestampPacketNumber = 0;

            ulong theMinTimestampSequenceNumber = 0;
            ulong theMaxTimestampSequenceNumber = 0;

            double theLastTimestamp = 0.0;

            ulong theNumberOfTimestampDifferenceInstances = 0;
            double theTotalOfTimestampDifferences = 0;
            double theAverageTimestampDifference = 0;

            bool theFirstRowProcessed = false;

            System.Collections.Generic.List<string> theOutOfRangeTimestamps =
                new System.Collections.Generic.List<string>();

            foreach (System.Data.DataRow theTimestampValuesRow in theRows)
            {
                // Keep a running total of the number of messages
                ++theNumberOfMessages;

                ulong theSequenceNumber = theTimestampValuesRow.Field<ulong>("SequenceNumber");
                ulong thePacketNumber = theTimestampValuesRow.Field<ulong>("PacketNumber");
                double thePacketTimestamp = theTimestampValuesRow.Field<double>("PacketTimestamp");

                // Do not calculate the difference in timestamp for first row - just record value and move on to second row
                if (!theFirstRowProcessed)
                {
                    theLastTimestamp = thePacketTimestamp;

                    theFirstRowProcessed = true;

                    continue;
                }

                //// The timestamp

                double theTimestampDifference =
                    (thePacketTimestamp - theLastTimestamp) * 1000.0; // Milliseconds

                //// Keep a running total of the timestamp differences to allow for averaging

                ++theNumberOfTimestampDifferenceInstances;

                theTotalOfTimestampDifferences += theTimestampDifference;

                if (!theTimestampHistogram.AddValue(theTimestampDifference))
                {
                    theOutOfRangeTimestamps.Add(
                        "The message for packet number " +
                        string.Format("{0,7}", thePacketNumber.ToString()) +
                        " and sequence number " +
                        string.Format("{0,7}", theSequenceNumber.ToString()) +
                        " has an out of range timestamp difference of " +
                        string.Format("{0,18}", theTimestampDifference.ToString()) +
                        " ms");
                }

                if (theMinTimestampDifference > theTimestampDifference)
                {
                    theMinTimestampDifference = theTimestampDifference;
                    theMinTimestampPacketNumber = thePacketNumber;
                    theMinTimestampSequenceNumber = theSequenceNumber;
                }

                if (theMaxTimestampDifference < theTimestampDifference)
                {
                    theMaxTimestampDifference = theTimestampDifference;
                    theMaxTimestampPacketNumber = thePacketNumber;
                    theMaxTimestampSequenceNumber = theSequenceNumber;
                }

                theLastTimestamp = thePacketTimestamp;
            }

            if (isOutgoing)
            {
                this.theDebugInformation.WriteTextLine(
                    "The number of outgoing messages with a Message Id of " +
                    string.Format("{0,5}", theMessageId) +
                    " was " +
                    theNumberOfMessages.ToString());
            }
            else
            {
                this.theDebugInformation.WriteTextLine(
                    "The number of incoming messages with a Message Id of " +
                    string.Format("{0,5}", theMessageId) +
                    " was " +
                    theNumberOfMessages.ToString());
            }

            if (theNumberOfTimestampDifferenceInstances > 0)
            {
                theAverageTimestampDifference =
                    theTotalOfTimestampDifferences /
                    theNumberOfTimestampDifferenceInstances;

                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "The minimum timestamp difference was " +
                    string.Format("{0,18}", theMinTimestampDifference.ToString()) +
                    " ms for packet number " +
                    string.Format("{0,7}", theMinTimestampPacketNumber.ToString()) +
                    " and sequence number " +
                    string.Format("{0,7}", theMinTimestampSequenceNumber.ToString()));

                this.theDebugInformation.WriteTextLine(
                    "The maximum timestamp difference was " +
                    string.Format("{0,18}", theMaxTimestampDifference.ToString()) +
                    " ms for packet number " +
                    string.Format("{0,7}", theMaxTimestampPacketNumber.ToString()) +
                    " and sequence number " +
                    string.Format("{0,7}", theMaxTimestampSequenceNumber.ToString()));

                this.theDebugInformation.WriteTextLine(
                    "The average timestamp difference was " +
                    string.Format("{0,18}", theAverageTimestampDifference.ToString()) +
                    " ms");

                if (theAverageTimestampDifference > 0)
                {
                    this.theDebugInformation.WriteBlankLine();

                    double theMessageRate =
                        1.0 /
                        (theAverageTimestampDifference / 1000.0); // Hertz

                    this.theDebugInformation.WriteTextLine(
                        "The average message rate was " +
                        string.Format("{0,18}", theMessageRate.ToString()) +
                        " Hz");
                }

                if (this.outputHistogram)
                {
                    //// Output the histogram

                    this.theDebugInformation.WriteBlankLine();

                    this.theDebugInformation.WriteTextLine(
                        "The histogram (" +
                        Constants.BinsPerMillisecond.ToString() +
                        " bins per millisecond) for the timestamp differences is:");

                    this.theDebugInformation.WriteBlankLine();

                    theTimestampHistogram.OutputValues();
                }

                if (theOutOfRangeTimestamps.Any())
                {
                    this.theDebugInformation.WriteBlankLine();

                    // Output the data for any message with an out of range timestamp difference
                    foreach (string theString in theOutOfRangeTimestamps)
                    {
                        this.theDebugInformation.WriteTextLine(theString);
                    }
                }
            }

            this.theDebugInformation.WriteBlankLine();

            this.theDebugInformation.WriteTextLine(
                new string('-', 144));

            this.theDebugInformation.WriteBlankLine();

            if (this.outputAdditionalInformation &&
                theNumberOfTimestampDifferenceInstances > 0)
            {
                System.Text.StringBuilder theOutputAdditionalInformationLines =
                    new System.Text.StringBuilder();

                //// Add a column titles line to the debug output file

                string theOutputAdditionalInformationTitleLine = string.Format(
                    "{0},{1},{2},,{3}{4}",
                    "Packet Number",
                    "Sequence Number",
                    "Packet Timestamp (s)",
                    "Packet Timestamp Delta (ms)",
                    System.Environment.NewLine);

                theOutputAdditionalInformationLines.Append(
                    theOutputAdditionalInformationTitleLine);

                double? thePreviousPacketTimestamp = null;

                //// Add a line to the debug output file for each of the processed messages

                foreach (System.Data.DataRow theTimestampValuesRow in theRows)
                {
                    ulong theSequenceNumber = theTimestampValuesRow.Field<ulong>("SequenceNumber");
                    ulong thePacketNumber = theTimestampValuesRow.Field<ulong>("PacketNumber");
                    double thePacketTimestamp = theTimestampValuesRow.Field<double>("PacketTimestamp");

                    string theOutputAdditionalInformationLine = null;

                    double theCurrentPacketTimestamp = thePacketTimestamp;

                    if (thePreviousPacketTimestamp == null)
                    {
                        // If this is the first message then there is no previous message and so just output the timestamp
                        theOutputAdditionalInformationLine = string.Format(
                            "{0},{1},{2,18}{3}",
                            thePacketNumber.ToString(),
                            theSequenceNumber.ToString(),
                            thePacketTimestamp.ToString(),
                            System.Environment.NewLine);
                    }
                    else
                    {
                        double theTimestampDifference =
                            (thePacketTimestamp - (double)thePreviousPacketTimestamp) * 1000.0; // Milliseconds

                        // If this is another message then also calculate the difference in timestamp from the previous message and output it along with the timestamp
                        theOutputAdditionalInformationLine = string.Format(
                            "{0},{1},{2,18},,{3,18}{4}",
                            thePacketNumber.ToString(),
                            theSequenceNumber.ToString(),
                            thePacketTimestamp.ToString(),
                            theTimestampDifference,
                            System.Environment.NewLine);
                    }

                    thePreviousPacketTimestamp = thePacketTimestamp;

                    theOutputAdditionalInformationLines.Append(
                        theOutputAdditionalInformationLine);
                }

                if (isOutgoing)
                {
                    System.IO.File.WriteAllText(
                        this.theSelectedPacketCaptureFile +
                        ".HostId" +
                        theHostId +
                        ".MessageId" +
                        theMessageId +
                        ".BurstAnalysis.csv",
                        theOutputAdditionalInformationLines.ToString());
                }
                else
                {
                    System.IO.File.WriteAllText(
                        this.theSelectedPacketCaptureFile +
                        ".HostId" +
                        theHostId +
                        ".MessageId" +
                        theMessageId +
                        ".BurstAnalysis.csv",
                        theOutputAdditionalInformationLines.ToString());
                }
            }
        }
    }
}
