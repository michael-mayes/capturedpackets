// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace Analysis.TimeAnalysis
{
    using System.Data; // Required to be able to use AsEnumerable method
    using System.Linq; // Required to be able to use Count method

    // This class will implement the Disposable class so as to be able to clean up after the datatables it creates which themselves implement the Disposable class
    class Processing : System.IDisposable
    {
        private Analysis.DebugInformation theDebugInformation;

        private bool outputDebug;

        private string selectedPacketCaptureFile;

        private System.Data.DataTable theTimeValuesTable;
        private System.Data.DataTable theHostIdsTable;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation"></param>
        /// <param name="outputDebug"></param>
        /// <param name="selectedPacketCaptureFile"></param>
        public Processing(Analysis.DebugInformation theDebugInformation, bool outputDebug, string selectedPacketCaptureFile)
        {
            this.theDebugInformation = theDebugInformation;

            this.outputDebug = outputDebug;

            this.selectedPacketCaptureFile = selectedPacketCaptureFile;

            // Create a datatable to hold the timestamp and time values for time-supplying messages
            this.theTimeValuesTable = new System.Data.DataTable();

            // Create a datatable to hold the set of host Ids encountered during the time analysis
            this.theHostIdsTable = new System.Data.DataTable();
        }

        public void Create()
        {
            // Add the required columns to the datatable to hold the timestamp and time values for time-supplying messages
            this.theTimeValuesTable.Columns.Add("HostId", typeof(byte));
            this.theTimeValuesTable.Columns.Add("PacketNumber", typeof(ulong));
            this.theTimeValuesTable.Columns.Add("Timestamp", typeof(double));
            this.theTimeValuesTable.Columns.Add("Time", typeof(double));
            this.theTimeValuesTable.Columns.Add("Processed", typeof(bool));

            // Add the required column to the datatable to hold the set of host Ids encountered during the time analysis
            this.theHostIdsTable.Columns.Add("HostId", typeof(byte));

            // Set the primary key to be the only column
            // The primary key is needed to allow for use of the Find method against the datatable
            this.theHostIdsTable.PrimaryKey =
                new System.Data.DataColumn[]
                {
                    this.theHostIdsTable.Columns["HostId"]
                };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose any resources allocated to the datatables if instructed
                this.theTimeValuesTable.Dispose();
                this.theHostIdsTable.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);

            System.GC.SuppressFinalize(this);
        }

        public void RegisterTimeMessageReceipt(byte theHostId, ulong thePacketNumber, double theTimestamp, double theTime)
        {
            // Add the supplied Host Id to the set of those encountered during the time analysis if not already in there
            this.RegisterEncounteredHostId(theHostId);

            //// Add the supplied timestamp and time to the datatable

            System.Data.DataRow theTimeValuesRowToAdd = this.theTimeValuesTable.NewRow();

            theTimeValuesRowToAdd["HostId"] = theHostId;
            theTimeValuesRowToAdd["PacketNumber"] = thePacketNumber;
            theTimeValuesRowToAdd["Timestamp"] = theTimestamp;
            theTimeValuesRowToAdd["Time"] = theTime;
            theTimeValuesRowToAdd["Processed"] = false;

            this.theTimeValuesTable.Rows.Add(theTimeValuesRowToAdd);
        }

        // Add the supplied host Id to the set of those encountered during the time analysis if not already in there
        private void RegisterEncounteredHostId(byte theHostId)
        {
            object[] theHostIdRowFindObject = new object[1];

            theHostIdRowFindObject[0] = theHostId.ToString(); // Primary key

            System.Data.DataRow theHostIdDataRowFound = this.theHostIdsTable.Rows.Find(theHostIdRowFindObject);

            if (theHostIdDataRowFound == null)
            {
                this.theDebugInformation.WriteInformationEvent(
                    "Found a time-supplying message for a Host Id " +
                    string.Format("{0,3}", theHostId) +
                    " - adding this Host Id to the time analysis");

                System.Data.DataRow theHostIdRowToAdd = this.theHostIdsTable.NewRow();

                theHostIdRowToAdd["HostId"] = theHostId;

                this.theHostIdsTable.Rows.Add(theHostIdRowToAdd);
            }
        }

        public void Finalise()
        {
            // Obtain the set of Host Ids encountered during the time analysis in ascending order
            EnumerableRowCollection<System.Data.DataRow>
                theHostIdRowsFound =
                from r in this.theHostIdsTable.AsEnumerable()
                orderby r.Field<byte>("HostId") ascending
                select r;

            //// Loop across all the time values for each of these Host Ids in turn

            this.theDebugInformation.WriteBlankLine();
            this.theDebugInformation.WriteTextLine("===================");
            this.theDebugInformation.WriteTextLine("== Time Analysis ==");
            this.theDebugInformation.WriteTextLine("===================");
            this.theDebugInformation.WriteBlankLine();

            foreach (System.Data.DataRow theHostIdRow in theHostIdRowsFound)
            {
                this.theDebugInformation.WriteTextLine(
                    "Host Id " +
                    string.Format("{0,3}", (theHostIdRow.Field<byte>("HostId")).ToString()));

                this.theDebugInformation.WriteTextLine("===========");
                this.theDebugInformation.WriteBlankLine();

                this.FinaliseTimeValuesForHostId(theHostIdRow.Field<byte>("HostId"));
            }
        }

        private void FinaliseTimeValuesForHostId(byte theHostId)
        {
            CommonHistogram theTimestampHistogram =
                new CommonHistogram(
                    this.theDebugInformation,
                    Constants.TimestampNumberOfBins,
                    Constants.MaxNegativeTimeDifference,
                    Constants.MaxPositiveTimeDifference);

            CommonHistogram theTimeHistogram =
                new CommonHistogram(
                    this.theDebugInformation,
                    Constants.TimeNumberOfBins,
                    Constants.MaxNegativeTimeDifference,
                    Constants.MaxPositiveTimeDifference);

            ulong theMinTimestampDifferencePacketNumber = 0;
            ulong theMaxTimestampDifferencePacketNumber = 0;

            ulong theMinTimeDifferencePacketNumber = 0;
            ulong theMaxTimeDifferencePacketNumber = 0;

            double theMinTimestampDifference = double.MaxValue;
            double theMaxTimestampDifference = double.MinValue;

            double theMinTimeDifference = double.MaxValue;
            double theMaxTimeDifference = double.MinValue;

            double theLastTimestamp = 0.0;
            double theLastTime = 0.0;

            ulong theNumberOfTimestampDifferenceInstances = 0;
            double theTotalOfTimestampDifferences = 0;
            double theAverageTimestampDifference = 0;

            ulong theNumberOfTimeDifferenceInstances = 0;
            double theTotalOfTimeDifferences = 0;
            double theAverageTimeDifference = 0;

            bool theFirstRowProcessed = false;

            EnumerableRowCollection<System.Data.DataRow>
                theTimeValuesRowsFound =
                from r in this.theTimeValuesTable.AsEnumerable()
                where r.Field<byte>("HostId") == theHostId
                select r;

            foreach (System.Data.DataRow theTimeValuesRow in theTimeValuesRowsFound)
            {
                // Do not calculate the differences in timestamp and time for first row - just record values and move on to second row
                if (!theFirstRowProcessed)
                {
                    theLastTimestamp = theTimeValuesRow.Field<double>("Timestamp");
                    theLastTime = theTimeValuesRow.Field<double>("Time");

                    // The first row is always marked as processed
                    theTimeValuesRow["Processed"] = true;

                    theFirstRowProcessed = true;

                    continue;
                }

                // The timestamp
                {
                    double theAbsoluteTimestampDifference = System.Math.Abs((theTimeValuesRow.Field<double>("Timestamp") - theLastTimestamp) * 1000.0);
                    double theTimestampDifference = ((theTimeValuesRow.Field<double>("Timestamp") - theLastTimestamp) * 1000.0) - Constants.ExpectedTimeDifference; // Milliseconds;

                    if (theAbsoluteTimestampDifference > Constants.MinTimestampDifference)
                    {
                        // Only those time messages in the chosen range will be marked as processed
                        // This should prevent the processing of duplicates of a time message (e.g. if port mirroring results in two copies of the time message)
                        theTimeValuesRow["Processed"] = true;

                        // Keep a running total to allow for averaging
                        ++theNumberOfTimestampDifferenceInstances;
                        theTotalOfTimestampDifferences += theTimestampDifference;

                        theTimestampHistogram.AddValue(theTimestampDifference);

                        if (theMinTimestampDifference > theTimestampDifference)
                        {
                            theMinTimestampDifference = theTimestampDifference;
                            theMinTimestampDifferencePacketNumber = theTimeValuesRow.Field<ulong>("PacketNumber");
                        }

                        if (theMaxTimestampDifference < theTimestampDifference)
                        {
                            theMaxTimestampDifference = theTimestampDifference;
                            theMaxTimestampDifferencePacketNumber = theTimeValuesRow.Field<ulong>("PacketNumber");
                        }

                        theLastTimestamp = theTimeValuesRow.Field<double>("Timestamp");

                        //// The time

                        double theTimeDifference = ((theTimeValuesRow.Field<double>("Time") - theLastTime) * 1000.0) - Constants.ExpectedTimeDifference; // Milliseconds;

                        ++theNumberOfTimeDifferenceInstances;
                        theTotalOfTimeDifferences += theTimeDifference;

                        theTimeHistogram.AddValue(theTimeDifference);

                        if (theMinTimeDifference > theTimeDifference)
                        {
                            theMinTimeDifference = theTimeDifference;
                            theMinTimeDifferencePacketNumber = theTimeValuesRow.Field<ulong>("PacketNumber");
                        }

                        if (theMaxTimeDifference < theTimeDifference)
                        {
                            theMaxTimeDifference = theTimeDifference;
                            theMaxTimeDifferencePacketNumber = theTimeValuesRow.Field<ulong>("PacketNumber");
                        }

                        theLastTime = theTimeValuesRow.Field<double>("Time");
                    }
                }
            }

            if (theNumberOfTimestampDifferenceInstances > 0)
            {
                theAverageTimestampDifference = theTotalOfTimestampDifferences / theNumberOfTimestampDifferenceInstances;
                theAverageTimeDifference = theTotalOfTimeDifferences / theNumberOfTimeDifferenceInstances;

                this.theDebugInformation.WriteTextLine(
                    "The number of time messages was " +
                    theNumberOfTimestampDifferenceInstances.ToString());

                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "The minimum timestamp difference was " +
                    theMinTimestampDifference.ToString() +
                    " ms for packet number " +
                    theMinTimestampDifferencePacketNumber.ToString());

                this.theDebugInformation.WriteTextLine(
                    "The maximum timestamp difference was " +
                    theMaxTimestampDifference.ToString() +
                    " ms for packet number " +
                    theMaxTimestampDifferencePacketNumber.ToString());

                this.theDebugInformation.WriteTextLine(
                    "The average timestamp difference was " +
                    theAverageTimestampDifference.ToString() +
                    " ms");

                this.theDebugInformation.WriteBlankLine();

                //// Output the histogram

                this.theDebugInformation.WriteTextLine(
                    "The histogram (" +
                    Constants.TimestampBinsPerMillisecond.ToString() +
                    " bins per millisecond) for timestamp values is:");

                this.theDebugInformation.WriteBlankLine();

                theTimestampHistogram.OutputValues();

                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "The minimum time difference was " +
                    theMinTimeDifference.ToString() +
                    " ms for packet number " +
                    theMinTimeDifferencePacketNumber.ToString());

                this.theDebugInformation.WriteTextLine(
                    "The maximum time difference was " +
                    theMaxTimeDifference.ToString() +
                    " ms for packet number " +
                    theMaxTimeDifferencePacketNumber.ToString());

                this.theDebugInformation.WriteTextLine(
                    "The average time difference was " +
                    theAverageTimeDifference.ToString() +
                    " ms");

                this.theDebugInformation.WriteBlankLine();

                //// Output the histogram

                this.theDebugInformation.WriteTextLine(
                    "The histogram (" +
                    Constants.TimeBinsPerMillisecond.ToString() +
                    " bins per millisecond) for time values is:");

                this.theDebugInformation.WriteBlankLine();

                theTimeHistogram.OutputValues();
            }

            this.theDebugInformation.WriteBlankLine();

            if (this.outputDebug)
            {
                System.Text.StringBuilder outputDebugLines = new System.Text.StringBuilder();

                string outputDebugTitleLine = string.Format(
                    "{0},{1}{2}",
                    "Timestamp",
                    "Time",
                    System.Environment.NewLine);

                outputDebugLines.Append(outputDebugTitleLine);

                foreach (System.Data.DataRow theTimeValuesRow in theTimeValuesRowsFound)
                {
                    if (theTimeValuesRow.Field<bool>("Processed"))
                    {
                        string outputDebugLine = string.Format(
                            "{0},{1}{2}",
                            (theTimeValuesRow.Field<double>("Timestamp")).ToString(),
                            (theTimeValuesRow.Field<double>("Time")).ToString(),
                            System.Environment.NewLine);

                        outputDebugLines.Append(outputDebugLine);
                    }
                }

                System.IO.File.WriteAllText(
                    this.selectedPacketCaptureFile +
                    ".HostId" +
                    theHostId +
                    ".TimeAnalysis.csv",
                    outputDebugLines.ToString());
            }
        }
    }
}
