// $Id$
// $URL$
// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace Analysis.TimeAnalysis
{
    using System.Data; // Required to be able to use AsEnumerable method
    using System.Linq; // Required to be able to use Count method

    /// <summary>
    /// This class provides the time analysis processing
    /// This class will implement the Disposable class so as to be able to clean up after the data tables it creates which themselves implement the Disposable class
    /// </summary>
    public class Processing : System.IDisposable
    {
        /// <summary>
        /// The object that provides for the logging of debug information
        /// </summary>
        private Analysis.DebugInformation theDebugInformation;

        /// <summary>
        /// Boolean flag that indicates whether to output additional information
        /// </summary>
        private bool outputAdditionalInformation;

        /// <summary>
        /// The path of the selected packet capture
        /// </summary>
        private string theSelectedPacketCaptureFile;

        /// <summary>
        /// The data table to hold the timestamp and time values for time-supplying messages
        /// </summary>
        private System.Data.DataTable theTimeValuesTable;

        /// <summary>
        /// The data table to hold the set of host Ids encountered during the time analysis
        /// </summary>
        private System.Data.DataTable theHostIdsTable;

        /// <summary>
        /// Initializes a new instance of the Processing class
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="outputAdditionalInformation">Boolean flag that indicates whether to output additional information</param>
        /// <param name="theSelectedPacketCaptureFile">The path of the selected packet capture</param>
        public Processing(Analysis.DebugInformation theDebugInformation, bool outputAdditionalInformation, string theSelectedPacketCaptureFile)
        {
            this.theDebugInformation = theDebugInformation;

            this.outputAdditionalInformation = outputAdditionalInformation;

            this.theSelectedPacketCaptureFile = theSelectedPacketCaptureFile;

            // Create a datatable to hold the timestamp and time values for time-supplying messages
            this.theTimeValuesTable = new System.Data.DataTable();

            // Create a datatable to hold the set of host Ids encountered during the time analysis
            this.theHostIdsTable = new System.Data.DataTable();
        }

        /// <summary>
        /// Creates the data tables necessary to perform the time analysis processing
        /// </summary>
        public void Create()
        {
            // Add the required columns to the datatable to hold the timestamp and time values for time-supplying messages
            this.theTimeValuesTable.Columns.Add("HostId", typeof(byte));
            this.theTimeValuesTable.Columns.Add("PacketNumber", typeof(ulong));
            this.theTimeValuesTable.Columns.Add("PacketTimestamp", typeof(double));
            this.theTimeValuesTable.Columns.Add("PacketTime", typeof(double));
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

        /// <summary>
        /// Clean up any resources used by the time analysis class
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Supplies a set of data for a time message to register its receipt for the purposes of time analysis
        /// </summary>
        /// <param name="theHostId">The host Id for the message</param>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <param name="thePacketTime">The time for the packet read from the packet capture</param>
        public void RegisterTimeMessageReceipt(byte theHostId, ulong thePacketNumber, double thePacketTimestamp, double thePacketTime)
        {
            // Add the supplied Host Id to the set of those encountered during the time analysis if not already in there
            this.RegisterEncounteredHostId(theHostId);

            //// Add the supplied timestamp and time to the datatable

            System.Data.DataRow theTimeValuesRowToAdd = this.theTimeValuesTable.NewRow();

            theTimeValuesRowToAdd["HostId"] = theHostId;
            theTimeValuesRowToAdd["PacketNumber"] = thePacketNumber;
            theTimeValuesRowToAdd["PacketTimestamp"] = thePacketTimestamp;
            theTimeValuesRowToAdd["PacketTime"] = thePacketTime;
            theTimeValuesRowToAdd["Processed"] = false;

            this.theTimeValuesTable.Rows.Add(theTimeValuesRowToAdd);
        }

        /// <summary>
        /// Finalize the time analysis
        /// </summary>
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
                    string.Format("{0,3}", theHostIdRow.Field<byte>("HostId").ToString()));

                this.theDebugInformation.WriteTextLine("===========");
                this.theDebugInformation.WriteBlankLine();

                this.FinaliseForHostId(theHostIdRow.Field<byte>("HostId"));
            }
        }

        /// <summary>
        /// Clean up any resources used by the time analysis class
        /// </summary>
        /// <param name="disposing">Boolean flag that indicates whether the method call comes from a Dispose method (its value is true) or from the garbage collector (its value is false)</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose any resources allocated to the datatables if instructed
                this.theTimeValuesTable.Dispose();
                this.theHostIdsTable.Dispose();
            }
        }

        /// <summary>
        /// Adds the supplied host Id to the set of those encountered during the time analysis if not already in there
        /// </summary>
        /// <param name="theHostId">The host Id encountered</param>
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

        /// <summary>
        /// Finalize time analysis for the supplied host Id
        /// </summary>
        /// <param name="theHostId">The host Id for which time analysis is to be finalized</param>
        private void FinaliseForHostId(byte theHostId)
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

            System.Collections.Generic.List<string> theOutOfRangeTimestamps =
                new System.Collections.Generic.List<string>();

            System.Collections.Generic.List<string> theOutOfRangeTimes =
                new System.Collections.Generic.List<string>();

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
                    theLastTimestamp = theTimeValuesRow.Field<double>("PacketTimestamp");
                    theLastTime = theTimeValuesRow.Field<double>("PacketTime");

                    // The first row is always marked as processed
                    theTimeValuesRow["Processed"] = true;

                    theFirstRowProcessed = true;

                    continue;
                }

                // The timestamp
                {
                    double theAbsoluteTimestampDifference = System.Math.Abs((theTimeValuesRow.Field<double>("PacketTimestamp") - theLastTimestamp) * 1000.0);
                    double theTimestampDifference = ((theTimeValuesRow.Field<double>("PacketTimestamp") - theLastTimestamp) * 1000.0) - Constants.ExpectedTimeDifference; // Milliseconds;

                    if (theAbsoluteTimestampDifference > Constants.MinTimestampDifference)
                    {
                        // Only those time messages in the chosen range will be marked as processed
                        // This should prevent the processing of duplicates of a time message (e.g. if port mirroring results in two copies of the time message)
                        theTimeValuesRow["Processed"] = true;

                        // Keep a running total to allow for averaging
                        ++theNumberOfTimestampDifferenceInstances;
                        theTotalOfTimestampDifferences += theTimestampDifference;

                        if (!theTimestampHistogram.AddValue(theTimestampDifference))
                        {
                            theOutOfRangeTimestamps.Add(
                                "The time message with packet number " +
                                string.Format("{0,7}", theTimeValuesRow.Field<ulong>("PacketNumber").ToString()) +
                                " has an out of range timestamp difference of " +
                                string.Format("{0,18}", theTimestampDifference.ToString()) +
                                " ms");
                        }

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

                        theLastTimestamp = theTimeValuesRow.Field<double>("PacketTimestamp");

                        //// The time

                        double theTimeDifference = ((theTimeValuesRow.Field<double>("PacketTime") - theLastTime) * 1000.0) - Constants.ExpectedTimeDifference; // Milliseconds;

                        ++theNumberOfTimeDifferenceInstances;
                        theTotalOfTimeDifferences += theTimeDifference;

                        if (!theTimeHistogram.AddValue(theTimeDifference))
                        {
                            theOutOfRangeTimes.Add(
                                "The time message with packet number " +
                                string.Format("{0,7}", theTimeValuesRow.Field<ulong>("PacketNumber").ToString()) +
                                " has an out of range time difference of " +
                                string.Format("{0,18}", theTimeDifference.ToString()) +
                                " ms");
                        }

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

                        theLastTime = theTimeValuesRow.Field<double>("PacketTime");
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
                    string.Format("{0,18}", theMinTimestampDifference.ToString()) +
                    " ms for packet number " +
                    string.Format("{0,7}", theMinTimestampDifferencePacketNumber.ToString()));

                this.theDebugInformation.WriteTextLine(
                    "The maximum timestamp difference was " +
                    string.Format("{0,18}", theMaxTimestampDifference.ToString()) +
                    " ms for packet number " +
                    string.Format("{0,7}", theMaxTimestampDifferencePacketNumber.ToString()));

                this.theDebugInformation.WriteTextLine(
                    "The average timestamp difference was " +
                    string.Format("{0,18}", theAverageTimestampDifference.ToString()) +
                    " ms");

                this.theDebugInformation.WriteBlankLine();

                //// Output the histogram

                this.theDebugInformation.WriteTextLine(
                    "The histogram (" +
                    Constants.TimestampBinsPerMillisecond.ToString() +
                    " bins per millisecond) for timestamp values is:");

                this.theDebugInformation.WriteBlankLine();

                theTimestampHistogram.OutputValues();

                if (theOutOfRangeTimestamps.Count > 0)
                {
                    this.theDebugInformation.WriteBlankLine();

                    // Output the data for any time message with an out of range timestamp difference
                    foreach (string theString in theOutOfRangeTimestamps)
                    {
                        this.theDebugInformation.WriteTextLine(theString);
                    }
                }

                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(new string('-', 144));

                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "The minimum time difference was " +
                    string.Format("{0,18}", theMinTimeDifference.ToString()) +
                    " ms for packet number " +
                    string.Format("{0,7}", theMinTimeDifferencePacketNumber.ToString()));

                this.theDebugInformation.WriteTextLine(
                    "The maximum time difference was " +
                    string.Format("{0,18}", theMaxTimeDifference.ToString()) +
                    " ms for packet number " +
                    string.Format("{0,7}", theMaxTimeDifferencePacketNumber.ToString()));

                this.theDebugInformation.WriteTextLine(
                    "The average time difference was " +
                    string.Format("{0,18}", theAverageTimeDifference.ToString()) +
                    " ms");

                this.theDebugInformation.WriteBlankLine();

                //// Output the histogram

                this.theDebugInformation.WriteTextLine(
                    "The histogram (" +
                    Constants.TimeBinsPerMillisecond.ToString() +
                    " bins per millisecond) for time values is:");

                this.theDebugInformation.WriteBlankLine();

                theTimeHistogram.OutputValues();

                if (theOutOfRangeTimes.Count > 0)
                {
                    this.theDebugInformation.WriteBlankLine();

                    // Output the data for any time message with an out of range time difference
                    foreach (string theString in theOutOfRangeTimes)
                    {
                        this.theDebugInformation.WriteTextLine(theString);
                    }
                }
            }

            this.theDebugInformation.WriteBlankLine();

            this.theDebugInformation.WriteTextLine(new string('-', 144));

            this.theDebugInformation.WriteBlankLine();

            if (this.outputAdditionalInformation)
            {
                System.Text.StringBuilder theOutputAdditionalInformationLines =
                    new System.Text.StringBuilder();

                //// Add a column titles line to the debug output file

                string theOutputAdditionalInformationTitleLine = string.Format(
                    "{0},{1},{2},,{3},{4}{5}",
                    "Packet Number",
                    "Packet Timestamp (s)",
                    "Message Time (s)",
                    "Packet Timestamp Delta (ms)",
                    "Message Time Delta (ms)",
                    System.Environment.NewLine);

                theOutputAdditionalInformationLines.Append(theOutputAdditionalInformationTitleLine);

                double? thePreviousPacketTimestamp = null;
                double? thePreviousPacketTime = null;

                //// Add a line to the debug output file for each of the processed time messages

                foreach (System.Data.DataRow theTimeValuesRow in theTimeValuesRowsFound)
                {
                    if (theTimeValuesRow.Field<bool>("Processed"))
                    {
                        string theOutputAdditionalInformationLine = null;

                        double theCurrentPacketTimestamp = theTimeValuesRow.Field<double>("PacketTimestamp");

                        double theCurrentPacketTime = theTimeValuesRow.Field<double>("PacketTime");

                        if (thePreviousPacketTimestamp == null || thePreviousPacketTime == null)
                        {
                            // If this is the first time message then there is no previous time message and so just output the timestamp and time
                            theOutputAdditionalInformationLine = string.Format(
                                "{0},{1,18},{2,18}{3}",
                                theTimeValuesRow.Field<ulong>("PacketNumber").ToString(),
                                theTimeValuesRow.Field<double>("PacketTimestamp").ToString(),
                                theTimeValuesRow.Field<double>("PacketTime").ToString(),
                                System.Environment.NewLine);
                        }
                        else
                        {
                            // If this is another time message then also calculate the differences in timestamp and time from the previous time message and output them along with the timestamp and time
                            theOutputAdditionalInformationLine = string.Format(
                                "{0},{1,18},{2,18},,{3,18},{4,18}{5}",
                                theTimeValuesRow.Field<ulong>("PacketNumber").ToString(),
                                theTimeValuesRow.Field<double>("PacketTimestamp").ToString(),
                                theTimeValuesRow.Field<double>("PacketTime").ToString(),
                                (((theCurrentPacketTimestamp - thePreviousPacketTimestamp) * 1000.0) - Constants.ExpectedTimeDifference).ToString(), // Milliseconds
                                (((theCurrentPacketTime - thePreviousPacketTime) * 1000.0) - Constants.ExpectedTimeDifference).ToString(), // Milliseconds
                                System.Environment.NewLine);
                        }

                        thePreviousPacketTimestamp = theCurrentPacketTimestamp;
                        thePreviousPacketTime = theCurrentPacketTime;

                        theOutputAdditionalInformationLines.Append(theOutputAdditionalInformationLine);
                    }
                }

                System.IO.File.WriteAllText(
                    this.theSelectedPacketCaptureFile +
                    ".HostId" +
                    theHostId +
                    ".TimeAnalysis.csv",
                    theOutputAdditionalInformationLines.ToString());
            }
        }
    }
}