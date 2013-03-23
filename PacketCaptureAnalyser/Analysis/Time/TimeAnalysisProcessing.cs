﻿//This is free and unencumbered software released into the public domain.

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

    class TimeAnalysisProcessing
    {
        private System.Data.DataTable TheTimeValuesTable;
        private System.Data.DataTable TheHostIdsTable;

        public TimeAnalysisProcessing()
        {
            //Create a datatable to hold the timestamp and time values for time-supplying messages
            TheTimeValuesTable = new System.Data.DataTable();

            //Create a datatable to hold the set of host Ids encountered during the time analysis
            TheHostIdsTable = new System.Data.DataTable();
        }

        public void Create()
        {
            //Add the required columns to the datatable to hold the timestamp and time values for time-supplying messages
            TheTimeValuesTable.Columns.Add("HostId", typeof(byte));
            TheTimeValuesTable.Columns.Add("PacketNumber", typeof(ulong));
            TheTimeValuesTable.Columns.Add("Timestamp", typeof(double));
            TheTimeValuesTable.Columns.Add("Time", typeof(double));
            TheTimeValuesTable.Columns.Add("Processed", typeof(bool));

            //Add the required column to the datatable to hold the set of host Ids encountered during the time analysis
            TheHostIdsTable.Columns.Add("HostId", typeof(byte));

            //Set the primary key to be the only column
            //The primary key is needed to allow for use of the Find method against the datatable
            TheHostIdsTable.PrimaryKey =
                new System.Data.DataColumn[]
                {
                    TheHostIdsTable.Columns["HostId"]
                };
        }

        public void RegisterTimeMessageReceipt(byte TheHostId, ulong ThePacketNumber, double TheTimestamp, double TheTime)
        {
            //Add the supplied host Id to the set of those encountered during the time analysis if not already in there
            RegisterEncounteredHostId(TheHostId);

            //Add the supplied timestamp and time to the datatable

            System.Data.DataRow TheTimeValuesRowToAdd = TheTimeValuesTable.NewRow();

            TheTimeValuesRowToAdd["HostId"] = TheHostId;
            TheTimeValuesRowToAdd["PacketNumber"] = ThePacketNumber;
            TheTimeValuesRowToAdd["Timestamp"] = TheTimestamp;
            TheTimeValuesRowToAdd["Time"] = TheTime;
            TheTimeValuesRowToAdd["Processed"] = false;

            TheTimeValuesTable.Rows.Add(TheTimeValuesRowToAdd);
        }

        //Add the supplied host Id to the set of those encountered during the time analysis if not already in there
        private void RegisterEncounteredHostId(byte TheHostId)
        {
            object[] TheHostIdRowFindObject = new object[1];

            TheHostIdRowFindObject[0] = TheHostId.ToString(); //Primary key

            System.Data.DataRow TheHostIdDataRowFound = TheHostIdsTable.Rows.Find(TheHostIdRowFindObject);

            if (TheHostIdDataRowFound == null)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "Found a time-supplying message for a Host Id " +
                    string.Format("{0,3}", TheHostId) +
                    " - adding this Host Id to the time analysis"
                    );

                System.Data.DataRow TheHostIdRowToAdd = TheHostIdsTable.NewRow();

                TheHostIdRowToAdd["HostId"] = TheHostId;

                TheHostIdsTable.Rows.Add(TheHostIdRowToAdd);
            }
        }

        public void Finalise()
        {
            //Obtain the set of host Ids encountered during the time analysis in ascending order

            EnumerableRowCollection<System.Data.DataRow>
                TheHostIdRowsFound =
                from r in TheHostIdsTable.AsEnumerable()
                orderby r.Field<byte>("HostId") ascending
                select r;

            //Loop across all the time values for each of these host Ids in turn

            System.Diagnostics.Trace.Write(System.Environment.NewLine);
            System.Diagnostics.Trace.WriteLine("===================");
            System.Diagnostics.Trace.WriteLine("== Time Analysis ==");
            System.Diagnostics.Trace.WriteLine("===================");
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

                FinaliseTimeValuesForHostId((byte)TheHostIdRow["HostId"]);
            }
        }

        private void FinaliseTimeValuesForHostId(byte TheHostId)
        {
            CommonAnalysisHistogram TheTimestampHistogram =
                new CommonAnalysisHistogram
                    (TimeAnalysisConstants.TimeAnalysisTimestampNumberOfBins,
                    TimeAnalysisConstants.TimeAnalysisMaxNegativeDifference,
                    TimeAnalysisConstants.TimeAnalysisMaxPositiveDifference);

            CommonAnalysisHistogram TheTimeHistogram =
                new CommonAnalysisHistogram
                    (TimeAnalysisConstants.TimeAnalysisTimeNumberOfBins,
                    TimeAnalysisConstants.TimeAnalysisMaxNegativeDifference,
                    TimeAnalysisConstants.TimeAnalysisMaxPositiveDifference);

            ulong TheMinTimestampDifferencePacketNumber = 0;
            ulong TheMaxTimestampDifferencePacketNumber = 0;

            ulong TheMinTimeDifferencePacketNumber = 0;
            ulong TheMaxTimeDifferencePacketNumber = 0;

            double TheMinTimestampDifference = double.MaxValue;
            double TheMaxTimestampDifference = double.MinValue;

            double TheMinTimeDifference = double.MaxValue;
            double TheMaxTimeDifference = double.MinValue;

            double TheLastTimestamp = 0.0;
            double TheLastTime = 0.0;

            ulong TheNumberOfTimestampDifferenceInstances = 0;
            double TheTotalOfTimestampDifferences = 0;
            double TheAverageTimestampDifference = 0;

            ulong TheNumberOfTimeDifferenceInstances = 0;
            double TheTotalOfTimeDifferences = 0;
            double TheAverageTimeDifference = 0;

            bool TheFirstRowProcessed = false;

            EnumerableRowCollection<System.Data.DataRow>
                TheTimeValuesRowsFound =
                from r in TheTimeValuesTable.AsEnumerable()
                where r.Field<byte>("HostId") == TheHostId
                select r;

            foreach (System.Data.DataRow TheTimeValuesRow in TheTimeValuesRowsFound)
            {
                //Do not calculate the differences in timestamp and time for first row - just record values and move on to second row
                if (!TheFirstRowProcessed)
                {
                    TheLastTimestamp = (double)TheTimeValuesRow["Timestamp"];
                    TheLastTime = (double)TheTimeValuesRow["Time"];

                    //The first row is always marked as processed
                    TheTimeValuesRow["Processed"] = true;

                    TheFirstRowProcessed = true;

                    continue;
                }

                //The timestamp
                {
                    double TheAbsoluteTimestampDifference = System.Math.Abs(((double)TheTimeValuesRow["Timestamp"] - TheLastTimestamp) * 1000.0);
                    double TheTimestampDifference = (((double)TheTimeValuesRow["Timestamp"] - TheLastTimestamp) * 1000.0) - TimeAnalysisConstants.TimeAnalysisExpectedDifference; //Milliseconds;

                    if (TheAbsoluteTimestampDifference > TimeAnalysisConstants.TimeAnalysisMinTimestampDifference)
                    {
                        //Only those time messages in the chosen range will be marked as processed
                        //This should prevent the processing of duplicates of a time message (e.g. if port mirroring results in two copies of the time message)
                        TheTimeValuesRow["Processed"] = true;

                        //Keep a running total to allow for averaging
                        ++TheNumberOfTimestampDifferenceInstances;
                        TheTotalOfTimestampDifferences += TheTimestampDifference;

                        TheTimestampHistogram.AddValue(TheTimestampDifference);

                        if (TheMinTimestampDifference > TheTimestampDifference)
                        {
                            TheMinTimestampDifference = TheTimestampDifference;
                            TheMinTimestampDifferencePacketNumber = (ulong)TheTimeValuesRow["PacketNumber"];
                        }

                        if (TheMaxTimestampDifference < TheTimestampDifference)
                        {
                            TheMaxTimestampDifference = TheTimestampDifference;
                            TheMaxTimestampDifferencePacketNumber = (ulong)TheTimeValuesRow["PacketNumber"];
                        }

                        TheLastTimestamp = (double)TheTimeValuesRow["Timestamp"];

                        //The time

                        double TheTimeDifference = (((double)TheTimeValuesRow["Time"] - TheLastTime) * 1000.0) - TimeAnalysisConstants.TimeAnalysisExpectedDifference; //Milliseconds;

                        ++TheNumberOfTimeDifferenceInstances;
                        TheTotalOfTimeDifferences += TheTimeDifference;

                        TheTimeHistogram.AddValue(TheTimeDifference);

                        if (TheMinTimeDifference > TheTimeDifference)
                        {
                            TheMinTimeDifference = TheTimeDifference;
                            TheMinTimeDifferencePacketNumber = (ulong)TheTimeValuesRow["PacketNumber"];
                        }

                        if (TheMaxTimeDifference < TheTimeDifference)
                        {
                            TheMaxTimeDifference = TheTimeDifference;
                            TheMaxTimeDifferencePacketNumber = (ulong)TheTimeValuesRow["PacketNumber"];
                        }

                        TheLastTime = (double)TheTimeValuesRow["Time"];
                    }
                }
            }

            if (TheNumberOfTimestampDifferenceInstances > 0)
            {
                TheAverageTimestampDifference = (TheTotalOfTimestampDifferences / TheNumberOfTimestampDifferenceInstances);
                TheAverageTimeDifference = (TheTotalOfTimeDifferences / TheNumberOfTimeDifferenceInstances);

                System.Diagnostics.Trace.WriteLine
                    (
                    "The number of time messages was " +
                    TheNumberOfTimestampDifferenceInstances.ToString()
                    );

                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                System.Diagnostics.Trace.WriteLine
                    (
                    "The minimum timestamp difference was " +
                    TheMinTimestampDifference.ToString() +
                    " ms for packet number " +
                    TheMinTimestampDifferencePacketNumber.ToString()
                    );

                System.Diagnostics.Trace.WriteLine
                    (
                    "The maximum timestamp difference was " +
                    TheMaxTimestampDifference.ToString() +
                    " ms for packet number " +
                    TheMaxTimestampDifferencePacketNumber.ToString()
                    );

                System.Diagnostics.Trace.WriteLine
                    (
                    "The average timestamp difference was " +
                    TheAverageTimestampDifference.ToString() +
                    " ms"
                    );

                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                //Output the histogram

                System.Diagnostics.Trace.WriteLine
                    (
                    "The histogram (" +
                    TimeAnalysisConstants.TimeAnalysisTimestampBinsPerMs.ToString() +
                    " bins per ms) for timestamp values is:"
                    );

                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                TheTimestampHistogram.OutputValues();

                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                System.Diagnostics.Trace.WriteLine
                    (
                    "The minimum time difference was " +
                    TheMinTimeDifference.ToString() +
                    " ms for packet number " +
                    TheMinTimeDifferencePacketNumber.ToString()
                    );

                System.Diagnostics.Trace.WriteLine
                    (
                    "The maximum time difference was " +
                    TheMaxTimeDifference.ToString() +
                    " ms for packet number " +
                    TheMaxTimeDifferencePacketNumber.ToString()
                    );

                System.Diagnostics.Trace.WriteLine
                    (
                    "The average time difference was " +
                    TheAverageTimeDifference.ToString() +
                    " ms"
                    );

                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                //Output the histogram

                System.Diagnostics.Trace.WriteLine
                    (
                    "The histogram (" +
                    TimeAnalysisConstants.TimeAnalysisTimeBinsPerMs.ToString() +
                    " bins per ms) for time values is:"
                    );

                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                TheTimeHistogram.OutputValues();
            }

            System.Diagnostics.Trace.Write(System.Environment.NewLine);

            //
            //Restore the following lines if you want the timestamps and times output to the file
            //

            //System.Diagnostics.Trace.WriteLine
            //    (
            //    "The timestamps and times for the time messages are:"
            //    );

            //System.Diagnostics.Trace.Write(System.Environment.NewLine);

            //foreach (System.Data.DataRow TheTimeValuesRow in TheTimeValuesRowsFound)
            //{
            //    if ((bool)TheTimeValuesRow["Processed"])
            //    {
            //        System.Diagnostics.Trace.WriteLine
            //            (
            //            ((double)TheTimeValuesRow["Timestamp"]).ToString() +
            //            "\t" +
            //            ((double)TheTimeValuesRow["Time"]).ToString()
            //            );
            //    }
            //}

            //System.Diagnostics.Trace.Write(System.Environment.NewLine);
        }
    }
}