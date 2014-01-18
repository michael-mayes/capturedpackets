//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

namespace AnalysisNamespace.LatencyAnalysisNamespace
{
    using System.Data; //Required to be able to use AsEnumerable method
    using System.Linq; //Required to be able to use Count method

    //Create an alias for the key value pair for the dictionary to improve clarity of later code that uses it
    using LatencyAnalysisDictionaryKeyValuePairType = System.Collections.Generic.KeyValuePair<Structures.LatencyAnalysisDictionaryKey, Structures.LatencyAnalysisDictionaryValue>;

    //Create an alias for the enumerable for the dictionary to improve clarity of later code that uses it
    //Cannot nest using declarations so must use the declaration of the key value pair type in full again
    using LatencyAnalysisDictionaryEnumerableType = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Structures.LatencyAnalysisDictionaryKey, Structures.LatencyAnalysisDictionaryValue>>;

    //This class will implement the Disposable class so as to be able to clean up after the datatables it creates which themselves implement the Disposable class
    class Processing : System.IDisposable
    {
        private System.Collections.Generic.Dictionary
            <
            Structures.LatencyAnalysisDictionaryKey,
            Structures.LatencyAnalysisDictionaryValue
            > TheLatencyValuesDictionary;

        private bool OutputLatencyAnalysisDebug;

        private System.Data.DataTable TheHostIdsTable;
        private System.Data.DataTable TheMessageIdsTable;

        public Processing(bool OutputLatencyAnalysisDebug)
        {
            this.OutputLatencyAnalysisDebug = OutputLatencyAnalysisDebug;

            //Create a dictionary to hold the latency values for message pairings
            TheLatencyValuesDictionary =
                new System.Collections.Generic.Dictionary<Structures.LatencyAnalysisDictionaryKey, Structures.LatencyAnalysisDictionaryValue>();

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

        public void RegisterMessageReceipt(byte TheHostId, Constants.LatencyAnalysisProtocol TheProtocol, ulong TheSequenceNumber, ulong TheMessageId, ulong ThePacketNumber, double TheTimestamp)
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

            Structures.LatencyAnalysisDictionaryKey TheLatencyAnalysisDictionaryKey =
                new Structures.LatencyAnalysisDictionaryKey
                    (
                    TheHostId,
                    TheProtocol,
                    TheSequenceNumber
                    );

            //Check whether there is a dictionary entry for this key i.e. is this the first message of the pair

            Structures.LatencyAnalysisDictionaryValue TheLatencyAnalysisDictionaryValueFound;

            bool TheLatencyValuesEntryFound = TheLatencyValuesDictionary.TryGetValue
                (TheLatencyAnalysisDictionaryKey, out TheLatencyAnalysisDictionaryValueFound);

            if (!TheLatencyValuesEntryFound)
            {
                //If this is the first message of the pairing then create the new entry in the dictionary

                Structures.LatencyAnalysisDictionaryValue
                    TheLatencyAnalysisDictionaryValueToAdd =
                    new Structures.LatencyAnalysisDictionaryValue
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
                TheLatencyValuesDictionary.Add
                    (
                    TheLatencyAnalysisDictionaryKey,
                    TheLatencyAnalysisDictionaryValueToAdd
                    );
            }
            else
            {
                //If this is the second message of the pairing then update the row and calculate the difference in timestamps i.e. the latency

                if (!TheLatencyAnalysisDictionaryValueFound.FirstInstanceFound)
                {
                    System.Diagnostics.Trace.WriteLine
                        (
                        "Found the row for the Host Id " +
                        TheHostId.ToString() +
                        " and the sequence number " +
                        TheSequenceNumber.ToString() +
                        " but the FirstInstanceFound flag is not set!!!"
                        );

                    return;
                }

                if (TheLatencyAnalysisDictionaryValueFound.SecondInstanceFound)
                {
                    System.Diagnostics.Trace.WriteLine
                        (
                        "Found the row for the Host Id " +
                        TheHostId.ToString() +
                        " and the sequence number "
                        + TheSequenceNumber.ToString() +
                        " but the SecondInstanceFound flag is already set!!!"
                        );

                    return;
                }

                TheLatencyAnalysisDictionaryValueFound.SecondInstanceFound = true;
                TheLatencyAnalysisDictionaryValueFound.SecondInstancePacketNumber = ThePacketNumber;

                if (TheTimestamp > TheLatencyAnalysisDictionaryValueFound.FirstInstanceTimestamp)
                {
                    TheLatencyAnalysisDictionaryValueFound.SecondInstanceTimestamp = TheTimestamp;
                    TheLatencyAnalysisDictionaryValueFound.TimestampDifference = (TheTimestamp - TheLatencyAnalysisDictionaryValueFound.FirstInstanceTimestamp) * 1000.0; //Milliseconds
                    TheLatencyAnalysisDictionaryValueFound.TimestampDifferenceCalculated = true;
                }
                else if (TheTimestamp == TheLatencyAnalysisDictionaryValueFound.FirstInstanceTimestamp)
                {
                    TheLatencyAnalysisDictionaryValueFound.SecondInstanceTimestamp = 0.0;
                    TheLatencyAnalysisDictionaryValueFound.TimestampDifference = 0.0;
                    TheLatencyAnalysisDictionaryValueFound.TimestampDifferenceCalculated = true;
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine
                        (
                        "Found the row for the Host Id " +
                        TheHostId.ToString() +
                        " and the sequence number " +
                        TheSequenceNumber.ToString() +
                        ", but the timestamp of the first message is higher than that of the second message!!!"
                        );

                    TheLatencyAnalysisDictionaryValueFound.TimestampDifferenceCalculated = false;
                }

                //Update the values in the dictionary entry
                TheLatencyValuesDictionary[TheLatencyAnalysisDictionaryKey] = TheLatencyAnalysisDictionaryValueFound;

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
                System.Diagnostics.Trace.WriteLine
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
                System.Diagnostics.Trace.WriteLine
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

            System.Diagnostics.Trace.Write(System.Environment.NewLine);
            System.Diagnostics.Trace.WriteLine("======================");
            System.Diagnostics.Trace.WriteLine("== Latency Analysis ==");
            System.Diagnostics.Trace.WriteLine("======================");
            System.Diagnostics.Trace.Write(System.Environment.NewLine);

            foreach (System.Data.DataRow TheHostIdRow in TheHostIdRowsFound)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "Host Id " +
                    string.Format("{0,3}", (TheHostIdRow.Field<byte>("HostId")).ToString())
                    );

                System.Diagnostics.Trace.WriteLine("===========");
                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                FinaliseProtocolsForHostId(TheHostIdRow.Field<byte>("HostId"));
            }
        }

        private void FinaliseProtocolsForHostId(byte TheHostId)
        {
            foreach (Constants.LatencyAnalysisProtocol TheProtocol in
                System.Enum.GetValues(typeof(Constants.LatencyAnalysisProtocol)))
            {
                string TheProtocolString = ((Constants.LatencyAnalysisProtocol)TheProtocol).ToString();

                System.Diagnostics.Trace.WriteLine
                    (
                    TheProtocolString +
                    " messages"
                    );

                System.Diagnostics.Trace.WriteLine("------------");
                System.Diagnostics.Trace.Write(System.Environment.NewLine);

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
                    LatencyAnalysisDictionaryEnumerableType
                        TheLatencyValueEntriesFound =
                        from s in TheLatencyValuesDictionary.AsEnumerable()
                        where s.Key.Protocol == TheProtocol &&
                        s.Value.MessageId == TheMessageIdRow.Field<ulong>("MessageId") &&
                        s.Value.TimestampDifferenceCalculated
                        select s;

                    int TheLatencyValuesRowsFoundCount = TheLatencyValueEntriesFound.Count();

                    if (TheLatencyValuesRowsFoundCount > 0)
                    {
                        System.Diagnostics.Trace.WriteLine
                            (
                            "The number of pairs of " +
                            TheProtocolString +
                            " messages with a Message Id of " +
                            (TheMessageIdRow.Field<ulong>("MessageId")).ToString() +
                            " was " +
                            TheLatencyValuesRowsFoundCount.ToString()
                            );

                        FinaliseLatencyValuesForMessageId(TheProtocolString, TheMessageIdRow.Field<ulong>("MessageId"), TheLatencyValueEntriesFound);
                    }
                }
            }
        }

        private void FinaliseLatencyValuesForMessageId(string TheProtocolString, ulong TheMessageId, LatencyAnalysisDictionaryEnumerableType TheLatencyValuesRows)
        {
            CommonHistogram TheHistogram =
                new CommonHistogram
                    (
                    Constants.LatencyAnalysisNumberOfBins,
                    Constants.LatencyAnalysisBestCaseLatency,
                    Constants.LatencyAnalysisWorstCaseLatency
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

            foreach (LatencyAnalysisDictionaryKeyValuePairType TheLatencyValuesRow in TheLatencyValuesRows)
            {
                double TheTimestampDifference = TheLatencyValuesRow.Value.TimestampDifference;

                TheHistogram.AddValue(TheTimestampDifference);

                //Keep a running total to allow for averaging
                ++TheNumberOfTimestampDifferenceInstances;
                TheTotalOfTimestampDifferences += TheTimestampDifference;

                if (TheMinTimestampDifference > TheTimestampDifference)
                {
                    TheMinTimestampDifference = TheTimestampDifference;
                    TheMinTimestampPacketNumber = TheLatencyValuesRow.Value.FirstInstancePacketNumber;
                    TheMinTimestampSequenceNumber = TheLatencyValuesRow.Key.SequenceNumber;
                }

                if (TheMaxTimestampDifference < TheTimestampDifference)
                {
                    TheMaxTimestampDifference = TheTimestampDifference;
                    TheMaxTimestampPacketNumber = TheLatencyValuesRow.Value.FirstInstancePacketNumber;
                    TheMaxTimestampSequenceNumber = TheLatencyValuesRow.Key.SequenceNumber;
                }
            }

            if (TheNumberOfTimestampDifferenceInstances > 0)
            {
                TheAverageTimestampDifference = (TheTotalOfTimestampDifferences / TheNumberOfTimestampDifferenceInstances);

                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                System.Diagnostics.Trace.WriteLine
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

                System.Diagnostics.Trace.WriteLine
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

                System.Diagnostics.Trace.WriteLine
                    (
                    "The average latency for pairs of " +
                    TheProtocolString +
                    " messages with a Message Id of " +
                    TheMessageId.ToString() +
                    " was " +
                    TheAverageTimestampDifference.ToString() +
                    " ms"
                    );

                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                //Output the histogram

                System.Diagnostics.Trace.WriteLine
                    (
                    "The histogram (" +
                    Constants.LatencyAnalysisBinsPerMs.ToString() +
                    " bins per ms) for latency values for " +
                    TheProtocolString +
                    " messages with a Message Id of " +
                    TheMessageId.ToString() +
                    " is:"
                    );

                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                TheHistogram.OutputValues();
            }

            System.Diagnostics.Trace.Write(System.Environment.NewLine);

            if (OutputLatencyAnalysisDebug)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The first and second packet numbers, sequence numbers and latency values for " +
                    TheProtocolString +
                    " messages with a Message Id of " +
                    TheMessageId.ToString() +
                    " are:"
                    );

                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                foreach (LatencyAnalysisDictionaryKeyValuePairType TheLatencyValuesRow in TheLatencyValuesRows)
                {
                    System.Diagnostics.Trace.WriteLine
                        (
                        TheLatencyValuesRow.Value.FirstInstancePacketNumber.ToString() +
                        "\t" +
                        TheLatencyValuesRow.Value.SecondInstancePacketNumber.ToString() +
                        "\t" +
                        TheLatencyValuesRow.Key.SequenceNumber.ToString() +
                        "\t" +
                        TheLatencyValuesRow.Value.TimestampDifference.ToString()
                        );
                }

                System.Diagnostics.Trace.Write(System.Environment.NewLine);
            }
        }
    }
}
