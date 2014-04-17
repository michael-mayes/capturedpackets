//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$Id$
//$Revision$
//$Date$
//$Author$

namespace Analysis.TimeAnalysis
{
    using System.Data; //Required to be able to use AsEnumerable method
    using System.Linq; //Required to be able to use Count method

    //This class will implement the Disposable class so as to be able to clean up after the datatables it creates which themselves implement the Disposable class
    class Processing : System.IDisposable
    {
        private Analysis.DebugInformation TheDebugInformation;

        private bool OutputDebug;

        private string SelectedPacketCaptureFile;

        private System.Data.DataTable TheTimeValuesTable;
        private System.Data.DataTable TheHostIdsTable;

        public Processing(Analysis.DebugInformation TheDebugInformation, bool OutputDebug, string SelectedPacketCaptureFile)
        {
            this.TheDebugInformation = TheDebugInformation;

            this.OutputDebug = OutputDebug;

            this.SelectedPacketCaptureFile = SelectedPacketCaptureFile;

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

        protected virtual void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                //Dispose any resources allocated to the datatables if instructed
                TheTimeValuesTable.Dispose();
                TheHostIdsTable.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);

            System.GC.SuppressFinalize(this);
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
                TheDebugInformation.WriteInformationEvent
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

            TheDebugInformation.WriteBlankLine();
            TheDebugInformation.WriteTextLine("===================");
            TheDebugInformation.WriteTextLine("== Time Analysis ==");
            TheDebugInformation.WriteTextLine("===================");
            TheDebugInformation.WriteBlankLine();

            foreach (System.Data.DataRow TheHostIdRow in TheHostIdRowsFound)
            {
                TheDebugInformation.WriteTextLine
                    (
                    "Host Id " +
                    string.Format("{0,3}", (TheHostIdRow.Field<byte>("HostId")).ToString())
                    );

                TheDebugInformation.WriteTextLine("===========");
                TheDebugInformation.WriteBlankLine();

                FinaliseTimeValuesForHostId(TheHostIdRow.Field<byte>("HostId"));
            }
        }

        private void FinaliseTimeValuesForHostId(byte TheHostId)
        {
            CommonHistogram TheTimestampHistogram =
                new CommonHistogram
                    (
                    TheDebugInformation,
                    Constants.TimestampNumberOfBins,
                    Constants.MaxNegativeTimeDifference,
                    Constants.MaxPositiveTimeDifference
                    );

            CommonHistogram TheTimeHistogram =
                new CommonHistogram
                    (
                    TheDebugInformation,
                    Constants.TimeNumberOfBins,
                    Constants.MaxNegativeTimeDifference,
                    Constants.MaxPositiveTimeDifference
                    );

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
                    TheLastTimestamp = TheTimeValuesRow.Field<double>("Timestamp");
                    TheLastTime = TheTimeValuesRow.Field<double>("Time");

                    //The first row is always marked as processed
                    TheTimeValuesRow["Processed"] = true;

                    TheFirstRowProcessed = true;

                    continue;
                }

                //The timestamp
                {
                    double TheAbsoluteTimestampDifference = System.Math.Abs((TheTimeValuesRow.Field<double>("Timestamp") - TheLastTimestamp) * 1000.0);
                    double TheTimestampDifference = ((TheTimeValuesRow.Field<double>("Timestamp") - TheLastTimestamp) * 1000.0) - Constants.ExpectedTimeDifference; //Milliseconds;

                    if (TheAbsoluteTimestampDifference > Constants.MinTimestampDifference)
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
                            TheMinTimestampDifferencePacketNumber = TheTimeValuesRow.Field<ulong>("PacketNumber");
                        }

                        if (TheMaxTimestampDifference < TheTimestampDifference)
                        {
                            TheMaxTimestampDifference = TheTimestampDifference;
                            TheMaxTimestampDifferencePacketNumber = TheTimeValuesRow.Field<ulong>("PacketNumber");
                        }

                        TheLastTimestamp = TheTimeValuesRow.Field<double>("Timestamp");

                        //The time

                        double TheTimeDifference = ((TheTimeValuesRow.Field<double>("Time") - TheLastTime) * 1000.0) - Constants.ExpectedTimeDifference; //Milliseconds;

                        ++TheNumberOfTimeDifferenceInstances;
                        TheTotalOfTimeDifferences += TheTimeDifference;

                        TheTimeHistogram.AddValue(TheTimeDifference);

                        if (TheMinTimeDifference > TheTimeDifference)
                        {
                            TheMinTimeDifference = TheTimeDifference;
                            TheMinTimeDifferencePacketNumber = TheTimeValuesRow.Field<ulong>("PacketNumber");
                        }

                        if (TheMaxTimeDifference < TheTimeDifference)
                        {
                            TheMaxTimeDifference = TheTimeDifference;
                            TheMaxTimeDifferencePacketNumber = TheTimeValuesRow.Field<ulong>("PacketNumber");
                        }

                        TheLastTime = TheTimeValuesRow.Field<double>("Time");
                    }
                }
            }

            if (TheNumberOfTimestampDifferenceInstances > 0)
            {
                TheAverageTimestampDifference = (TheTotalOfTimestampDifferences / TheNumberOfTimestampDifferenceInstances);
                TheAverageTimeDifference = (TheTotalOfTimeDifferences / TheNumberOfTimeDifferenceInstances);

                TheDebugInformation.WriteTextLine
                    (
                    "The number of time messages was " +
                    TheNumberOfTimestampDifferenceInstances.ToString()
                    );

                TheDebugInformation.WriteBlankLine();

                TheDebugInformation.WriteTextLine
                    (
                    "The minimum timestamp difference was " +
                    TheMinTimestampDifference.ToString() +
                    " ms for packet number " +
                    TheMinTimestampDifferencePacketNumber.ToString()
                    );

                TheDebugInformation.WriteTextLine
                    (
                    "The maximum timestamp difference was " +
                    TheMaxTimestampDifference.ToString() +
                    " ms for packet number " +
                    TheMaxTimestampDifferencePacketNumber.ToString()
                    );

                TheDebugInformation.WriteTextLine
                    (
                    "The average timestamp difference was " +
                    TheAverageTimestampDifference.ToString() +
                    " ms"
                    );

                TheDebugInformation.WriteBlankLine();

                //Output the histogram

                TheDebugInformation.WriteTextLine
                    (
                    "The histogram (" +
                    Constants.TimestampBinsPerMillisecond.ToString() +
                    " bins per millisecond) for timestamp values is:"
                    );

                TheDebugInformation.WriteBlankLine();

                TheTimestampHistogram.OutputValues();

                TheDebugInformation.WriteBlankLine();

                TheDebugInformation.WriteTextLine
                    (
                    "The minimum time difference was " +
                    TheMinTimeDifference.ToString() +
                    " ms for packet number " +
                    TheMinTimeDifferencePacketNumber.ToString()
                    );

                TheDebugInformation.WriteTextLine
                    (
                    "The maximum time difference was " +
                    TheMaxTimeDifference.ToString() +
                    " ms for packet number " +
                    TheMaxTimeDifferencePacketNumber.ToString()
                    );

                TheDebugInformation.WriteTextLine
                    (
                    "The average time difference was " +
                    TheAverageTimeDifference.ToString() +
                    " ms"
                    );

                TheDebugInformation.WriteBlankLine();

                //Output the histogram

                TheDebugInformation.WriteTextLine
                    (
                    "The histogram (" +
                    Constants.TimeBinsPerMillisecond.ToString() +
                    " bins per millisecond) for time values is:"
                    );

                TheDebugInformation.WriteBlankLine();

                TheTimeHistogram.OutputValues();
            }

            TheDebugInformation.WriteBlankLine();

            if (OutputDebug)
            {
                System.Text.StringBuilder OutputDebugLines = new System.Text.StringBuilder();

                string OutputDebugTitleLine = string.Format
                    (
                    "{0},{1}{2}",
                    "Timestamp",
                    "Time",
                    System.Environment.NewLine
                    );

                OutputDebugLines.Append(OutputDebugTitleLine);

                foreach (System.Data.DataRow TheTimeValuesRow in TheTimeValuesRowsFound)
                {
                    if (TheTimeValuesRow.Field<bool>("Processed"))
                    {
                        string OutputDebugLine = string.Format
                            (
                            "{0},{1}{2}",
                            (TheTimeValuesRow.Field<double>("Timestamp")).ToString(),
                            (TheTimeValuesRow.Field<double>("Time")).ToString(),
                            System.Environment.NewLine
                            );

                        OutputDebugLines.Append(OutputDebugLine);
                    }
                }

                System.IO.File.WriteAllText
                    (
                    SelectedPacketCaptureFile + ".HostId" + TheHostId + ".TimeAnalysis.csv",
                    OutputDebugLines.ToString()
                    );
            }
        }
    }
}
