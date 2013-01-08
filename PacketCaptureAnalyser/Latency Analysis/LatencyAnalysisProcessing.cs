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
        private System.Data.DataTable TheLatencyDataTable;
        private System.Data.DataTable TheHostIdDataTable;

        public LatencyAnalysisProcessing()
        {
            //Create a datatable to hold the latency data for message pairings
            TheLatencyDataTable = new System.Data.DataTable();

            //Create a datatable to hold the set of host Ids encountered during the latency analysis
            TheHostIdDataTable = new System.Data.DataTable();
        }

        public void Create()
        {
            //Add the required columns to the datatable to hold the latency data for message pairings
            TheLatencyDataTable.Columns.Add("HostId", typeof(byte));
            TheLatencyDataTable.Columns.Add("SequenceNumber", typeof(ulong));
            TheLatencyDataTable.Columns.Add("FirstInstanceFound", typeof(bool));
            TheLatencyDataTable.Columns.Add("SecondInstanceFound", typeof(bool));
            TheLatencyDataTable.Columns.Add("FirstInstanceTimestamp", typeof(double));
            TheLatencyDataTable.Columns.Add("SecondInstanceTimestamp", typeof(double));
            TheLatencyDataTable.Columns.Add("TimestampDifference", typeof(double));

            //Set a multi-column primary key as only a combination of host Id and the sequence number uniquely identify a message pairing
            //The primary key is needed to allow for use of the Find method against the datatable
            TheLatencyDataTable.PrimaryKey =
                new System.Data.DataColumn[]
            {
                TheLatencyDataTable.Columns["HostId"], 
                TheLatencyDataTable.Columns["SequenceNumber"]
            };

            //Add the required column to the datatable to hold the set of host Ids encountered during the latency analysis
            TheHostIdDataTable.Columns.Add("HostId", typeof(byte));

            //Set the primary key to be the only column
            //The primary key is needed to allow for use of the Find method against the datatable
            TheHostIdDataTable.PrimaryKey = new System.Data.DataColumn[] { TheHostIdDataTable.Columns["HostId"] };
        }

        public void RegisterMessage(byte TheHostId, ulong TheSequenceNumber, double TheTimestamp)
        {
            //Do not process messages where the sequence number is not populated as we would not be able match message pairs using them
            if (TheSequenceNumber == 0)
            {
                return;
            }

            //Add the supplied host Id to the set of encountered during the latency analysis if not already in there

            object[] TheHostIdDataRowFindObject = new object[1];

            TheHostIdDataRowFindObject[0] = TheHostId.ToString(); //Primary key

            System.Data.DataRow TheHostIdDataRowFound = TheHostIdDataTable.Rows.Find(TheHostIdDataRowFindObject);

            if (TheHostIdDataRowFound == null)
            {
                System.Diagnostics.Debug.WriteLine("Found entry for Host Id of {0} so adding it to latency analysis", TheHostId);

                System.Data.DataRow TheHostIdDataRowToAdd = TheHostIdDataTable.NewRow();

                TheHostIdDataRowToAdd["HostId"] = TheHostId;

                TheHostIdDataTable.Rows.Add(TheHostIdDataRowToAdd);
            }

            //Add the suppplied sequence number and timestamp to datatable to hold the latency data for message pairings

            object[] TheLatencyDataRowFindObject = new object[2];

            TheLatencyDataRowFindObject[0] = TheHostId.ToString(); //Primary key (part one)
            TheLatencyDataRowFindObject[1] = TheSequenceNumber.ToString(); //Primary key (part two)

            System.Data.DataRow TheLatencyDataRowFound = TheLatencyDataTable.Rows.Find(TheLatencyDataRowFindObject);

            if (TheLatencyDataRowFound == null)
            {
                //If this is the first message of the pairing then create the row

                System.Data.DataRow TheLatencyDataRowToAdd = TheLatencyDataTable.NewRow();

                TheLatencyDataRowToAdd["HostId"] = TheHostId;
                TheLatencyDataRowToAdd["SequenceNumber"] = TheSequenceNumber;
                TheLatencyDataRowToAdd["FirstInstanceFound"] = true;
                TheLatencyDataRowToAdd["SecondInstanceFound"] = false;
                TheLatencyDataRowToAdd["FirstInstanceTimestamp"] = TheTimestamp;
                TheLatencyDataRowToAdd["SecondInstanceTimestamp"] = 0.0;
                TheLatencyDataRowToAdd["TimestampDifference"] = 0.0;

                TheLatencyDataTable.Rows.Add(TheLatencyDataRowToAdd);
            }
            else
            {
                //If this is the second message of the pairing then update the row and calculate the difference in timestamps i.e. the latency

                if (!(bool)TheLatencyDataRowFound["FirstInstanceFound"])
                {
                    System.Diagnostics.Debug.WriteLine("Found the row for Host Id of {0} and Sequence Number {1} but the FirstInstanceFound flag is not set!!!", TheHostId, TheSequenceNumber);
                    return;
                }

                if ((bool)TheLatencyDataRowFound["SecondInstanceFound"])
                {
                    System.Diagnostics.Debug.WriteLine("Found the row for Host Id of {0} and Sequence Number {1} but the SecondInstanceFound flag is already set!!!", TheHostId, TheSequenceNumber);
                    return;
                }

                if (TheTimestamp > (double)TheLatencyDataRowFound["FirstInstanceTimestamp"])
                {

                    TheLatencyDataRowFound["SecondInstanceFound"] = true;
                    TheLatencyDataRowFound["SecondInstanceTimestamp"] = TheTimestamp;
                    TheLatencyDataRowFound["TimestampDifference"] = (TheTimestamp - (double)TheLatencyDataRowFound["FirstInstanceTimestamp"]) * 1000; //Milliseconds
                }
                else if (TheTimestamp == (double)TheLatencyDataRowFound["FirstInstanceTimestamp"])
                {
                    TheLatencyDataRowFound["SecondInstanceFound"] = true;
                    TheLatencyDataRowFound["SecondInstanceTimestamp"] = 0.0;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Found the row for Host Id of {0} and Sequence Number {1}, but the timestamp of the first message is higher than that of the second message!!!", TheHostId, TheSequenceNumber);
                }
            }
        }

        public void Finalise()
        {
            //Obtain the set host Id to the set of encountered during the latency analysis in ascending order

            var TheHostIdDataRowsFound = from r in TheHostIdDataTable.AsEnumerable()
                                         orderby r.Field<byte>("HostId") ascending
                                         select r;

            //Loop across all the latency data for the message pairings using each of these host Ids in turn

            foreach (System.Data.DataRow TheHostIdDataRow in TheHostIdDataRowsFound)
            {
                ulong MinTimestampSequenceNumber = 0;
                ulong MaxTimestampSequenceNumber = 0;

                double MinTimestampDifference = double.MaxValue;
                double MaxTimestampDifference = double.MinValue;

                var TheLatencyDataRowsFound = from r in TheLatencyDataTable.AsEnumerable()
                                              where r.Field<byte>("HostId") == (byte)TheHostIdDataRow["HostId"]
                                              select r;

                System.Diagnostics.Debug.WriteLine("Host Id {0,3}", (byte)TheHostIdDataRow["HostId"]);
                System.Diagnostics.Debug.WriteLine("===========");
                System.Diagnostics.Debug.WriteLine("The number of message pairs was {0}", TheLatencyDataRowsFound.Count());

                foreach (System.Data.DataRow TheDataRow in TheLatencyDataRowsFound)
                {
                    double TimestampDifference = (double)TheDataRow["TimestampDifference"];

                    if (MinTimestampDifference > TimestampDifference)
                    {
                        MinTimestampDifference = TimestampDifference;
                        MinTimestampSequenceNumber = (ulong)TheDataRow["SequenceNumber"];
                    }

                    if (MaxTimestampDifference < TimestampDifference)
                    {
                        MaxTimestampDifference = TimestampDifference;
                        MaxTimestampSequenceNumber = (ulong)TheDataRow["SequenceNumber"];
                    }
                }

                System.Diagnostics.Debug.WriteLine("The miniumum latency is {0} ms for sequence number {1}", MinTimestampDifference, MinTimestampSequenceNumber);
                System.Diagnostics.Debug.WriteLine("The maxiumum latency is {0} ms for sequence number {1}", MaxTimestampDifference, MaxTimestampSequenceNumber);
                System.Diagnostics.Debug.WriteLine(System.Environment.NewLine);
            }
        }
    }
}
