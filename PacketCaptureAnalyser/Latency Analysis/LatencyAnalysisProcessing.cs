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

namespace LatencyAnalysisNamespace
{
    using System.Data; //Required to be able to use AsEnumerable method
    using System.Linq; //Required to be able to use Count method

    class LatencyAnalysisProcessing
    {
        private System.Data.DataTable TheLatencyValuesTable;
        private System.Data.DataTable TheHostIdTable;

        public LatencyAnalysisProcessing()
        {
            //Create a datatable to hold the latency values for message pairings
            TheLatencyValuesTable = new System.Data.DataTable();

            //Create a datatable to hold the set of host Ids encountered during the latency analysis
            TheHostIdTable = new System.Data.DataTable();
        }

        public void Create()
        {
            //Add the required columns to the datatable to hold the latency values for message pairings
            TheLatencyValuesTable.Columns.Add("HostId", typeof(byte));
            TheLatencyValuesTable.Columns.Add("Protocol", typeof(byte));
            TheLatencyValuesTable.Columns.Add("SequenceNumber", typeof(ulong));
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
        }

        public void RegisterMessage(byte TheHostId, LatencyAnalysisConstants.LatencyAnalysisProtocol TheProtocol, ulong TheSequenceNumber, double TheTimestamp)
        {
            //Do not process messages where the sequence number is not populated as we would not be able match message pairs using them
            if (TheSequenceNumber == 0)
            {
                return;
            }

            byte TheProtocolAsByte = (byte)TheProtocol;

            //Add the supplied host Id to the set of encountered during the latency analysis if not already in there

            object[] TheHostIdRowFindObject = new object[1];

            TheHostIdRowFindObject[0] = TheHostId.ToString(); //Primary key

            System.Data.DataRow TheHostIdDataRowFound = TheHostIdTable.Rows.Find(TheHostIdRowFindObject);

            if (TheHostIdDataRowFound == null)
            {
                System.Diagnostics.Debug.WriteLine("Found entry for Host Id of {0} so adding it to latency analysis", TheHostId);

                System.Data.DataRow TheHostIdRowToAdd = TheHostIdTable.NewRow();

                TheHostIdRowToAdd["HostId"] = TheHostId;

                TheHostIdTable.Rows.Add(TheHostIdRowToAdd);
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
            //Obtain the set host Id to the set of encountered during the latency analysis in ascending order

            EnumerableRowCollection<System.Data.DataRow>
                TheHostIdRowsFound =
                from r in TheHostIdTable.AsEnumerable()
                orderby r.Field<byte>("HostId") ascending
                select r;

            //Loop across all the latency values for the message pairings using each of these host Ids in turn

            System.Diagnostics.Debug.Write(System.Environment.NewLine);

            foreach (System.Data.DataRow TheHostIdRow in TheHostIdRowsFound)
            {
                System.Diagnostics.Debug.WriteLine("Host Id {0,3}", (byte)TheHostIdRow["HostId"]);
                System.Diagnostics.Debug.WriteLine("===========");
                System.Diagnostics.Debug.Write(System.Environment.NewLine);

                //Finalise the latency analysis for the pairings of TCP messages for this host Id
                FinaliseProtocol((byte)TheHostIdRow["HostId"], LatencyAnalysisConstants.LatencyAnalysisProtocol.TCP);

                //Finalise the latency analysis for the pairings of UDP messages for this host Id
                FinaliseProtocol((byte)TheHostIdRow["HostId"], LatencyAnalysisConstants.LatencyAnalysisProtocol.UDP);
            }
        }

        private void FinaliseProtocol(byte TheHostId, LatencyAnalysisConstants.LatencyAnalysisProtocol TheProtocol)
        {
            string TheProtocolString;

            switch (TheProtocol)
            {
                case LatencyAnalysisConstants.LatencyAnalysisProtocol.TCP:
                    {
                        TheProtocolString = "TCP";
                        break;
                    }

                case LatencyAnalysisConstants.LatencyAnalysisProtocol.UDP:
                    {
                        TheProtocolString = "UDP";
                        break;
                    }

                default:
                    {
                        TheProtocolString = "<Unknown Protocol>";
                        break;
                    }
            }

            System.Diagnostics.Debug.WriteLine(TheProtocolString + " messages");
            System.Diagnostics.Debug.WriteLine("------------");
            System.Diagnostics.Debug.Write(System.Environment.NewLine);

            EnumerableRowCollection<System.Data.DataRow>
                TheLatencyValuesRowsFound =
                from r in TheLatencyValuesTable.AsEnumerable()
                where r.Field<byte>("HostId") == TheHostId &&
                r.Field<byte>("Protocol") == (byte)TheProtocol &&
                r.Field<bool>("TimestampDifferenceCalculated")
                select r;

            int TheLatencyDataRowsFoundCount = TheLatencyValuesRowsFound.Count();

            System.Diagnostics.Debug.WriteLine("The number of " + TheProtocolString + " message pairs was {0}", TheLatencyDataRowsFoundCount);

            if (TheLatencyDataRowsFoundCount > 0)
            {
                FinaliseRows(TheProtocolString, TheLatencyValuesRowsFound);
            }

            System.Diagnostics.Debug.Write(System.Environment.NewLine);
        }

        private void FinaliseRows(string TheProtocolString, EnumerableRowCollection<System.Data.DataRow> TheLatencyValueRows)
        {
            LatencyAnalysisHistogram TheHistogram = new LatencyAnalysisHistogram(LatencyAnalysisConstants.LatencyAnalysisNumberOfBins, 0.0, LatencyAnalysisConstants.LatencyAnalysisWorstCaseLatency);

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
            System.Diagnostics.Debug.WriteLine("The minimum latency for " + TheProtocolString + " messages is {0} ms for sequence number {1}", TheMinTimestampDifference, TheMinTimestampSequenceNumber);
            System.Diagnostics.Debug.WriteLine("The maximum latency for " + TheProtocolString + " messages is {0} ms for sequence number {1}", TheMaxTimestampDifference, TheMaxTimestampSequenceNumber);
            System.Diagnostics.Debug.Write(System.Environment.NewLine);

            //Output the values for the histogram

            System.Diagnostics.Debug.WriteLine("Histogram for latency for " + TheProtocolString + " messages:");
            System.Diagnostics.Debug.Write(System.Environment.NewLine);

            TheHistogram.OutputValues(TheMaxTimestampDifference);
        }
    }
}
