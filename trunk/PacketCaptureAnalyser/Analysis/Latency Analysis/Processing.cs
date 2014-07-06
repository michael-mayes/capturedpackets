// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace Analysis.LatencyAnalysis
{
    using System.Data; // Required to be able to use AsEnumerable method
    using System.Linq; // Required to be able to use Count method

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
        /// 
        /// </summary>
        private System.Collections.Generic.Dictionary<Structures.DictionaryKey,
            Structures.DictionaryValue> theDictionary;

        /// <summary>
        /// 
        /// </summary>
        private Analysis.DebugInformation theDebugInformation;

        /// <summary>
        /// 
        /// </summary>
        private bool outputDebug;

        /// <summary>
        /// 
        /// </summary>
        private string selectedPacketCaptureFile;

        /// <summary>
        /// 
        /// </summary>
        private System.Data.DataTable theHostIdsTable;

        /// <summary>
        /// 
        /// </summary>
        private System.Data.DataTable theMessageIdsTable;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="outputDebug"></param>
        /// <param name="selectedPacketCaptureFile"></param>
        public Processing(Analysis.DebugInformation theDebugInformation, bool outputDebug, string selectedPacketCaptureFile)
        {
            this.theDebugInformation = theDebugInformation;

            this.outputDebug = outputDebug;

            this.selectedPacketCaptureFile = selectedPacketCaptureFile;

            // Create a dictionary to hold the latency values for message pairings
            this.theDictionary =
                new System.Collections.Generic.Dictionary<Structures.DictionaryKey, Structures.DictionaryValue>();

            // Create a datatable to hold the set of host Ids encountered during the latency analysis
            this.theHostIdsTable = new System.Data.DataTable();

            // Create a datatable to hold the set of message Ids encountered during the latency analysis
            this.theMessageIdsTable = new System.Data.DataTable();
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theHostId"></param>
        /// <param name="theProtocol"></param>
        /// <param name="theSequenceNumber"></param>
        /// <param name="theMessageId"></param>
        /// <param name="thePacketNumber"></param>
        /// <param name="theTimestamp">The timestamp read from the packet capture</param>
        public void RegisterMessageReceipt(byte theHostId, Constants.Protocol theProtocol, ulong theSequenceNumber, ulong theMessageId, ulong thePacketNumber, double theTimestamp)
        {
            // Do not process messages where the sequence number is not populated as we would not be able match message pairs using them
            if (theSequenceNumber == 0)
            {
                return;
            }

            // Do not process messages where the message Id is not populated as we would not be able match message pairs using them
            if (theHostId == 0)
            {
                return;
            }

            // Do not process messages where the message Id is not populated as we would not be able match message pairs using them
            if (theMessageId == 0)
            {
                return;
            }

            //// Add the supplied sequence number and timestamp to latency values dictionary

            Structures.DictionaryKey theDictionaryKey =
                new Structures.DictionaryKey(
                    theHostId,
                    theProtocol,
                    theSequenceNumber);

            //// Check whether there is a dictionary entry for this key i.e. is this the first message of the pair

            Structures.DictionaryValue theDictionaryValueFound;

            bool theEntryFound = this.theDictionary.TryGetValue(theDictionaryKey, out theDictionaryValueFound);

            if (!theEntryFound)
            {
                // If this is the first message of the pairing then create the new entry in the dictionary
                Structures.DictionaryValue
                    theDictionaryValueToAdd =
                    new Structures.DictionaryValue
                    {
                        MessageId = theMessageId,
                        FirstInstanceFound = true,
                        SecondInstanceFound = false,
                        FirstInstancePacketNumber = thePacketNumber,
                        SecondInstancePacketNumber = 0,
                        FirstInstanceTimestamp = theTimestamp,
                        SecondInstanceTimestamp = 0.0,
                        TimestampDifference = 0.0,
                        TimestampDifferenceCalculated = false
                    };

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
                        theHostId.ToString() +
                        " and the sequence number " +
                        theSequenceNumber.ToString() +
                        " but the FirstInstanceFound flag is not set!!!");

                    return;
                }

                if (theDictionaryValueFound.SecondInstanceFound)
                {
                    this.theDebugInformation.WriteErrorEvent(
                        "Found the row for the Host Id " +
                        theHostId.ToString() +
                        " and the sequence number "
                        + theSequenceNumber.ToString() +
                        " but the SecondInstanceFound flag is already set!!!");

                    return;
                }

                theDictionaryValueFound.SecondInstanceFound = true;
                theDictionaryValueFound.SecondInstancePacketNumber = thePacketNumber;

                if (theTimestamp > theDictionaryValueFound.FirstInstanceTimestamp)
                {
                    theDictionaryValueFound.SecondInstanceTimestamp = theTimestamp;
                    theDictionaryValueFound.TimestampDifference = (theTimestamp - theDictionaryValueFound.FirstInstanceTimestamp) * 1000.0; // Milliseconds
                    theDictionaryValueFound.TimestampDifferenceCalculated = true;
                }
                else if (theTimestamp == theDictionaryValueFound.FirstInstanceTimestamp)
                {
                    theDictionaryValueFound.SecondInstanceTimestamp = 0.0;
                    theDictionaryValueFound.TimestampDifference = 0.0;
                    theDictionaryValueFound.TimestampDifferenceCalculated = true;
                }
                else
                {
                    this.theDebugInformation.WriteErrorEvent(
                        "Found the row for the Host Id " +
                        theHostId.ToString() +
                        " and the sequence number " +
                        theSequenceNumber.ToString() +
                        ", but the timestamp of the first message is higher than that of the second message!!!");

                    theDictionaryValueFound.TimestampDifferenceCalculated = false;
                }

                // Update the values in the dictionary entry
                this.theDictionary[theDictionaryKey] = theDictionaryValueFound;

                // Add the supplied host Id to the set of those encountered during the latency analysis if not already in there
                this.RegisterEncounteredHostId(theHostId);

                // Add the supplied message Id to the set of those encountered during the latency analysis if not already in there
                this.RegisterEncounteredMessageId(theHostId, theMessageId);
            }
        }

        /// <summary>
        /// 
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
                    string.Format("{0,3}", theHostIdRow.Field<byte>("HostId").ToString()));

                this.theDebugInformation.WriteTextLine("===========");
                this.theDebugInformation.WriteBlankLine();

                this.FinaliseProtocolsForHostId(theHostIdRow.Field<byte>("HostId"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
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
        /// <param name="theHostId"></param>
        private void RegisterEncounteredHostId(byte theHostId)
        {
            object[] theHostIdRowFindObject = new object[1];

            theHostIdRowFindObject[0] = theHostId.ToString(); // Primary key

            System.Data.DataRow theHostIdDataRowFound = this.theHostIdsTable.Rows.Find(theHostIdRowFindObject);

            if (theHostIdDataRowFound == null)
            {
                this.theDebugInformation.WriteInformationEvent(
                    "Found a pair of data-supplying messages for a Host Id " +
                    string.Format("{0,3}", theHostId) +
                    " - adding this Host Id to the latency analysis");

                System.Data.DataRow theHostIdRowToAdd = this.theHostIdsTable.NewRow();

                theHostIdRowToAdd["HostId"] = theHostId;

                this.theHostIdsTable.Rows.Add(theHostIdRowToAdd);
            }
        }

        /// <summary>
        /// Adds the supplied message Id to the set of those encountered during the latency analysis if not already in there
        /// </summary>
        /// <param name="theHostId"></param>
        /// <param name="theMessageId"></param>
        private void RegisterEncounteredMessageId(byte theHostId, ulong theMessageId)
        {
            object[] theMessageIdRowFindObject = new object[2];

            theMessageIdRowFindObject[0] = theHostId.ToString(); // Primary key (part one)
            theMessageIdRowFindObject[1] = theMessageId.ToString(); // Primary key (part two)

            System.Data.DataRow theMessageIdDataRowFound = this.theMessageIdsTable.Rows.Find(theMessageIdRowFindObject);

            if (theMessageIdDataRowFound == null)
            {
                this.theDebugInformation.WriteInformationEvent(
                    "Found a pair of data-supplying messages with a Message Id " +
                    string.Format("{0,5}", theMessageId) +
                    " for a Host Id " +
                    string.Format("{0,3}", theHostId) +
                    " - adding this Message Id/Host Id combination to the latency analysis");

                System.Data.DataRow theMessageIdRowToAdd = this.theMessageIdsTable.NewRow();

                theMessageIdRowToAdd["HostId"] = theHostId;
                theMessageIdRowToAdd["MessageId"] = theMessageId;

                this.theMessageIdsTable.Rows.Add(theMessageIdRowToAdd);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theHostId"></param>
        private void FinaliseProtocolsForHostId(byte theHostId)
        {
            foreach (Constants.Protocol theProtocol in
                System.Enum.GetValues(typeof(Constants.Protocol)))
            {
                string theProtocolString = ((Constants.Protocol)theProtocol).ToString();

                this.theDebugInformation.WriteTextLine(
                    theProtocolString +
                    " messages");

                this.theDebugInformation.WriteTextLine("------------");
                this.theDebugInformation.WriteBlankLine();

                //// Obtain the set of message Ids encountered for this host Id during the latency analysis in ascending order

                EnumerableRowCollection<System.Data.DataRow>
                    theMessageIdRowsFound =
                    from r in this.theMessageIdsTable.AsEnumerable()
                    where r.Field<byte>("HostId") == theHostId
                    orderby r.Field<ulong>("MessageId") ascending
                    select r;

                //// Loop across all the latency values for the message pairings using each of these message Ids in turn

                foreach (System.Data.DataRow theMessageIdRow in theMessageIdRowsFound)
                {
                    DictionaryEnumerableType
                        theLatencyValueEntriesFound =
                        from s in this.theDictionary.AsEnumerable()
                        where s.Key.Protocol == theProtocol &&
                        s.Value.MessageId == theMessageIdRow.Field<ulong>("MessageId") &&
                        s.Value.TimestampDifferenceCalculated
                        select s;

                    int theRowsFoundCount = theLatencyValueEntriesFound.Count();

                    if (theRowsFoundCount > 0)
                    {
                        this.theDebugInformation.WriteTextLine(
                            "The number of pairs of " +
                            theProtocolString +
                            " messages with a Message Id of " +
                            theMessageIdRow.Field<ulong>("MessageId").ToString() +
                            " was " +
                            theRowsFoundCount.ToString());

                        this.FinaliseForMessageId(theProtocolString, theMessageIdRow.Field<ulong>("MessageId"), theLatencyValueEntriesFound);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theProtocolString"></param>
        /// <param name="theMessageId"></param>
        /// <param name="theRows"></param>
        private void FinaliseForMessageId(string theProtocolString, ulong theMessageId, DictionaryEnumerableType theRows)
        {
            CommonHistogram theHistogram =
                new CommonHistogram(
                    this.theDebugInformation,
                    Constants.NumberOfBins,
                    Constants.BestCaseLatency,
                    Constants.WorstCaseLatency);

            ulong theMinTimestampPacketNumber = 0;
            ulong theMaxTimestampPacketNumber = 0;

            ulong theMinTimestampSequenceNumber = 0;
            ulong theMaxTimestampSequenceNumber = 0;

            double theMinTimestampDifference = double.MaxValue;
            double theMaxTimestampDifference = double.MinValue;

            ulong theNumberOfTimestampDifferenceInstances = 0;
            double theTotalOfTimestampDifferences = 0;
            double theAverageTimestampDifference = 0;

            foreach (DictionaryKeyValuePairType theRow in theRows)
            {
                double theTimestampDifference = theRow.Value.TimestampDifference;

                theHistogram.AddValue(theTimestampDifference);

                // Keep a running total to allow for averaging
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

            if (theNumberOfTimestampDifferenceInstances > 0)
            {
                theAverageTimestampDifference = theTotalOfTimestampDifferences / theNumberOfTimestampDifferenceInstances;

                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "The minimum latency for pairs of " +
                    theProtocolString +
                    " messages with a Message Id of " +
                    theMessageId.ToString() +
                    " was " +
                    theMinTimestampDifference.ToString() +
                    " ms for packet number " +
                    theMinTimestampPacketNumber.ToString() +
                    " and sequence number " +
                    theMinTimestampSequenceNumber.ToString());

                this.theDebugInformation.WriteTextLine(
                    "The maximum latency for pairs of " +
                    theProtocolString +
                    " messages with a Message Id of " +
                    theMessageId.ToString() +
                    " was " +
                    theMaxTimestampDifference.ToString() +
                    " ms for packet number " +
                    theMaxTimestampPacketNumber.ToString() +
                    " and sequence number " +
                    theMaxTimestampSequenceNumber.ToString());

                this.theDebugInformation.WriteTextLine(
                    "The average latency for pairs of " +
                    theProtocolString +
                    " messages with a Message Id of " +
                    theMessageId.ToString() +
                    " was " +
                    theAverageTimestampDifference.ToString() +
                    " ms");

                this.theDebugInformation.WriteBlankLine();

                //// Output the histogram

                this.theDebugInformation.WriteTextLine(
                    "The histogram (" +
                    Constants.BinsPerMillisecond.ToString() +
                    " bins per millisecond) for latency values for " +
                    theProtocolString +
                    " messages with a Message Id of " +
                    theMessageId.ToString() +
                    " is:");

                this.theDebugInformation.WriteBlankLine();

                theHistogram.OutputValues();
            }

            this.theDebugInformation.WriteBlankLine();

            if (this.outputDebug)
            {
                System.Text.StringBuilder outputDebugLines = new System.Text.StringBuilder();

                string outputDebugTitleLine = string.Format(
                    "{0},{1},{2},{3}{4}",
                    "First Packet Number",
                    "Second Packet Number",
                    "Sequence Number",
                    "Latency",
                    System.Environment.NewLine);

                outputDebugLines.Append(outputDebugTitleLine);

                foreach (DictionaryKeyValuePairType theRow in theRows)
                {
                    string outputDebugLine = string.Format(
                        "{0},{1},{2},{3}{4}",
                        theRow.Value.FirstInstancePacketNumber.ToString(),
                        theRow.Value.SecondInstancePacketNumber.ToString(),
                        theRow.Key.SequenceNumber.ToString(),
                        theRow.Value.TimestampDifference.ToString(),
                        System.Environment.NewLine);

                    outputDebugLines.Append(outputDebugLine);
                }

                System.IO.File.WriteAllText(
                    this.selectedPacketCaptureFile +
                    ".MessageId" +
                    theMessageId +
                    ".LatencyAnalysis.csv",
                    outputDebugLines.ToString());
            }
        }
    }
}
