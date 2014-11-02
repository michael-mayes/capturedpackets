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
    using System.Linq; // Required to be able to use Any method

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
        /// Boolean flag that indicates whether to output the histograms
        /// </summary>
        private bool outputHistograms;

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
        /// <param name="outputHistograms">Boolean flag that indicates whether to output the histograms</param>
        /// <param name="outputAdditionalInformation">Boolean flag that indicates whether to output additional information</param>
        /// <param name="theSelectedPacketCaptureFile">The path of the selected packet capture</param>
        public Processing(Analysis.DebugInformation theDebugInformation, bool outputHistograms, bool outputAdditionalInformation, string theSelectedPacketCaptureFile)
        {
            this.theDebugInformation = theDebugInformation;

            this.outputHistograms = outputHistograms;

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
        /// Supplies a set of data for a time-supplying message to register its receipt for the purposes of time analysis
        /// </summary>
        /// <param name="theHostId">The host Id for the message</param>
        /// <param name="thePacketNumber">The number for the packet read from the packet capture</param>
        /// <param name="thePacketTimestamp">The timestamp for the packet read from the packet capture</param>
        /// <param name="thePacketTime">The time for the packet read from the packet capture</param>
        public void RegisterTimeMessageReceipt(byte theHostId, ulong thePacketNumber, double thePacketTimestamp, double thePacketTime)
        {
            //// Add the supplied timestamp and time to the datatable

            System.Data.DataRow theTimeValuesRowToAdd =
                this.theTimeValuesTable.NewRow();

            theTimeValuesRowToAdd["HostId"] = theHostId;
            theTimeValuesRowToAdd["PacketNumber"] = thePacketNumber;
            theTimeValuesRowToAdd["PacketTimestamp"] = thePacketTimestamp;
            theTimeValuesRowToAdd["PacketTime"] = thePacketTime;
            theTimeValuesRowToAdd["Processed"] = false;

            this.theTimeValuesTable.Rows.Add(
                theTimeValuesRowToAdd);

            // Add the supplied Host Id to the set of those encountered during the time analysis if not already in there
            this.RegisterEncounteredHostId(
                theHostId);
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

            System.Data.DataRow theHostIdDataRowFound =
                this.theHostIdsTable.Rows.Find(
                theHostIdRowFindObject);

            if (theHostIdDataRowFound == null)
            {
                this.theDebugInformation.WriteInformationEvent(
                    "Found a time-supplying message for a Host Id " +
                    string.Format("{0,3}", theHostId) +
                    " - adding this Host Id to the time analysis");

                System.Data.DataRow theHostIdRowToAdd =
                    this.theHostIdsTable.NewRow();

                theHostIdRowToAdd["HostId"] = theHostId;

                this.theHostIdsTable.Rows.Add(
                    theHostIdRowToAdd);
            }
        }

        /// <summary>
        /// Finalize time analysis for the supplied host Id
        /// </summary>
        /// <param name="theHostId">The host Id for which time analysis is to be finalized</param>
        private void FinaliseForHostId(byte theHostId)
        {
            CommonHistogram theLowerRangeTimestampHistogram =
                new CommonHistogram(
                    this.theDebugInformation,
                    Constants.LowerRangeTimestampNumberOfBins,
                    Constants.MinLowerRangeTimestampDifference,
                    Constants.MaxLowerRangeTimestampDifference);

            CommonHistogram theMiddleRangeTimestampHistogram =
                new CommonHistogram(
                    this.theDebugInformation,
                    Constants.MiddleRangeTimestampNumberOfBins,
                    Constants.MinMiddleRangeTimestampDifference,
                    Constants.MaxMiddleRangeTimestampDifference);

            CommonHistogram theUpperRangeTimestampHistogram =
                new CommonHistogram(
                    this.theDebugInformation,
                    Constants.UpperRangeTimestampNumberOfBins,
                    Constants.MinUpperRangeTimestampDifference,
                    Constants.MaxUpperRangeTimestampDifference);

            CommonHistogram theLowerRangeTimeHistogram =
                new CommonHistogram(
                    this.theDebugInformation,
                    Constants.LowerRangeTimeNumberOfBins,
                    Constants.MinLowerRangeTimeDifference,
                    Constants.MaxLowerRangeTimeDifference);

            CommonHistogram theMiddleRangeTimeHistogram =
                new CommonHistogram(
                    this.theDebugInformation,
                    Constants.MiddleRangeTimeNumberOfBins,
                    Constants.MinMiddleRangeTimeDifference,
                    Constants.MaxMiddleRangeTimeDifference);

            CommonHistogram theUpperRangeTimeHistogram =
                new CommonHistogram(
                    this.theDebugInformation,
                    Constants.UpperRangeTimeNumberOfBins,
                    Constants.MinUpperRangeTimeDifference,
                    Constants.MaxUpperRangeTimeDifference);

            ulong theNumberOfMessages = 0;

            ulong theNumberOfIgnoredMessages = 0;

            double theMinTimestampDifference = double.MaxValue;
            double theMaxTimestampDifference = double.MinValue;

            double theMinTimeDifference = double.MaxValue;
            double theMaxTimeDifference = double.MinValue;

            ulong theMinTimestampDifferencePacketNumber = 0;
            ulong theMaxTimestampDifferencePacketNumber = 0;

            ulong theMinTimeDifferencePacketNumber = 0;
            ulong theMaxTimeDifferencePacketNumber = 0;

            double theLastTimestamp = 0.0;
            double theLastTime = 0.0;

            ulong theNumberOfTimestampDifferenceInstances = 0;
            double theTotalOfTimestampDifferences = 0;
            double theAverageTimestampDifference = 0;

            ulong theNumberOfTimeDifferenceInstances = 0;
            double theTotalOfTimeDifferences = 0;
            double theAverageTimeDifference = 0;

            bool theFirstRowProcessed = false;

            System.Collections.Generic.List<string> theIgnoredTimestamps =
                new System.Collections.Generic.List<string>();

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
                // Keep a running total of the number of time-supplying messages
                ++theNumberOfMessages;

                ulong thePacketNumber = theTimeValuesRow.Field<ulong>("PacketNumber");
                double thePacketTimestamp = theTimeValuesRow.Field<double>("PacketTimestamp");
                double thePacketTime = theTimeValuesRow.Field<double>("PacketTime");

                // Do not calculate the differences in timestamp and time for first row - just record values and move on to second row
                if (!theFirstRowProcessed)
                {
                    theLastTimestamp = thePacketTimestamp;
                    theLastTime = thePacketTime;

                    // The first time-supplying message is always marked as processed
                    theTimeValuesRow["Processed"] = true;

                    theFirstRowProcessed = true;

                    continue;
                }

                //// The timestamp

                double theAbsoluteTimestampDifference =
                    System.Math.Abs((thePacketTimestamp - theLastTimestamp) * 1000.0); // Milliseconds

                // Only those time-supplying messages with a timestamp difference in the chosen range will be marked as processed
                // This should prevent the processing of duplicates of a time-supplying message (e.g. if port mirroring results in two copies of the time-supplying message)
                // This should prevent a large gap in time-supplying messages unduly affecting the processing (e.g. if the host is in the process of being restarted or becomes available)
                if (theAbsoluteTimestampDifference < Constants.MinAbsoluteTimestampDifference)
                {
                    // Keep a running total of the number of time-supplying messages that were ignored and discarded
                    ++theNumberOfIgnoredMessages;

                    theIgnoredTimestamps.Add(
                        "The time-supplying message with packet number " +
                        string.Format("{0,7}", thePacketNumber.ToString()) +
                        " has an absolute timestamp difference of " +
                        string.Format("{0,19}", theAbsoluteTimestampDifference.ToString()) +
                        " ms which is below the minimum value of " +
                        string.Format("{0,5}", Constants.MinAbsoluteTimestampDifference.ToString()) +
                        " ms so it has been ignored and discarded");
                }
                else if (theAbsoluteTimestampDifference > Constants.MaxAbsoluteTimestampDifference)
                {
                    // Keep a running total of the number of time-supplying messages that were ignored and discarded
                    ++theNumberOfIgnoredMessages;

                    theIgnoredTimestamps.Add(
                        "The time-supplying message with packet number " +
                        string.Format("{0,7}", thePacketNumber.ToString()) +
                        " has an absolute timestamp difference of " +
                        string.Format("{0,19}", theAbsoluteTimestampDifference.ToString()) +
                        " ms which is above the maximum value of " +
                        string.Format("{0,5}", Constants.MaxAbsoluteTimestampDifference.ToString()) +
                        " ms so it has been ignored and discarded");

                    // Keep a record of the last timestamp received (from this time-supplying
                    // message) to allow comparison with the next timestamp received
                    theLastTimestamp = thePacketTimestamp;

                    // Keep a record of the last time received (from this time-supplying
                    // message) to allow comparison with the next times received
                    theLastTime = thePacketTime;
                }
                else
                {
                    double theTimestampDifference =
                        (thePacketTimestamp - theLastTimestamp) * 1000.0; // Milliseconds;

                    // Mark this time-supplying message as processed as its timestamp difference is within the chosen range
                    theTimeValuesRow["Processed"] = true;

                    //// Keep a running total of the timestamp differences to allow for averaging

                    ++theNumberOfTimestampDifferenceInstances;

                    theTotalOfTimestampDifferences += theTimestampDifference;

                    if (theTimestampDifference >= Constants.MinLowerRangeTimestampDifference &&
                        theTimestampDifference <= Constants.MaxLowerRangeTimestampDifference)
                    {
                        if (!theLowerRangeTimestampHistogram.AddValue(theTimestampDifference))
                        {
                            theOutOfRangeTimestamps.Add(
                                "The time-supplying message with packet number " +
                                string.Format("{0,7}", thePacketNumber.ToString()) +
                                " has an out of range timestamp difference of " +
                                string.Format("{0,18}", theTimestampDifference.ToString()) +
                                " ms");
                        }
                    }
                    else if (theTimestampDifference >= Constants.MinMiddleRangeTimestampDifference &&
                        theTimestampDifference <= Constants.MaxMiddleRangeTimestampDifference)
                    {
                        if (!theMiddleRangeTimestampHistogram.AddValue(theTimestampDifference))
                        {
                            theOutOfRangeTimestamps.Add(
                                "The time-supplying message with packet number " +
                                string.Format("{0,7}", thePacketNumber.ToString()) +
                                " has an out of range timestamp difference of " +
                                string.Format("{0,18}", theTimestampDifference.ToString()) +
                                " ms");
                        }
                    }
                    else if (theTimestampDifference >= Constants.MinUpperRangeTimestampDifference &&
                        theTimestampDifference <= Constants.MaxUpperRangeTimestampDifference)
                    {
                        if (!theUpperRangeTimestampHistogram.AddValue(theTimestampDifference))
                        {
                            theOutOfRangeTimestamps.Add(
                                "The time-supplying message with packet number " +
                                string.Format("{0,7}", thePacketNumber.ToString()) +
                                " has an out of range timestamp difference of " +
                                string.Format("{0,18}", theTimestampDifference.ToString()) +
                                " ms");
                        }
                    }
                    else
                    {
                        theOutOfRangeTimestamps.Add(
                            "The time-supplying message with packet number " +
                            string.Format("{0,7}", thePacketNumber.ToString()) +
                            " has an out of range timestamp difference of " +
                            string.Format("{0,18}", theTimestampDifference.ToString()) +
                            " ms");
                    }

                    if (theMinTimestampDifference > theTimestampDifference)
                    {
                        theMinTimestampDifference = theTimestampDifference;
                        theMinTimestampDifferencePacketNumber = thePacketNumber;
                    }

                    if (theMaxTimestampDifference < theTimestampDifference)
                    {
                        theMaxTimestampDifference = theTimestampDifference;
                        theMaxTimestampDifferencePacketNumber = thePacketNumber;
                    }

                    // Keep a record of the last timestamp received (from this time-supplying
                    // message) to allow comparison with the next timestamp received
                    theLastTimestamp = thePacketTimestamp;

                    //// The time

                    double theTimeDifference =
                        (thePacketTime - theLastTime) * 1000.0; // Milliseconds

                    //// Keep a running total of the time differences to allow for averaging

                    ++theNumberOfTimeDifferenceInstances;

                    theTotalOfTimeDifferences += theTimeDifference;

                    if (theTimeDifference >= Constants.MinLowerRangeTimeDifference &&
                        theTimeDifference <= Constants.MaxLowerRangeTimeDifference)
                    {
                        if (!theLowerRangeTimeHistogram.AddValue(theTimeDifference))
                        {
                            theOutOfRangeTimes.Add(
                                "The time-supplying message with packet number " +
                                string.Format("{0,7}", thePacketNumber.ToString()) +
                                " has an out of range time difference of " +
                                string.Format("{0,18}", theTimeDifference.ToString()) +
                                " ms");
                        }
                    }
                    else if (theTimeDifference >= Constants.MinMiddleRangeTimeDifference &&
                        theTimeDifference <= Constants.MaxMiddleRangeTimeDifference)
                    {
                        if (!theMiddleRangeTimeHistogram.AddValue(theTimeDifference))
                        {
                            theOutOfRangeTimes.Add(
                                "The time-supplying message with packet number " +
                                string.Format("{0,7}", thePacketNumber.ToString()) +
                                " has an out of range time difference of " +
                                string.Format("{0,18}", theTimeDifference.ToString()) +
                                " ms");
                        }
                    }
                    else if (theTimeDifference >= Constants.MinUpperRangeTimeDifference &&
                        theTimeDifference <= Constants.MaxUpperRangeTimeDifference)
                    {
                        if (!theUpperRangeTimeHistogram.AddValue(theTimeDifference))
                        {
                            theOutOfRangeTimes.Add(
                                "The time-supplying message with packet number " +
                                string.Format("{0,7}", thePacketNumber.ToString()) +
                                " has an out of range time difference of " +
                                string.Format("{0,18}", theTimeDifference.ToString()) +
                                " ms");
                        }
                    }
                    else
                    {
                        theOutOfRangeTimes.Add(
                            "The time-supplying message with packet number " +
                            string.Format("{0,7}", thePacketNumber.ToString()) +
                            " has an out of range time difference of " +
                            string.Format("{0,18}", theTimeDifference.ToString()) +
                            " ms");
                    }

                    if (theMinTimeDifference > theTimeDifference)
                    {
                        theMinTimeDifference = theTimeDifference;
                        theMinTimeDifferencePacketNumber = thePacketNumber;
                    }

                    if (theMaxTimeDifference < theTimeDifference)
                    {
                        theMaxTimeDifference = theTimeDifference;
                        theMaxTimeDifferencePacketNumber = thePacketNumber;
                    }

                    // Keep a record of the last time received (from this time-supplying
                    // message) to allow comparison with the next time received
                    theLastTime = thePacketTime;
                }
            }

            this.theDebugInformation.WriteTextLine(
                "The number of time-supplying messages was " +
                theNumberOfMessages.ToString());

            if (theNumberOfIgnoredMessages > 0)
            {
                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "The number of those time-supplying messages that were ignored and discarded was " +
                    theNumberOfIgnoredMessages.ToString());
            }

            if (theNumberOfTimestampDifferenceInstances > 0)
            {
                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    new string('-', 144));

                this.theDebugInformation.WriteBlankLine();

                theAverageTimestampDifference =
                    theTotalOfTimestampDifferences /
                    theNumberOfTimestampDifferenceInstances;

                theAverageTimeDifference =
                    theTotalOfTimeDifferences /
                    theNumberOfTimeDifferenceInstances;

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

                if (theAverageTimestampDifference > 0)
                {
                    this.theDebugInformation.WriteBlankLine();

                    double theMessageRate =
                        1.0 /
                        (theAverageTimestampDifference / 1000.0); // Hertz

                    this.theDebugInformation.WriteTextLine(
                        "The average rate for the time-supplying messages was " +
                        string.Format("{0,18}", theMessageRate.ToString()) +
                        " Hz");
                }

                if (this.outputHistograms)
                {
                    //// Output the histogram

                    this.theDebugInformation.WriteBlankLine();

                    this.theDebugInformation.WriteTextLine(
                        "The histogram (" +
                        Constants.TimestampBinsPerMillisecond.ToString() +
                        " bins per millisecond) for the timestamp differences is:");

                    this.theDebugInformation.WriteBlankLine();

                    theLowerRangeTimestampHistogram.OutputValues();
                    theMiddleRangeTimestampHistogram.OutputValues();
                    theUpperRangeTimestampHistogram.OutputValues();
                }

                if (theOutOfRangeTimestamps.Any())
                {
                    this.theDebugInformation.WriteBlankLine();

                    // Output the data for any time-supplying message with an out of range timestamp difference
                    foreach (string theString in theOutOfRangeTimestamps)
                    {
                        this.theDebugInformation.WriteTextLine(theString);
                    }
                }
            }

            if (theNumberOfTimeDifferenceInstances > 0)
            {
                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    new string('-', 144));

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

                if (this.outputHistograms)
                {
                    //// Output the histogram

                    this.theDebugInformation.WriteBlankLine();

                    this.theDebugInformation.WriteTextLine(
                        "The histogram (" +
                        Constants.TimeBinsPerMillisecond.ToString() +
                        " bins per millisecond) for the time differences is:");

                    this.theDebugInformation.WriteBlankLine();

                    theLowerRangeTimeHistogram.OutputValues();
                    theMiddleRangeTimeHistogram.OutputValues();
                    theUpperRangeTimeHistogram.OutputValues();
                }

                if (theOutOfRangeTimes.Any())
                {
                    this.theDebugInformation.WriteBlankLine();

                    // Output the data for any time-supplying message with an out of range time difference
                    foreach (string theString in theOutOfRangeTimes)
                    {
                        this.theDebugInformation.WriteTextLine(theString);
                    }
                }
            }

            if (theIgnoredTimestamps.Any())
            {
                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    new string('-', 144));

                this.theDebugInformation.WriteBlankLine();

                // Output the data for any time-supplying messages that were ignored and discarded
                foreach (string theString in theIgnoredTimestamps)
                {
                    this.theDebugInformation.WriteTextLine(theString);
                }
            }

            this.theDebugInformation.WriteBlankLine();

            this.theDebugInformation.WriteTextLine(
                new string('-', 144));

            this.theDebugInformation.WriteBlankLine();

            if (this.outputAdditionalInformation &&
                theNumberOfTimestampDifferenceInstances > 0 &&
                theNumberOfTimeDifferenceInstances > 0)
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

                theOutputAdditionalInformationLines.Append(
                    theOutputAdditionalInformationTitleLine);

                double? thePreviousPacketTimestamp = null;
                double? thePreviousPacketTime = null;

                //// Add a line to the debug output file for each of the processed time-supplying messages

                foreach (System.Data.DataRow theTimeValuesRow in theTimeValuesRowsFound)
                {
                    ulong thePacketNumber = theTimeValuesRow.Field<ulong>("PacketNumber");
                    double thePacketTimestamp = theTimeValuesRow.Field<double>("PacketTimestamp");
                    double thePacketTime = theTimeValuesRow.Field<double>("PacketTime");

                    if (theTimeValuesRow.Field<bool>("Processed"))
                    {
                        string theOutputAdditionalInformationLine = null;

                        if (thePreviousPacketTimestamp == null ||
                            thePreviousPacketTime == null)
                        {
                            // If this is the first time-supplying message then there is no previous time-supplying message and so just output the timestamp and time
                            theOutputAdditionalInformationLine = string.Format(
                                "{0},{1,18},{2,18}{3}",
                                thePacketNumber.ToString(),
                                thePacketTimestamp.ToString(),
                                thePacketTime.ToString(),
                                System.Environment.NewLine);
                        }
                        else
                        {
                            double theTimestampDifference =
                                (thePacketTimestamp - (double)thePreviousPacketTimestamp) * 1000.0; // Milliseconds

                            double theTimeDifference =
                                (thePacketTime - (double)thePreviousPacketTime) * 1000.0; // Milliseconds

                            // If this is another time-supplying message then also calculate the differences in timestamp and time from the previous time-supplying message and output them along with the timestamp and time
                            theOutputAdditionalInformationLine = string.Format(
                                "{0},{1,18},{2,18},,{3,18},{4,18}{5}",
                                thePacketNumber.ToString(),
                                thePacketTimestamp.ToString(),
                                thePacketTime.ToString(),
                                theTimestampDifference,
                                theTimeDifference,
                                System.Environment.NewLine);
                        }

                        thePreviousPacketTimestamp = thePacketTimestamp;
                        thePreviousPacketTime = thePacketTime;

                        theOutputAdditionalInformationLines.Append(
                            theOutputAdditionalInformationLine);
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
