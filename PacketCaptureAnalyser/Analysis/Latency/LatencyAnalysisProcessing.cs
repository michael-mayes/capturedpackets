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
        private System.Data.DataTable TheLatencyValuesTable;
        private System.Data.DataTable TheHostIdTable;
        private System.Data.DataTable TheMessageIdTable;

        public LatencyAnalysisProcessing()
        {
            //Create a datatable to hold the latency values for message pairings
            TheLatencyValuesTable = new System.Data.DataTable();

            //Create a datatable to hold the set of host Ids encountered during the latency analysis
            TheHostIdTable = new System.Data.DataTable();

            //Create a datatable to hold the set of message Ids encountered during the latency analysis
            TheMessageIdTable = new System.Data.DataTable();
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
            TheHostIdTable.Columns.Add("HostId", typeof(byte));

            //Set the primary key to be the only column
            //The primary key is needed to allow for use of the Find method against the datatable
            TheHostIdTable.PrimaryKey = new System.Data.DataColumn[] { TheHostIdTable.Columns["HostId"] };

            //Add the required column to the datatable to hold the set of message Ids encountered during the latency analysis
            TheMessageIdTable.Columns.Add("MessageId", typeof(ulong));
            TheMessageIdTable.Columns.Add("HostId", typeof(byte));

            //Set a multi-column primary key
            //The primary key is needed to allow for use of the Find method against the datatable
            TheMessageIdTable.PrimaryKey =
                new System.Data.DataColumn[]
                {
                    TheMessageIdTable.Columns["MessageId"],
                    TheMessageIdTable.Columns["HostId"]
                };
        }

        public void RegisterMessageReceipt(byte TheHostId, LatencyAnalysisConstants.LatencyAnalysisProtocol TheProtocol, ulong TheSequenceNumber, ulong TheMessageId, double TheTimestamp)
        {
            //Do not process messages where the sequence number is not populated as we would not be able match message pairs using them
            if (TheSequenceNumber == 0)
            {
                return;
            }

            //Do not process messages where the message Id is not populated as we would not be able match message pairs using them
            if (TheMessageId == 0)
            {
                return;
            }

            byte TheProtocolAsByte = (byte)TheProtocol;

            //Add the supplied host Id to the set of those encountered during the latency analysis if not already in there

            object[] TheHostIdRowFindObject = new object[1];

            TheHostIdRowFindObject[0] = TheHostId.ToString(); //Primary key

            System.Data.DataRow TheHostIdDataRowFound = TheHostIdTable.Rows.Find(TheHostIdRowFindObject);

            if (TheHostIdDataRowFound == null)
            {
                System.Diagnostics.Debug.WriteLine("Found a message with a Host Id of value {0,3} - adding that Host Id to the latency analysis", TheHostId);

                System.Data.DataRow TheHostIdRowToAdd = TheHostIdTable.NewRow();

                TheHostIdRowToAdd["HostId"] = TheHostId;

                TheHostIdTable.Rows.Add(TheHostIdRowToAdd);
            }

            //Add the supplied message Id to the set of those encountered during the latency analysis if not already in there

            object[] TheMessageIdRowFindObject = new object[2];

            TheMessageIdRowFindObject[0] = TheMessageId.ToString(); //Primary key (part one)
            TheMessageIdRowFindObject[1] = TheHostId.ToString(); //Primary key (part two)

            System.Data.DataRow TheMessageIdDataRowFound = TheMessageIdTable.Rows.Find(TheMessageIdRowFindObject);

            if (TheMessageIdDataRowFound == null)
            {
                System.Diagnostics.Debug.WriteLine("Found a message with a Message Id of value {0,5} for Host Id {1,3} - adding that Message Id to the latency analysis", TheMessageId, TheHostId);

                System.Data.DataRow TheMessageIdRowToAdd = TheMessageIdTable.NewRow();

                TheMessageIdRowToAdd["MessageId"] = TheMessageId;
                TheMessageIdRowToAdd["HostId"] = TheHostId;

                TheMessageIdTable.Rows.Add(TheMessageIdRowToAdd);
            }

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
                    System.Diagnostics.Debug.WriteLine("Found the row for Host Id of {0} and Sequence Number {1} but the FirstInstanceFound flag is not set!!!", TheHostId, TheSequenceNumber);
                    return;
                }

                if ((bool)TheLatencyValuesRowFound["SecondInstanceFound"])
                {
                    System.Diagnostics.Debug.WriteLine("Found the row for Host Id of {0} and Sequence Number {1} but the SecondInstanceFound flag is already set!!!", TheHostId, TheSequenceNumber);
                    return;
                }

                if (TheTimestamp > (double)TheLatencyValuesRowFound["FirstInstanceTimestamp"])
                {

                    TheLatencyValuesRowFound["SecondInstanceFound"] = true;
                    TheLatencyValuesRowFound["SecondInstanceTimestamp"] = TheTimestamp;
                    TheLatencyValuesRowFound["TimestampDifference"] = (TheTimestamp - (double)TheLatencyValuesRowFound["FirstInstanceTimestamp"]) * 1000; //Milliseconds
                    TheLatencyValuesRowFound["TimestampDifferenceCalculated"] = true;
                }
                else if (TheTimestamp == (double)TheLatencyValuesRowFound["FirstInstanceTimestamp"])
                {
                    TheLatencyValuesRowFound["SecondInstanceFound"] = true;
                    TheLatencyValuesRowFound["SecondInstanceTimestamp"] = 0.0;
                    TheLatencyValuesRowFound["TimestampDifference"] = 0.0;
                    TheLatencyValuesRowFound["TimestampDifferenceCalculated"] = true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Found the row for Host Id of {0} and Sequence Number {1}, but the timestamp of the first message is higher than that of the second message!!!", TheHostId, TheSequenceNumber);
                    TheLatencyValuesRowFound["SecondInstanceFound"] = true;
                    TheLatencyValuesRowFound["TimestampDifferenceCalculated"] = false;
                }
            }
        }

        public void Finalise()
        {
            //Obtain the set of host Ids encountered during the latency analysis in ascending order

            EnumerableRowCollection<System.Data.DataRow>
                TheHostIdRowsFound =
                from r in TheHostIdTable.AsEnumerable()
                orderby r.Field<byte>("HostId") ascending
                select r;

            //Loop across all the latency values for the message pairings using each of these host Ids in turn

            foreach (System.Data.DataRow TheHostIdRow in TheHostIdRowsFound)
            {
                System.Diagnostics.Debug.Write(System.Environment.NewLine);

                System.Diagnostics.Debug.WriteLine("Host Id {0,3}", (byte)TheHostIdRow["HostId"]);
                System.Diagnostics.Debug.WriteLine("===========");
                System.Diagnostics.Debug.Write(System.Environment.NewLine);

                //Finalise the latency analysis for the pairings of messages of each supported protocol for this host Id
                FinaliseProtocols((byte)TheHostIdRow["HostId"]);
            }
        }

        private void FinaliseProtocols(byte TheHostId)
        {
            foreach (byte TheProtocol in System.Enum.GetValues(typeof(LatencyAnalysisConstants.LatencyAnalysisProtocol)))
            {
                string TheProtocolString = ((LatencyAnalysisConstants.LatencyAnalysisProtocol)TheProtocol).ToString();

                System.Diagnostics.Debug.WriteLine(TheProtocolString + " messages");
                System.Diagnostics.Debug.WriteLine("------------");
                System.Diagnostics.Debug.Write(System.Environment.NewLine);

                //Obtain the set of message Ids encountered during the latency analysis in ascending order

                EnumerableRowCollection<System.Data.DataRow>
                    TheMessageIdRowsFound =
                    from r in TheMessageIdTable.AsEnumerable()
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

                    int TheLatencyDataRowsFoundCount = TheLatencyValuesRowsFound.Count();

                    if (TheLatencyDataRowsFoundCount > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("The number of pairs of " + TheProtocolString + " messages with a Message Id of {0,5} was {1}", (ulong)TheMessageIdRow["MessageId"], TheLatencyDataRowsFoundCount);

                        FinaliseRows(TheProtocolString, (ulong)TheMessageIdRow["MessageId"], TheLatencyValuesRowsFound);
                    }
                }
            }
        }

        private void FinaliseRows(string TheProtocolString, ulong TheMessageId, EnumerableRowCollection<System.Data.DataRow> TheLatencyValueRows)
        {
            CommonAnalysisHistogram TheHistogram = new CommonAnalysisHistogram
                (LatencyAnalysisConstants.LatencyAnalysisNumberOfBins,
                LatencyAnalysisConstants.LatencyAnalysisBestCaseLatency,
                LatencyAnalysisConstants.LatencyAnalysisWorstCaseLatency);

            ulong TheMinTimestampSequenceNumber = 0;
            ulong TheMaxTimestampSequenceNumber = 0;

            double TheMinTimestampDifference = double.MaxValue;
            double TheMaxTimestampDifference = double.MinValue;

            foreach (System.Data.DataRow TheDataRow in TheLatencyValueRows)
            {
                double TheTimestampDifference = (double)TheDataRow["TimestampDifference"];

                TheHistogram.AddValue(TheTimestampDifference);

                if (TheMinTimestampDifference > TheTimestampDifference)
                {
                    TheMinTimestampDifference = TheTimestampDifference;
                    TheMinTimestampSequenceNumber = (ulong)TheDataRow["SequenceNumber"];
                }

                if (TheMaxTimestampDifference < TheTimestampDifference)
                {
                    TheMaxTimestampDifference = TheTimestampDifference;
                    TheMaxTimestampSequenceNumber = (ulong)TheDataRow["SequenceNumber"];
                }
            }

            System.Diagnostics.Debug.Write(System.Environment.NewLine);
            System.Diagnostics.Debug.WriteLine("The minimum latency for pairs of " + TheProtocolString + " messages with a Message Id of {0,5} was {1} ms for sequence number {2}", TheMessageId, TheMinTimestampDifference, TheMinTimestampSequenceNumber);
            System.Diagnostics.Debug.WriteLine("The maximum latency for pairs of " + TheProtocolString + " messages with a Message Id of {0,5} was {1} ms for sequence number {2}", TheMessageId, TheMaxTimestampDifference, TheMaxTimestampSequenceNumber);
            System.Diagnostics.Debug.Write(System.Environment.NewLine);

            //Output the values for the histogram

            System.Diagnostics.Debug.WriteLine("The histogram for latency values for " + TheProtocolString + " messages with a Message Id of {0,5} is:", TheMessageId);
            System.Diagnostics.Debug.Write(System.Environment.NewLine);

            TheHistogram.OutputValues();

            System.Diagnostics.Debug.Write(System.Environment.NewLine);
        }
    }
}
