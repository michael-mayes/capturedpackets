//This is free and unencumbered software released into the public domain.

//Anyone is free to copy, modify, publish, use, compile, sell, or
//distribute this software, either in source code form or as a compiled
//binary, for any purpose, commercial or non-commercial, and by any
//means.

//In jurisdictions that recognize copyright laws, the author or authors
//of this software dedicate any and all copyright interest in the
//software to the public domain. We make this dedication for the benefit
//of the public at large and to the detriment of our heirs and
//successors. We intend this dedication to be an overt act of
//relinquishment in perpetuity of all present and future rights to this
//software under copyright law.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.

//For more information, please refer to <http://unlicense.org/>

namespace AnalysisNamespace
{
    using System.Data; //Required to be able to use AsEnumerable method
    using System.Linq; //Required to be able to use Count method

    class LatencyAnalysisProcessing
    {
        private bool OutputLatencyAnalysisDebug;
        private System.Data.DataTable TheLatencyValuesTable;
        private System.Data.DataTable TheHostIdsTable;
        private System.Data.DataTable TheMessageIdsTable;

        public LatencyAnalysisProcessing(bool OutputLatencyAnalysisDebug)
        {
            this.OutputLatencyAnalysisDebug = OutputLatencyAnalysisDebug;

            //Create a datatable to hold the latency values for message pairings
            TheLatencyValuesTable = new System.Data.DataTable();

            //Create a datatable to hold the set of host Ids encountered during the latency analysis
            TheHostIdsTable = new System.Data.DataTable();

            //Create a datatable to hold the set of message Ids encountered during the latency analysis
            TheMessageIdsTable = new System.Data.DataTable();
        }

        public void Create()
        {
            //Add the required columns to the datatable to hold the latency values for message pairings
            TheLatencyValuesTable.Columns.Add("HostId", typeof(byte));
            TheLatencyValuesTable.Columns.Add("Protocol", typeof(byte));
            TheLatencyValuesTable.Columns.Add("SequenceNumber", typeof(ulong));
            TheLatencyValuesTable.Columns.Add("MessageId", typeof(ulong));
            TheLatencyValuesTable.Columns.Add("FirstInstanceFound", typeof(bool));
            TheLatencyValuesTable.Columns.Add("SecondInstanceFound", typeof(bool));
            TheLatencyValuesTable.Columns.Add("FirstInstancePacketNumber", typeof(ulong));
            TheLatencyValuesTable.Columns.Add("SecondInstancePacketNumber", typeof(ulong));
            TheLatencyValuesTable.Columns.Add("FirstInstanceTimestamp", typeof(double));
            TheLatencyValuesTable.Columns.Add("SecondInstanceTimestamp", typeof(double));
            TheLatencyValuesTable.Columns.Add("TimestampDifference", typeof(double));
            TheLatencyValuesTable.Columns.Add("TimestampDifferenceCalculated", typeof(bool));

            //Set a multi-column primary key as only a combination of host Id, protocol and the sequence number uniquely identify a message pairing
            //The primary key is needed to allow for use of the Find method against the datatable
            TheLatencyValuesTable.PrimaryKey =
                new System.Data.DataColumn[]
            {
                TheLatencyValuesTable.Columns["HostId"],
                TheLatencyValuesTable.Columns["Protocol"],
                TheLatencyValuesTable.Columns["SequenceNumber"]
            };

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

        public void RegisterMessageReceipt(byte TheHostId, LatencyAnalysisConstants.LatencyAnalysisProtocol TheProtocol, ulong TheSequenceNumber, ulong TheMessageId, ulong ThePacketNumber, double TheTimestamp)
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

            byte TheProtocolAsByte = (byte)TheProtocol;

            //Add the supplied sequence number and timestamp to latency values datatable

            object[] TheLatencyValuesRowFindObject = new object[3];

            TheLatencyValuesRowFindObject[0] = TheHostId.ToString(); //Primary key (part one)
            TheLatencyValuesRowFindObject[1] = TheProtocolAsByte.ToString(); //Primary key (part two)
            TheLatencyValuesRowFindObject[2] = TheSequenceNumber.ToString(); //Primary key (part three)

            System.Data.DataRow TheLatencyValuesRowFound = TheLatencyValuesTable.Rows.Find(TheLatencyValuesRowFindObject);

            if (TheLatencyValuesRowFound == null)
            {
                //If this is the first message of the pairing then create the row

                System.Data.DataRow TheLatencyValuesRowToAdd = TheLatencyValuesTable.NewRow();

                TheLatencyValuesRowToAdd["HostId"] = TheHostId;
                TheLatencyValuesRowToAdd["Protocol"] = TheProtocolAsByte;
                TheLatencyValuesRowToAdd["SequenceNumber"] = TheSequenceNumber;
                TheLatencyValuesRowToAdd["MessageId"] = TheMessageId;
                TheLatencyValuesRowToAdd["FirstInstanceFound"] = true;
                TheLatencyValuesRowToAdd["SecondInstanceFound"] = false;
                TheLatencyValuesRowToAdd["FirstInstancePacketNumber"] = ThePacketNumber;
                TheLatencyValuesRowToAdd["SecondInstancePacketNumber"] = 0;
                TheLatencyValuesRowToAdd["FirstInstanceTimestamp"] = TheTimestamp;
                TheLatencyValuesRowToAdd["SecondInstanceTimestamp"] = 0.0;
                TheLatencyValuesRowToAdd["TimestampDifference"] = 0.0;
                TheLatencyValuesRowToAdd["TimestampDifferenceCalculated"] = false;

                TheLatencyValuesTable.Rows.Add(TheLatencyValuesRowToAdd);
            }
            else
            {
                //If this is the second message of the pairing then update the row and calculate the difference in timestamps i.e. the latency

                if (!(bool)TheLatencyValuesRowFound["FirstInstanceFound"])
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

                if ((bool)TheLatencyValuesRowFound["SecondInstanceFound"])
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

                TheLatencyValuesRowFound["SecondInstanceFound"] = true;
                TheLatencyValuesRowFound["SecondInstancePacketNumber"] = ThePacketNumber;

                if (TheTimestamp > (double)TheLatencyValuesRowFound["FirstInstanceTimestamp"])
                {

                    TheLatencyValuesRowFound["SecondInstanceTimestamp"] = TheTimestamp;
                    TheLatencyValuesRowFound["TimestampDifference"] = (TheTimestamp - (double)TheLatencyValuesRowFound["FirstInstanceTimestamp"]) * 1000.0; //Milliseconds
                    TheLatencyValuesRowFound["TimestampDifferenceCalculated"] = true;
                }
                else if (TheTimestamp == (double)TheLatencyValuesRowFound["FirstInstanceTimestamp"])
                {
                    TheLatencyValuesRowFound["SecondInstanceTimestamp"] = 0.0;
                    TheLatencyValuesRowFound["TimestampDifference"] = 0.0;
                    TheLatencyValuesRowFound["TimestampDifferenceCalculated"] = true;
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

                    TheLatencyValuesRowFound["TimestampDifferenceCalculated"] = false;
                }

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
                    string.Format("{0,3}", ((byte)TheHostIdRow["HostId"]).ToString())
                    );

                System.Diagnostics.Trace.WriteLine("===========");
                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                FinaliseProtocolsForHostId((byte)TheHostIdRow["HostId"]);
            }
        }

        private void FinaliseProtocolsForHostId(byte TheHostId)
        {
            foreach (byte TheProtocol in System.Enum.GetValues(typeof(LatencyAnalysisConstants.LatencyAnalysisProtocol)))
            {
                string TheProtocolString = ((LatencyAnalysisConstants.LatencyAnalysisProtocol)TheProtocol).ToString();

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
                    EnumerableRowCollection<System.Data.DataRow>
                        TheLatencyValuesRowsFound =
                        from r in TheLatencyValuesTable.AsEnumerable()
                        where r.Field<byte>("Protocol") == (byte)TheProtocol &&
                        r.Field<ulong>("MessageId") == (ulong)TheMessageIdRow["MessageId"] &&
                        r.Field<bool>("TimestampDifferenceCalculated")
                        select r;

                    int TheLatencyValuesRowsFoundCount = TheLatencyValuesRowsFound.Count();

                    if (TheLatencyValuesRowsFoundCount > 0)
                    {
                        System.Diagnostics.Trace.WriteLine
                            (
                            "The number of pairs of " +
                            TheProtocolString +
                            " messages with a Message Id of " +
                            ((ulong)TheMessageIdRow["MessageId"]).ToString() +
                            " was " +
                            TheLatencyValuesRowsFoundCount.ToString()
                            );

                        FinaliseLatencyValuesForMessageId(TheProtocolString, (ulong)TheMessageIdRow["MessageId"], TheLatencyValuesRowsFound);
                    }
                }
            }
        }

        private void FinaliseLatencyValuesForMessageId(string TheProtocolString, ulong TheMessageId, EnumerableRowCollection<System.Data.DataRow> TheLatencyValuesRows)
        {
            CommonAnalysisHistogram TheHistogram =
                new CommonAnalysisHistogram
                    (LatencyAnalysisConstants.LatencyAnalysisNumberOfBins,
                    LatencyAnalysisConstants.LatencyAnalysisBestCaseLatency,
                    LatencyAnalysisConstants.LatencyAnalysisWorstCaseLatency);

            ulong TheMinTimestampPacketNumber = 0;
            ulong TheMaxTimestampPacketNumber = 0;

            ulong TheMinTimestampSequenceNumber = 0;
            ulong TheMaxTimestampSequenceNumber = 0;

            double TheMinTimestampDifference = double.MaxValue;
            double TheMaxTimestampDifference = double.MinValue;

            ulong TheNumberOfTimestampDifferenceInstances = 0;
            double TheTotalOfTimestampDifferences = 0;
            double TheAverageTimestampDifference = 0;

            foreach (System.Data.DataRow TheLatencyValuesRow in TheLatencyValuesRows)
            {
                double TheTimestampDifference = (double)TheLatencyValuesRow["TimestampDifference"];

                TheHistogram.AddValue(TheTimestampDifference);

                //Keep a running total to allow for averaging
                ++TheNumberOfTimestampDifferenceInstances;
                TheTotalOfTimestampDifferences += TheTimestampDifference;

                if (TheMinTimestampDifference > TheTimestampDifference)
                {
                    TheMinTimestampDifference = TheTimestampDifference;
                    TheMinTimestampPacketNumber = (ulong)TheLatencyValuesRow["FirstInstancePacketNumber"];
                    TheMinTimestampSequenceNumber = (ulong)TheLatencyValuesRow["SequenceNumber"];
                }

                if (TheMaxTimestampDifference < TheTimestampDifference)
                {
                    TheMaxTimestampDifference = TheTimestampDifference;
                    TheMaxTimestampPacketNumber = (ulong)TheLatencyValuesRow["FirstInstancePacketNumber"];
                    TheMaxTimestampSequenceNumber = (ulong)TheLatencyValuesRow["SequenceNumber"];
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
                    LatencyAnalysisConstants.LatencyAnalysisBinsPerMs.ToString() +
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

                foreach (System.Data.DataRow TheLatencyValuesRow in TheLatencyValuesRows)
                {
                    System.Diagnostics.Trace.WriteLine
                        (
                        ((ulong)TheLatencyValuesRow["FirstInstancePacketNumber"]).ToString() +
                        "\t" +
                        ((ulong)TheLatencyValuesRow["SecondInstancePacketNumber"]).ToString() +
                        "\t" +
                        ((ulong)TheLatencyValuesRow["SequenceNumber"]).ToString() +
                        "\t" +
                        ((double)TheLatencyValuesRow["TimestampDifference"]).ToString()
                        );
                }

                System.Diagnostics.Trace.Write(System.Environment.NewLine);
            }
        }
    }
}
