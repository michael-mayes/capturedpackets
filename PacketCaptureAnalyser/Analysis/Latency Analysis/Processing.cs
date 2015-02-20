// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.Analysis.LatencyAnalysis
{
    using System.Data; // Required to be able to use AsEnumerable method
    using System.Linq; // Required to be able to use Any method

    // Create an alias for the enumerable for the dictionary to improve clarity of later code that uses it
    // Cannot nest using declarations so must use the declaration of the key value pair type in full again
    using DictionaryEnumerableType = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Structures.DictionaryKey, Structures.DictionaryValue>>;

    // Create an alias for the key value pair for the dictionary to improve clarity of later code that uses it
    using DictionaryKeyValuePairType = System.Collections.Generic.KeyValuePair<Structures.DictionaryKey, Structures.DictionaryValue>;

    /// <summary>
    /// This class provides the latency analysis processing
    /// This class will implement the Disposable class so as to be able to clean up after the data tables it creates which themselves implement the Disposable class
    /// </summary>
    public class Processing : System.IDisposable
    {
        /// <summary>
        /// The dictionary to hold the latency values for message pairings
        /// </summary>
        private System.Collections.Generic.Dictionary<Structures.DictionaryKey,
            Structures.DictionaryValue> theDictionary;

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
        /// The data table to hold the set of host Ids encountered during the latency analysis
        /// </summary>
        private System.Data.DataTable theHostIdsTable;

        /// <summary>
        /// The data table to hold the set of message Ids encountered during the latency analysis
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

            // Create a dictionary to hold the latency values for message pairings
            this.theDictionary =
                new System.Collections.Generic.Dictionary<Structures.DictionaryKey, Structures.DictionaryValue>();

            // Create a datatable to hold the set of host Ids encountered during the latency analysis
            this.theHostIdsTable = new System.Data.DataTable();
            this.theHostIdsTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

            // Create a datatable to hold the set of message Ids encountered during the latency analysis
            this.theMessageIdsTable = new System.Data.DataTable();
            this.theMessageIdsTable.Locale = System.Globalization.CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Creates the data tables necessary to perform the latency analysis processing
        /// </summary>
        public void Create()
        {
            // Add the required column to the datatable to hold the set of host Ids encountered during the latency analysis
            this.theHostIdsTable.Columns.Add("HostId", typeof(byte));

            // Set the primary key to be the only column
            // The primary key is needed to allow for use of the Find method against the datatable
            this.theHostIdsTable.PrimaryKey =
                new System.Data.DataColumn[]
                {
                    this.theHostIdsTable.Columns["HostId"]
                };

            // Add the required columns to the datatable to hold the set of message Ids encountered during the latency analysis
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
        /// Clean up any resources used by the latency analysis class
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Supplies a set of data for a message to register its receipt for the purposes of latency analysis
        /// </summary>
        /// <param name="theHostId">The host Id for the message</param>
        /// <param name="isReliable">Boolean flag that indicates whether the message was received reliably (true indicates reliably, false indicates non-reliably)</param>
        /// <param name="theSequenceNumber">The sequence number for the message</param>
        /// <param name="theMessageId">The identifier for the type of message</param>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp read from the packet capture</param>
        public void RegisterMessageReceipt(byte theHostId, bool isReliable, ulong theSequenceNumber, ulong theMessageId, ulong thePacketNumber, double thePacketTimestamp)
        {
            // Do not process messages where the sequence number is not populated (or is invalid) as we would not be able match message pairs using them
            if (theSequenceNumber == 0)
            {
                return;
            }

            // Do not process messages where the message Id is not populated (or is invalid) as we would not be able match message pairs using them
            if (theHostId == 0)
            {
                return;
            }

            // Do not process messages where the message Id is not populated (or is invalid) as we would not be able match message pairs using them
            if (theMessageId == 0)
            {
                return;
            }

            //// Add the supplied sequence number and timestamp to latency values dictionary

            Structures.DictionaryKey theDictionaryKey =
                new Structures.DictionaryKey(
                    theHostId,
                    isReliable,
                    theSequenceNumber);

            //// Check whether there is a dictionary entry for this key i.e. is this the first message of the pair

            Structures.DictionaryValue theDictionaryValueFound;

            bool theEntryFound = this.theDictionary.TryGetValue(
                theDictionaryKey,
                out theDictionaryValueFound);

            if (!theEntryFound)
            {
                // If this is the first message of the pairing then create the new entry in the dictionary
                Structures.DictionaryValue
                    theDictionaryValueToAdd =
                    new Structures.DictionaryValue(
                        theMessageId,
                        thePacketNumber,
                        thePacketTimestamp);

                // Add the new entry to the dictionary
                this.theDictionary.Add(
                    theDictionaryKey,
                    theDictionaryValueToAdd);
            }
            else
            {
                //// If this is the second message of the pairing then update the row and calculate the difference in timestamps i.e. the latency

                if (!theDictionaryValueFound.FirstInstanceFound)
                {
                    this.theDebugInformation.WriteErrorEvent(
                        "Found the row for the Host Id " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,3}", theHostId) +
                        " and the sequence number " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theSequenceNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " but the FirstInstanceFound flag is not set!!!");

                    return;
                }

                if (theDictionaryValueFound.SecondInstanceFound)
                {
                    this.theDebugInformation.WriteErrorEvent(
                        "Found the row for the Host Id " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,3}", theHostId) +
                        " and the sequence number " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theSequenceNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " but the SecondInstanceFound flag is already set!!!");

                    return;
                }

                theDictionaryValueFound.SecondInstanceFound = true;
                theDictionaryValueFound.SecondInstancePacketNumber = thePacketNumber;

                if (thePacketTimestamp == theDictionaryValueFound.FirstInstancePacketTimestamp)
                {
                    theDictionaryValueFound.SecondInstancePacketTimestamp = thePacketTimestamp;

                    theDictionaryValueFound.TimestampDifference = 0.0;

                    theDictionaryValueFound.TimestampDifferenceCalculated = true;
                }
                else
                {
                    theDictionaryValueFound.SecondInstancePacketTimestamp = thePacketTimestamp;

                    theDictionaryValueFound.TimestampDifference =
                        (thePacketTimestamp - theDictionaryValueFound.FirstInstancePacketTimestamp) * 1000.0; // Milliseconds

                    theDictionaryValueFound.TimestampDifferenceCalculated = true;
                }

                // Update the values in the dictionary entry
                this.theDictionary[theDictionaryKey] =
                    theDictionaryValueFound;

                // Add the supplied host Id to the set of those encountered during the latency analysis if not already in there
                this.RegisterEncounteredHostId(
                    theHostId);

                // Add the supplied message Id to the set of those encountered during the latency analysis if not already in there
                this.RegisterEncounteredMessageId(
                    theHostId,
                    theMessageId);
            }
        }

        /// <summary>
        /// Finalize the latency analysis
        /// </summary>
        public void Finalise()
        {
            // Obtain the set of host Ids encountered during the latency analysis in ascending order
            EnumerableRowCollection<System.Data.DataRow>
                theHostIdRowsFound =
                from r in this.theHostIdsTable.AsEnumerable()
                orderby r.Field<byte>("HostId") ascending
                select r;

            //// Loop across all the latency values for the message pairings using each of these host Ids in turn

            this.theDebugInformation.WriteBlankLine();
            this.theDebugInformation.WriteTextLine("======================");
            this.theDebugInformation.WriteTextLine("== Latency Analysis ==");
            this.theDebugInformation.WriteTextLine("======================");
            this.theDebugInformation.WriteBlankLine();

            foreach (System.Data.DataRow theHostIdRow in theHostIdRowsFound)
            {
                this.theDebugInformation.WriteTextLine(
                    "Host Id " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,3}", theHostIdRow.Field<byte>("HostId").ToString(System.Globalization.CultureInfo.CurrentCulture)));

                this.theDebugInformation.WriteTextLine("===========");
                this.theDebugInformation.WriteBlankLine();

                this.FinaliseForHostId(theHostIdRow.Field<byte>("HostId"));
            }
        }

        /// <summary>
        /// Clean up any resources used by the latency analysis class
        /// </summary>
        /// <param name="disposing">Boolean flag that indicates whether the method call comes from a Dispose method (its value is true) or from the garbage collector (its value is false)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose any resources allocated to the datatables if instructed
                this.theHostIdsTable.Dispose();
                this.theMessageIdsTable.Dispose();
            }
        }

        /// <summary>
        /// Adds the supplied host Id to the set of those encountered during the latency analysis if not already in there
        /// </summary>
        /// <param name="theHostId">The host Id encountered</param>
        private void RegisterEncounteredHostId(byte theHostId)
        {
            object[] theHostIdRowFindObject = new object[1];

            theHostIdRowFindObject[0] = theHostId.ToString(System.Globalization.CultureInfo.CurrentCulture); // Primary key

            System.Data.DataRow theHostIdDataRowFound =
                this.theHostIdsTable.Rows.Find(
                theHostIdRowFindObject);

            if (theHostIdDataRowFound == null)
            {
                this.theDebugInformation.WriteInformationEvent(
                    "Found a pair of data-supplying messages for a Host Id " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,3}", theHostId) +
                    " - adding this Host Id to the latency analysis");

                System.Data.DataRow theHostIdRowToAdd =
                    this.theHostIdsTable.NewRow();

                theHostIdRowToAdd["HostId"] = theHostId;

                this.theHostIdsTable.Rows.Add(
                    theHostIdRowToAdd);
            }
        }

        /// <summary>
        /// Adds the supplied message Id to the set of those encountered during the latency analysis if not already in there
        /// </summary>
        /// <param name="theHostId">The host Id for the message encountered</param>
        /// <param name="theMessageId">The identifier for the type of message encountered</param>
        private void RegisterEncounteredMessageId(byte theHostId, ulong theMessageId)
        {
            object[] theMessageIdRowFindObject = new object[2];

            theMessageIdRowFindObject[0] = theHostId.ToString(System.Globalization.CultureInfo.CurrentCulture); // Primary key (part one)
            theMessageIdRowFindObject[1] = theMessageId.ToString(System.Globalization.CultureInfo.CurrentCulture); // Primary key (part two)

            System.Data.DataRow theMessageIdDataRowFound =
                this.theMessageIdsTable.Rows.Find(
                theMessageIdRowFindObject);

            if (theMessageIdDataRowFound == null)
            {
                this.theDebugInformation.WriteInformationEvent(
                    "Found a pair of data-supplying messages with a Message Id " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,5}", theMessageId) +
                    " for a Host Id " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,3}", theHostId) +
                    " - adding this Message Id/Host Id combination to the latency analysis");

                System.Data.DataRow theMessageIdRowToAdd =
                    this.theMessageIdsTable.NewRow();

                theMessageIdRowToAdd["HostId"] = theHostId;
                theMessageIdRowToAdd["MessageId"] = theMessageId;

                this.theMessageIdsTable.Rows.Add(
                    theMessageIdRowToAdd);
            }
        }

        /// <summary>
        /// Finalize latency analysis for the supplied host Id
        /// </summary>
        /// <param name="theHostId">The host Id for which latency analysis is to be finalized</param>
        private void FinaliseForHostId(byte theHostId)
        {
            //// Obtain the set of message Ids encountered for this host Id during the latency analysis in ascending order

            EnumerableRowCollection<System.Data.DataRow>
                theMessageIdRowsFound =
                from r in this.theMessageIdsTable.AsEnumerable()
                where r.Field<byte>("HostId") == theHostId
                orderby r.Field<ulong>("MessageId") ascending
                select r;

            // Finalize the latency analysis separately for reliable and non-reliable messages
            foreach (bool isReliable in new bool[] { true, false })
            {
                bool theFirstRowProcessed = false;

                //// Loop across all the latency values for the message pairings using each of these message Ids in turn

                foreach (System.Data.DataRow theMessageIdRow in theMessageIdRowsFound)
                {
                    ulong theMessageId = theMessageIdRow.Field<ulong>("MessageId");

                    DictionaryEnumerableType
                        theLatencyValueEntriesFound =
                        from s in this.theDictionary.AsEnumerable()
                        where s.Key.IsReliable == isReliable &&
                        s.Key.HostId == theHostId &&
                        s.Value.MessageId == theMessageId &&
                        s.Value.TimestampDifferenceCalculated
                        select s;

                    if (theLatencyValueEntriesFound.Any())
                    {
                        if (!theFirstRowProcessed)
                        {
                            theFirstRowProcessed = true;

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
                            theMessageId,
                            theLatencyValueEntriesFound);
                    }
                }
            }
        }

        /// <summary>
        /// Finalize latency analysis for the supplied type of message
        /// </summary>
        /// <param name="theMessageId">The identifier for the type of message for which latency analysis is to be finalized</param>
        /// <param name="theRows">The data table rows on which latency analysis is to be finalized</param>
        private void FinaliseForMessageId(ulong theMessageId, DictionaryEnumerableType theRows)
        {
            CommonHistogram theHistogram =
                new CommonHistogram(
                    this.theDebugInformation,
                    Constants.NumberOfBins,
                    Constants.BestCaseLatency,
                    Constants.WorstCaseLatency);

            ulong theNumberOfMessagePairs = 0;

            double theMinTimestampDifference = double.MaxValue;
            double theMaxTimestampDifference = double.MinValue;

            ulong theMinTimestampPacketNumber = 0;
            ulong theMaxTimestampPacketNumber = 0;

            ulong theMinTimestampSequenceNumber = 0;
            ulong theMaxTimestampSequenceNumber = 0;

            ulong theNumberOfTimestampDifferenceInstances = 0;
            double theTotalOfTimestampDifferences = 0;
            double theAverageTimestampDifference = 0;

            System.Collections.Generic.List<string> theOutOfRangeLatencies =
                new System.Collections.Generic.List<string>();

            foreach (DictionaryKeyValuePairType theRow in theRows)
            {
                // Keep a running total of the number of message pairs
                ++theNumberOfMessagePairs;

                double theTimestampDifference = theRow.Value.TimestampDifference;

                if (!theHistogram.AddValue(theTimestampDifference))
                {
                    theOutOfRangeLatencies.Add(
                        "The message pair for packet number " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theRow.Value.FirstInstancePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " and sequence number " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theRow.Key.SequenceNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " has an out of range latency of " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,18}", theRow.Value.TimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " ms");
                }

                //// Keep a running total of the timestamp differences to allow for averaging

                ++theNumberOfTimestampDifferenceInstances;

                theTotalOfTimestampDifferences += theTimestampDifference;

                if (theMinTimestampDifference > theTimestampDifference)
                {
                    theMinTimestampDifference = theTimestampDifference;
                    theMinTimestampPacketNumber = theRow.Value.FirstInstancePacketNumber;
                    theMinTimestampSequenceNumber = theRow.Key.SequenceNumber;
                }

                if (theMaxTimestampDifference < theTimestampDifference)
                {
                    theMaxTimestampDifference = theTimestampDifference;
                    theMaxTimestampPacketNumber = theRow.Value.FirstInstancePacketNumber;
                    theMaxTimestampSequenceNumber = theRow.Key.SequenceNumber;
                }
            }

            this.theDebugInformation.WriteTextLine(
                "The number of message pairs with a Message Id of " +
                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,5}", theMessageId.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                " was " +
                theNumberOfMessagePairs.ToString(System.Globalization.CultureInfo.CurrentCulture));

            if (theNumberOfTimestampDifferenceInstances > 0)
            {
                theAverageTimestampDifference =
                    theTotalOfTimestampDifferences /
                    theNumberOfTimestampDifferenceInstances;

                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "The minimum latency was " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,18}", theMinTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " ms for packet number " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theMinTimestampPacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " and sequence number " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theMinTimestampSequenceNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)));

                this.theDebugInformation.WriteTextLine(
                    "The maximum latency was " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,18}", theMaxTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " ms for packet number " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theMaxTimestampPacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " and sequence number " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theMaxTimestampSequenceNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)));

                this.theDebugInformation.WriteTextLine(
                    "The average latency was " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,18}", theAverageTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " ms");

                if (this.outputHistogram)
                {
                    //// Output the histogram

                    this.theDebugInformation.WriteBlankLine();

                    this.theDebugInformation.WriteTextLine(
                        "The histogram (" +
                        Constants.BinsPerMillisecond.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                        " bins per millisecond) for the latencies is:");

                    this.theDebugInformation.WriteBlankLine();

                    theHistogram.OutputValues();
                }

                if (theOutOfRangeLatencies.Any())
                {
                    this.theDebugInformation.WriteBlankLine();

                    // Output the data for any message pair with an out of range latency
                    foreach (string theString in theOutOfRangeLatencies)
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
                    System.Globalization.CultureInfo.CurrentCulture, 
                    "{0},{1},{2},{3},{4},,{5}{6}",
                    "First Packet Number",
                    "Second Packet Number",
                    "Sequence Number",
                    "First Packet Timestamp",
                    "Second Packet Timestamp",
                    "Latency (ms)",
                    System.Environment.NewLine);

                theOutputAdditionalInformationLines.Append(
                    theOutputAdditionalInformationTitleLine);

                //// Add a line to the debug output file for each of the processed message pairs

                foreach (DictionaryKeyValuePairType theRow in theRows)
                {
                    string theOutputAdditionalInformationLine = string.Format(
                        System.Globalization.CultureInfo.CurrentCulture, 
                        "{0,7},{1,7},{2,7},{3,18},{4,18},,{5,18}{6}",
                        theRow.Value.FirstInstancePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture),
                        theRow.Value.SecondInstancePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture),
                        theRow.Key.SequenceNumber.ToString(System.Globalization.CultureInfo.CurrentCulture),
                        theRow.Value.FirstInstancePacketTimestamp.ToString(System.Globalization.CultureInfo.CurrentCulture),
                        theRow.Value.SecondInstancePacketTimestamp.ToString(System.Globalization.CultureInfo.CurrentCulture),
                        theRow.Value.TimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture),
                        System.Environment.NewLine);

                    theOutputAdditionalInformationLines.Append(
                        theOutputAdditionalInformationLine);
                }

                System.IO.File.WriteAllText(
                    this.theSelectedPacketCaptureFile +
                    ".MessageId" +
                    theMessageId +
                    ".LatencyAnalysis.csv",
                    theOutputAdditionalInformationLines.ToString());
            }
        }
    }
}
