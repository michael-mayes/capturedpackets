﻿//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace Analysis.LatencyAnalysis
{
    using System.Data; //Required to be able to use AsEnumerable method
    using System.Linq; //Required to be able to use Count method

    //Create an alias for the key value pair for the dictionary to improve clarity of later code that uses it
    using DictionaryKeyValuePairType = System.Collections.Generic.KeyValuePair<Structures.DictionaryKey, Structures.DictionaryValue>;

    //Create an alias for the enumerable for the dictionary to improve clarity of later code that uses it
    //Cannot nest using declarations so must use the declaration of the key value pair type in full again
    using DictionaryEnumerableType = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Structures.DictionaryKey, Structures.DictionaryValue>>;

    //This class will implement the Disposable class so as to be able to clean up after the datatables it creates which themselves implement the Disposable class
    class Processing : System.IDisposable
    {
        private System.Collections.Generic.Dictionary
            <
            Structures.DictionaryKey,
            Structures.DictionaryValue
            > TheDictionary;

        private Analysis.DebugInformation TheDebugInformation;

        private bool OutputDebug;

        private string SelectedPacketCaptureFile;

        private System.Data.DataTable TheHostIdsTable;
        private System.Data.DataTable TheMessageIdsTable;

        public Processing(Analysis.DebugInformation TheDebugInformation, bool OutputDebug, string SelectedPacketCaptureFile)
        {
            this.TheDebugInformation = TheDebugInformation;

            this.OutputDebug = OutputDebug;

            this.SelectedPacketCaptureFile = SelectedPacketCaptureFile;

            //Create a dictionary to hold the latency values for message pairings
            TheDictionary =
                new System.Collections.Generic.Dictionary
                    <Structures.DictionaryKey, Structures.DictionaryValue>();

            //Create a datatable to hold the set of host Ids encountered during the latency analysis
            TheHostIdsTable = new System.Data.DataTable();

            //Create a datatable to hold the set of message Ids encountered during the latency analysis
            TheMessageIdsTable = new System.Data.DataTable();
        }

        public void Create()
        {
            //Add the required column to the datatable to hold the set of host Ids encountered during the latency analysis
            TheHostIdsTable.Columns.Add("HostId", typeof(byte));

            //Set the primary key to be the only column
            //The primary key is needed to allow for use of the Find method against the datatable
            TheHostIdsTable.PrimaryKey =
                new System.Data.DataColumn[]
                {
                    TheHostIdsTable.Columns["HostId"]
                };

            //Add the required columns to the datatable to hold the set of message Ids encountered during the latency analysis
            TheMessageIdsTable.Columns.Add("HostId", typeof(byte));
            TheMessageIdsTable.Columns.Add("MessageId", typeof(ulong));

            //Set a multi-column primary key
            //The primary key is needed to allow for use of the Find method against the datatable
            TheMessageIdsTable.PrimaryKey =
                new System.Data.DataColumn[]
                {
                    TheMessageIdsTable.Columns["HostId"],
                    TheMessageIdsTable.Columns["MessageId"]
                };
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                //Dispose any resources allocated to the datatables if instructed
                TheHostIdsTable.Dispose();
                TheMessageIdsTable.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);

            System.GC.SuppressFinalize(this);
        }

        public void RegisterMessageReceipt(byte TheHostId, Constants.Protocol TheProtocol, ulong TheSequenceNumber, ulong TheMessageId, ulong ThePacketNumber, double TheTimestamp)
        {
            //Do not process messages where the sequence number is not populated as we would not be able match message pairs using them
            if (TheSequenceNumber == 0)
            {
                return;
            }

            //Do not process messages where the message Id is not populated as we would not be able match message pairs using them
            if (TheHostId == 0)
            {
                return;
            }

            //Do not process messages where the message Id is not populated as we would not be able match message pairs using them
            if (TheMessageId == 0)
            {
                return;
            }

            //Add the supplied sequence number and timestamp to latency values dictionary

            Structures.DictionaryKey TheDictionaryKey =
                new Structures.DictionaryKey
                    (
                    TheHostId,
                    TheProtocol,
                    TheSequenceNumber
                    );

            //Check whether there is a dictionary entry for this key i.e. is this the first message of the pair

            Structures.DictionaryValue TheDictionaryValueFound;

            bool TheEntryFound = TheDictionary.TryGetValue
                (TheDictionaryKey, out TheDictionaryValueFound);

            if (!TheEntryFound)
            {
                //If this is the first message of the pairing then create the new entry in the dictionary

                Structures.DictionaryValue
                    TheDictionaryValueToAdd =
                    new Structures.DictionaryValue
                    {
                        MessageId = TheMessageId,
                        FirstInstanceFound = true,
                        SecondInstanceFound = false,
                        FirstInstancePacketNumber = ThePacketNumber,
                        SecondInstancePacketNumber = 0,
                        FirstInstanceTimestamp = TheTimestamp,
                        SecondInstanceTimestamp = 0.0,
                        TimestampDifference = 0.0,
                        TimestampDifferenceCalculated = false
                    };

                //Add the new entry to the dictionary
                TheDictionary.Add
                    (
                    TheDictionaryKey,
                    TheDictionaryValueToAdd
                    );
            }
            else
            {
                //If this is the second message of the pairing then update the row and calculate the difference in timestamps i.e. the latency

                if (!TheDictionaryValueFound.FirstInstanceFound)
                {
                    TheDebugInformation.WriteErrorEvent
                        (
                        "Found the row for the Host Id " +
                        TheHostId.ToString() +
                        " and the sequence number " +
                        TheSequenceNumber.ToString() +
                        " but the FirstInstanceFound flag is not set!!!"
                        );

                    return;
                }

                if (TheDictionaryValueFound.SecondInstanceFound)
                {
                    TheDebugInformation.WriteErrorEvent
                        (
                        "Found the row for the Host Id " +
                        TheHostId.ToString() +
                        " and the sequence number "
                        + TheSequenceNumber.ToString() +
                        " but the SecondInstanceFound flag is already set!!!"
                        );

                    return;
                }

                TheDictionaryValueFound.SecondInstanceFound = true;
                TheDictionaryValueFound.SecondInstancePacketNumber = ThePacketNumber;

                if (TheTimestamp > TheDictionaryValueFound.FirstInstanceTimestamp)
                {
                    TheDictionaryValueFound.SecondInstanceTimestamp = TheTimestamp;
                    TheDictionaryValueFound.TimestampDifference = (TheTimestamp - TheDictionaryValueFound.FirstInstanceTimestamp) * 1000.0; //Milliseconds
                    TheDictionaryValueFound.TimestampDifferenceCalculated = true;
                }
                else if (TheTimestamp == TheDictionaryValueFound.FirstInstanceTimestamp)
                {
                    TheDictionaryValueFound.SecondInstanceTimestamp = 0.0;
                    TheDictionaryValueFound.TimestampDifference = 0.0;
                    TheDictionaryValueFound.TimestampDifferenceCalculated = true;
                }
                else
                {
                    TheDebugInformation.WriteErrorEvent
                        (
                        "Found the row for the Host Id " +
                        TheHostId.ToString() +
                        " and the sequence number " +
                        TheSequenceNumber.ToString() +
                        ", but the timestamp of the first message is higher than that of the second message!!!"
                        );

                    TheDictionaryValueFound.TimestampDifferenceCalculated = false;
                }

                //Update the values in the dictionary entry
                TheDictionary[TheDictionaryKey] = TheDictionaryValueFound;

                //Add the supplied host Id to the set of those encountered during the latency analysis if not already in there
                RegisterEncounteredHostId(TheHostId);

                //Add the supplied message Id to the set of those encountered during the latency analysis if not already in there
                RegisterEncounteredMessageId(TheHostId, TheMessageId);
            }
        }

        //Add the supplied host Id to the set of those encountered during the latency analysis if not already in there
        private void RegisterEncounteredHostId(byte TheHostId)
        {
            object[] TheHostIdRowFindObject = new object[1];

            TheHostIdRowFindObject[0] = TheHostId.ToString(); //Primary key

            System.Data.DataRow TheHostIdDataRowFound = TheHostIdsTable.Rows.Find(TheHostIdRowFindObject);

            if (TheHostIdDataRowFound == null)
            {
                TheDebugInformation.WriteInformationEvent
                    (
                    "Found a pair of data-supplying messages for a Host Id " +
                    string.Format("{0,3}", TheHostId) +
                    " - adding this Host Id to the latency analysis"
                    );

                System.Data.DataRow TheHostIdRowToAdd = TheHostIdsTable.NewRow();

                TheHostIdRowToAdd["HostId"] = TheHostId;

                TheHostIdsTable.Rows.Add(TheHostIdRowToAdd);
            }
        }

        //Add the supplied message Id to the set of those encountered during the latency analysis if not already in there
        private void RegisterEncounteredMessageId(byte TheHostId, ulong TheMessageId)
        {
            object[] TheMessageIdRowFindObject = new object[2];

            TheMessageIdRowFindObject[0] = TheHostId.ToString(); //Primary key (part one)
            TheMessageIdRowFindObject[1] = TheMessageId.ToString(); //Primary key (part two)

            System.Data.DataRow TheMessageIdDataRowFound = TheMessageIdsTable.Rows.Find(TheMessageIdRowFindObject);

            if (TheMessageIdDataRowFound == null)
            {
                TheDebugInformation.WriteInformationEvent
                    (
                    "Found a pair of data-supplying messages with a Message Id " +
                    string.Format("{0,5}", TheMessageId) +
                    " for a Host Id " +
                    string.Format("{0,3}", TheHostId) +
                    " - adding this Message Id/Host Id combination to the latency analysis"
                    );

                System.Data.DataRow TheMessageIdRowToAdd = TheMessageIdsTable.NewRow();

                TheMessageIdRowToAdd["HostId"] = TheHostId;
                TheMessageIdRowToAdd["MessageId"] = TheMessageId;

                TheMessageIdsTable.Rows.Add(TheMessageIdRowToAdd);
            }
        }

        public void Finalise()
        {
            //Obtain the set of host Ids encountered during the latency analysis in ascending order

            EnumerableRowCollection<System.Data.DataRow>
                TheHostIdRowsFound =
                from r in TheHostIdsTable.AsEnumerable()
                orderby r.Field<byte>("HostId") ascending
                select r;

            //Loop across all the latency values for the message pairings using each of these host Ids in turn

            TheDebugInformation.WriteBlankLine();
            TheDebugInformation.WriteTextString("======================");
            TheDebugInformation.WriteTextString("== Latency Analysis ==");
            TheDebugInformation.WriteTextString("======================");
            TheDebugInformation.WriteBlankLine();

            foreach (System.Data.DataRow TheHostIdRow in TheHostIdRowsFound)
            {
                TheDebugInformation.WriteTextString
                    (
                    "Host Id " +
                    string.Format("{0,3}", (TheHostIdRow.Field<byte>("HostId")).ToString())
                    );

                TheDebugInformation.WriteTextString("===========");
                TheDebugInformation.WriteBlankLine();

                FinaliseProtocolsForHostId(TheHostIdRow.Field<byte>("HostId"));
            }
        }

        private void FinaliseProtocolsForHostId(byte TheHostId)
        {
            foreach (Constants.Protocol TheProtocol in
                System.Enum.GetValues(typeof(Constants.Protocol)))
            {
                string TheProtocolString = ((Constants.Protocol)TheProtocol).ToString();

                TheDebugInformation.WriteTextString
                    (
                    TheProtocolString +
                    " messages"
                    );

                TheDebugInformation.WriteTextString("------------");
                TheDebugInformation.WriteBlankLine();

                //Obtain the set of message Ids encountered for this host Id during the latency analysis in ascending order

                EnumerableRowCollection<System.Data.DataRow>
                    TheMessageIdRowsFound =
                    from r in TheMessageIdsTable.AsEnumerable()
                    where r.Field<byte>("HostId") == TheHostId
                    orderby r.Field<ulong>("MessageId") ascending
                    select r;

                //Loop across all the latency values for the message pairings using each of these message Ids in turn

                foreach (System.Data.DataRow TheMessageIdRow in TheMessageIdRowsFound)
                {
                    DictionaryEnumerableType
                        TheLatencyValueEntriesFound =
                        from s in TheDictionary.AsEnumerable()
                        where s.Key.Protocol == TheProtocol &&
                        s.Value.MessageId == TheMessageIdRow.Field<ulong>("MessageId") &&
                        s.Value.TimestampDifferenceCalculated
                        select s;

                    int TheRowsFoundCount = TheLatencyValueEntriesFound.Count();

                    if (TheRowsFoundCount > 0)
                    {
                        TheDebugInformation.WriteTextString
                            (
                            "The number of pairs of " +
                            TheProtocolString +
                            " messages with a Message Id of " +
                            (TheMessageIdRow.Field<ulong>("MessageId")).ToString() +
                            " was " +
                            TheRowsFoundCount.ToString()
                            );

                        FinaliseForMessageId(TheProtocolString, TheMessageIdRow.Field<ulong>("MessageId"), TheLatencyValueEntriesFound);
                    }
                }
            }
        }

        private void FinaliseForMessageId(string TheProtocolString, ulong TheMessageId, DictionaryEnumerableType TheRows)
        {
            CommonHistogram TheHistogram =
                new CommonHistogram
                    (
                    TheDebugInformation,
                    Constants.NumberOfBins,
                    Constants.BestCaseLatency,
                    Constants.WorstCaseLatency
                    );

            ulong TheMinTimestampPacketNumber = 0;
            ulong TheMaxTimestampPacketNumber = 0;

            ulong TheMinTimestampSequenceNumber = 0;
            ulong TheMaxTimestampSequenceNumber = 0;

            double TheMinTimestampDifference = double.MaxValue;
            double TheMaxTimestampDifference = double.MinValue;

            ulong TheNumberOfTimestampDifferenceInstances = 0;
            double TheTotalOfTimestampDifferences = 0;
            double TheAverageTimestampDifference = 0;

            foreach (DictionaryKeyValuePairType TheRow in TheRows)
            {
                double TheTimestampDifference = TheRow.Value.TimestampDifference;

                TheHistogram.AddValue(TheTimestampDifference);

                //Keep a running total to allow for averaging
                ++TheNumberOfTimestampDifferenceInstances;
                TheTotalOfTimestampDifferences += TheTimestampDifference;

                if (TheMinTimestampDifference > TheTimestampDifference)
                {
                    TheMinTimestampDifference = TheTimestampDifference;
                    TheMinTimestampPacketNumber = TheRow.Value.FirstInstancePacketNumber;
                    TheMinTimestampSequenceNumber = TheRow.Key.SequenceNumber;
                }

                if (TheMaxTimestampDifference < TheTimestampDifference)
                {
                    TheMaxTimestampDifference = TheTimestampDifference;
                    TheMaxTimestampPacketNumber = TheRow.Value.FirstInstancePacketNumber;
                    TheMaxTimestampSequenceNumber = TheRow.Key.SequenceNumber;
                }
            }

            if (TheNumberOfTimestampDifferenceInstances > 0)
            {
                TheAverageTimestampDifference = (TheTotalOfTimestampDifferences / TheNumberOfTimestampDifferenceInstances);

                TheDebugInformation.WriteBlankLine();

                TheDebugInformation.WriteTextString
                    (
                    "The minimum latency for pairs of " +
                    TheProtocolString +
                    " messages with a Message Id of " +
                    TheMessageId.ToString() +
                    " was " +
                    TheMinTimestampDifference.ToString() +
                    " ms for packet number " +
                    TheMinTimestampPacketNumber.ToString() +
                    " and sequence number " +
                    TheMinTimestampSequenceNumber.ToString()
                    );

                TheDebugInformation.WriteTextString
                    (
                    "The maximum latency for pairs of " +
                    TheProtocolString +
                    " messages with a Message Id of " +
                    TheMessageId.ToString() +
                    " was " +
                    TheMaxTimestampDifference.ToString() +
                    " ms for packet number " +
                    TheMaxTimestampPacketNumber.ToString() +
                    " and sequence number " +
                    TheMaxTimestampSequenceNumber.ToString()
                    );

                TheDebugInformation.WriteTextString
                    (
                    "The average latency for pairs of " +
                    TheProtocolString +
                    " messages with a Message Id of " +
                    TheMessageId.ToString() +
                    " was " +
                    TheAverageTimestampDifference.ToString() +
                    " ms"
                    );

                TheDebugInformation.WriteBlankLine();

                //Output the histogram

                TheDebugInformation.WriteTextString
                    (
                    "The histogram (" +
                    Constants.BinsPerMillisecond.ToString() +
                    " bins per millisecond) for latency values for " +
                    TheProtocolString +
                    " messages with a Message Id of " +
                    TheMessageId.ToString() +
                    " is:"
                    );

                TheDebugInformation.WriteBlankLine();

                TheHistogram.OutputValues();
            }

            TheDebugInformation.WriteBlankLine();

            if (OutputDebug)
            {
                System.Text.StringBuilder OutputDebugLines = new System.Text.StringBuilder();

                string OutputDebugTitleLine = string.Format
                    (
                    "{0},{1},{2},{3}{4}",
                    "First Packet Number",
                    "Second Packet Number",
                    "Sequence Number",
                    "Latency",
                    System.Environment.NewLine
                    );

                OutputDebugLines.Append(OutputDebugTitleLine);

                foreach (DictionaryKeyValuePairType TheRow in TheRows)
                {
                    string OutputDebugLine = string.Format
                        (
                        "{0},{1},{2},{3}{4}",
                        TheRow.Value.FirstInstancePacketNumber.ToString(),
                        TheRow.Value.SecondInstancePacketNumber.ToString(),
                        TheRow.Key.SequenceNumber.ToString(),
                        TheRow.Value.TimestampDifference.ToString(),
                        System.Environment.NewLine
                        );

                    OutputDebugLines.Append(OutputDebugLine);
                }

                System.IO.File.WriteAllText
                    (
                    SelectedPacketCaptureFile + ".MessageId" + TheMessageId + ".LatencyAnalysis.csv",
                    OutputDebugLines.ToString()
                    );
            }
        }
    }
}