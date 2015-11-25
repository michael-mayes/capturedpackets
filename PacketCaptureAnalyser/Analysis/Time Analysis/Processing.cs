// <copyright file="Processing.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture Analyser application. It is
// free and unencumbered software released into the public domain as detailed
// in the UNLICENSE file in the top level directory of this distribution

namespace PacketCaptureAnalyzer.Analysis.TimeAnalysis
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
        /// Boolean flag that indicates whether to output the histogram for timestamps
        /// </summary>
        private bool outputTimestampHistograms;

        /// <summary>
        /// Boolean flag that indicates whether to output the histogram for times
        /// </summary>
        private bool outputTimeHistograms;

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
        /// <param name="outputTimestampHistograms">Boolean flag that indicates whether to output the histogram for timestamps</param>
        /// <param name="outputTimeHistograms">Boolean flag that indicates whether to output the histogram for times</param>
        /// <param name="outputAdditionalInformation">Boolean flag that indicates whether to output additional information</param>
        /// <param name="theSelectedPacketCaptureFile">The path of the selected packet capture</param>
        public Processing(Analysis.DebugInformation theDebugInformation, bool outputTimestampHistograms, bool outputTimeHistograms, bool outputAdditionalInformation, string theSelectedPacketCaptureFile)
        {
            this.theDebugInformation = theDebugInformation;

            this.outputTimestampHistograms = outputTimestampHistograms;

            this.outputTimeHistograms = outputTimeHistograms;

            this.outputAdditionalInformation = outputAdditionalInformation;

            this.theSelectedPacketCaptureFile = theSelectedPacketCaptureFile;

            // Create a datatable to hold the timestamp and time values for time-supplying messages
            this.theTimeValuesTable = new System.Data.DataTable();
            this.theTimeValuesTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

            // Create a datatable to hold the set of host Ids encountered during the time analysis
            this.theHostIdsTable = new System.Data.DataTable();
            this.theHostIdsTable.Locale = System.Globalization.CultureInfo.InvariantCulture;
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
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,3}", theHostIdRow.Field<byte>("HostId").ToString(System.Globalization.CultureInfo.CurrentCulture)));

                this.theDebugInformation.WriteTextLine("===========");
                this.theDebugInformation.WriteBlankLine();

                EnumerableRowCollection<System.Data.DataRow>
                    theTimeValuesRowsFound =
                    from r in this.theTimeValuesTable.AsEnumerable()
                    where r.Field<byte>("HostId") == theHostIdRow.Field<byte>("HostId")
                    select r;

                this.FinaliseTimestamps(
                    theTimeValuesRowsFound);

                this.FinaliseTimes(
                    theTimeValuesRowsFound);

                // Output additional information for the supplied type of message
                this.OutputAdditionalInformation(
                    theHostIdRow.Field<byte>("HostId"),
                    theTimeValuesRowsFound);
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

            theHostIdRowFindObject[0] = theHostId.ToString(System.Globalization.CultureInfo.CurrentCulture); // Primary key

            System.Data.DataRow theHostIdDataRowFound =
                this.theHostIdsTable.Rows.Find(
                theHostIdRowFindObject);

            if (theHostIdDataRowFound == null)
            {
                this.theDebugInformation.WriteInformationEvent(
                    "Found a time-supplying message for a Host Id " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,3}", theHostId) +
                    " - adding this Host Id to the time analysis");

                System.Data.DataRow theHostIdRowToAdd =
                    this.theHostIdsTable.NewRow();

                theHostIdRowToAdd["HostId"] = theHostId;

                this.theHostIdsTable.Rows.Add(
                    theHostIdRowToAdd);
            }
        }

        /// <summary>
        /// Finalize timestamp analysis for the supplied data table rows
        /// </summary>
        /// <param name="theTimeValuesRowsFound">The data table rows for which timestamp analysis is to be finalized</param>
        private void FinaliseTimestamps(EnumerableRowCollection<System.Data.DataRow> theTimeValuesRowsFound)
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

            ulong theNumberOfMessages = 0;

            double theMinTimestampDifference = double.MaxValue;
            double theMaxTimestampDifference = double.MinValue;

            ulong theMinTimestampDifferencePacketNumber = 0;
            ulong theMaxTimestampDifferencePacketNumber = 0;

            double theLastTimestamp = 0.0;

            ulong theNumberOfTimestampDifferenceInstances = 0;
            double theTotalOfTimestampDifferences = 0;

            bool theFirstRowProcessed = false;

            System.Collections.Generic.List<string> theIgnoredTimestamps =
                new System.Collections.Generic.List<string>();

            System.Collections.Generic.List<string> theOutOfRangeTimestamps =
                new System.Collections.Generic.List<string>();

            foreach (System.Data.DataRow theTimeValuesRow in theTimeValuesRowsFound)
            {
                // Keep a running total of the number of time-supplying messages
                ++theNumberOfMessages;

                ulong thePacketNumber = theTimeValuesRow.Field<ulong>("PacketNumber");
                double thePacketTimestamp = theTimeValuesRow.Field<double>("PacketTimestamp");

                // Do not calculate the differences in timestamp and time for first row - just record values and move on to second row
                if (!theFirstRowProcessed)
                {
                    theLastTimestamp = thePacketTimestamp;

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
                    theIgnoredTimestamps.Add(
                        "The time-supplying message with packet number " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " has an absolute timestamp difference of " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theAbsoluteTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " ms which is below the minimum value of " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,5}", Constants.MinAbsoluteTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " ms so it has been ignored and discarded");
                }
                else if (theAbsoluteTimestampDifference > Constants.MaxAbsoluteTimestampDifference)
                {
                    theIgnoredTimestamps.Add(
                        "The time-supplying message with packet number " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " has an absolute timestamp difference of " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theAbsoluteTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " ms which is above the maximum value of " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,5}", Constants.MaxAbsoluteTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " ms so it has been ignored and discarded");

                    // Keep a record of the last timestamp received (from this time-supplying
                    // message) to allow comparison with the next timestamp received
                    theLastTimestamp = thePacketTimestamp;
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
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                                " has an out of range timestamp difference of " +
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
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
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                                " has an out of range timestamp difference of " +
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
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
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                                " has an out of range timestamp difference of " +
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                                " ms");
                        }
                    }
                    else
                    {
                        theOutOfRangeTimestamps.Add(
                            "The time-supplying message with packet number " +
                            string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                            " has an out of range timestamp difference of " +
                            string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
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
                }
            }

            this.theDebugInformation.WriteTextLine(
                "The number of time-supplying messages was " +
                theNumberOfMessages.ToString(System.Globalization.CultureInfo.CurrentCulture));

            // Output information, and potentially a histogram, for the timestamp differences
            this.OutputTimestampDifferences(
                theNumberOfTimestampDifferenceInstances,
                theTotalOfTimestampDifferences,
                theMinTimestampDifference,
                theMaxTimestampDifference,
                theMinTimestampDifferencePacketNumber,
                theMaxTimestampDifferencePacketNumber,
                theLowerRangeTimestampHistogram,
                theMiddleRangeTimestampHistogram,
                theUpperRangeTimestampHistogram);

            if (theOutOfRangeTimestamps.Any())
            {
                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    new string('-', 144));

                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "The number of those time-supplying messages that had an out of range timestamp difference was " +
                    theOutOfRangeTimestamps.Count().ToString(System.Globalization.CultureInfo.CurrentCulture));

                this.theDebugInformation.WriteBlankLine();

                // Output the data for any time-supplying message with an out of range timestamp difference
                foreach (string theString in theOutOfRangeTimestamps)
                {
                    this.theDebugInformation.WriteTextLine(theString);
                }
            }

            if (theIgnoredTimestamps.Any())
            {
                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    new string('-', 144));

                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "The number of those time-supplying messages that were ignored and discarded was " +
                    theIgnoredTimestamps.Count().ToString(System.Globalization.CultureInfo.CurrentCulture));

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
        }

        /// <summary>
        /// Finalize time analysis for the supplied data table rows
        /// </summary>
        /// <param name="theTimeValuesRowsFound">The data table rows for which time analysis is to be finalized</param>
        private void FinaliseTimes(EnumerableRowCollection<System.Data.DataRow> theTimeValuesRowsFound)
        {
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

            double theMinTimeDifference = double.MaxValue;
            double theMaxTimeDifference = double.MinValue;

            ulong theMinTimeDifferencePacketNumber = 0;
            ulong theMaxTimeDifferencePacketNumber = 0;

            double theLastTimestamp = 0.0;
            double theLastTime = 0.0;

            ulong theNumberOfTimeDifferenceInstances = 0;
            double theTotalOfTimeDifferences = 0;

            bool theFirstRowProcessed = false;

            System.Collections.Generic.List<string> theOutOfRangeTimes =
                new System.Collections.Generic.List<string>();

            foreach (System.Data.DataRow theTimeValuesRow in theTimeValuesRowsFound)
            {
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
                    // No functionality required in this case for the time analysis
                }
                else if (theAbsoluteTimestampDifference > Constants.MaxAbsoluteTimestampDifference)
                {
                    // Keep a record of the last timestamp received (from this time-supplying
                    // message) to allow comparison with the next timestamp received
                    theLastTimestamp = thePacketTimestamp;

                    // Keep a record of the last time received (from this time-supplying
                    // message) to allow comparison with the next times received
                    theLastTime = thePacketTime;
                }
                else
                {
                    // Mark this time-supplying message as processed as its timestamp difference is within the chosen range
                    theTimeValuesRow["Processed"] = true;

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
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                                " has an out of range time difference of " +
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theTimeDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
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
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                                " has an out of range time difference of " +
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theTimeDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
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
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                                " has an out of range time difference of " +
                                string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theTimeDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                                " ms");
                        }
                    }
                    else
                    {
                        theOutOfRangeTimes.Add(
                            "The time-supplying message with packet number " +
                            string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                            " has an out of range time difference of " +
                            string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theTimeDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
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

                    // Keep a record of the last timestamp received (from this time-supplying
                    // message) to allow comparison with the next timestamp received
                    theLastTimestamp = thePacketTimestamp;

                    // Keep a record of the last time received (from this time-supplying
                    // message) to allow comparison with the next time received
                    theLastTime = thePacketTime;
                }
            }

            // Output information, and potentially a histogram, for the times differences
            this.OutputTimeDifferences(
                theNumberOfTimeDifferenceInstances,
                theTotalOfTimeDifferences,
                theMinTimeDifference,
                theMaxTimeDifference,
                theMinTimeDifferencePacketNumber,
                theMaxTimeDifferencePacketNumber,
                theLowerRangeTimeHistogram,
                theMiddleRangeTimeHistogram,
                theUpperRangeTimeHistogram);

            if (theOutOfRangeTimes.Any())
            {
                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    new string('-', 144));

                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "The number of those time-supplying messages that had an out of range time difference was " +
                    theOutOfRangeTimes.Count().ToString(System.Globalization.CultureInfo.CurrentCulture));

                if (this.outputAdditionalInformation)
                {
                    this.theDebugInformation.WriteBlankLine();

                    // Output the data for any time-supplying message with an out of range time difference
                    foreach (string theString in theOutOfRangeTimes)
                    {
                        this.theDebugInformation.WriteTextLine(theString);
                    }
                }
            }

            this.theDebugInformation.WriteBlankLine();

            this.theDebugInformation.WriteTextLine(
                new string('-', 144));

            this.theDebugInformation.WriteBlankLine();
        }

        /// <summary>
        /// Output information, and potentially a histogram, for the timestamp differences
        /// </summary>
        /// <param name="theNumberOfTimestampDifferenceInstances">The number of timestamp differences</param>
        /// <param name="theTotalOfTimestampDifferences">The total of all the timestamp differences</param>
        /// <param name="theMinTimestampDifference">The minimum timestamp difference</param>
        /// <param name="theMaxTimestampDifference">The maximum timestamp difference</param>
        /// <param name="theMinTimestampDifferencePacketNumber">The packet number with the minimum timestamp difference</param>
        /// <param name="theMaxTimestampDifferencePacketNumber">The packet number with the maximum timestamp difference</param>
        /// <param name="theLowerRangeTimestampHistogram">The histogram for the lower range of timestamp differences</param>
        /// <param name="theMiddleRangeTimestampHistogram">The histogram for the middle range of timestamp differences</param>
        /// <param name="theUpperRangeTimestampHistogram">The histogram for the upper range of timestamp differences</param>
        private void OutputTimestampDifferences(ulong theNumberOfTimestampDifferenceInstances, double theTotalOfTimestampDifferences, double theMinTimestampDifference, double theMaxTimestampDifference, ulong theMinTimestampDifferencePacketNumber, ulong theMaxTimestampDifferencePacketNumber, CommonHistogram theLowerRangeTimestampHistogram, CommonHistogram theMiddleRangeTimestampHistogram, CommonHistogram theUpperRangeTimestampHistogram)
        {
            if (theNumberOfTimestampDifferenceInstances > 0)
            {
                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    new string('-', 144));

                this.theDebugInformation.WriteBlankLine();

                double theAverageTimestampDifference =
                    theTotalOfTimestampDifferences /
                    theNumberOfTimestampDifferenceInstances;

                this.theDebugInformation.WriteTextLine(
                    "The minimum timestamp difference was " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theMinTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " ms for packet number " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theMinTimestampDifferencePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)));

                this.theDebugInformation.WriteTextLine(
                    "The maximum timestamp difference was " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theMaxTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " ms for packet number " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theMaxTimestampDifferencePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)));

                this.theDebugInformation.WriteTextLine(
                    "The average timestamp difference was " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theAverageTimestampDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " ms");

                if (theAverageTimestampDifference > 0)
                {
                    this.theDebugInformation.WriteBlankLine();

                    double theMessageRate =
                        1.0 /
                        (theAverageTimestampDifference / 1000.0); // Hertz

                    this.theDebugInformation.WriteTextLine(
                        "The average rate for the time-supplying messages was " +
                        string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theMessageRate.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                        " Hz");
                }

                if (this.outputTimestampHistograms)
                {
                    //// Output the histogram

                    this.theDebugInformation.WriteBlankLine();

                    this.theDebugInformation.WriteTextLine(
                        new string('-', 144));

                    this.theDebugInformation.WriteBlankLine();

                    this.theDebugInformation.WriteTextLine(
                        "The histogram (" +
                        Constants.TimestampBinsPerMillisecond.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                        " bins per millisecond) for the timestamp differences is:");

                    this.theDebugInformation.WriteBlankLine();

                    theLowerRangeTimestampHistogram.OutputValues();
                    theMiddleRangeTimestampHistogram.OutputValues();
                    theUpperRangeTimestampHistogram.OutputValues();
                }
            }
        }

        /// <summary>
        /// Output information, and potentially a histogram, for the time differences
        /// </summary>
        /// <param name="theNumberOfTimeDifferenceInstances">The number of time differences</param>
        /// <param name="theTotalOfTimeDifferences">The total of all the time differences</param>
        /// <param name="theMinTimeDifference">The minimum time difference</param>
        /// <param name="theMaxTimeDifference">The maximum time difference</param>
        /// <param name="theMinTimeDifferencePacketNumber">The packet number with the minimum time difference</param>
        /// <param name="theMaxTimeDifferencePacketNumber">The packet number with the maximum time difference</param>
        /// <param name="theLowerRangeTimeHistogram">The histogram for the lower range of time differences</param>
        /// <param name="theMiddleRangeTimeHistogram">The histogram for the middle range of time differences</param>
        /// <param name="theUpperRangeTimeHistogram">The histogram for the upper range of time differences</param>
        private void OutputTimeDifferences(ulong theNumberOfTimeDifferenceInstances, double theTotalOfTimeDifferences, double theMinTimeDifference, double theMaxTimeDifference, ulong theMinTimeDifferencePacketNumber, ulong theMaxTimeDifferencePacketNumber, CommonHistogram theLowerRangeTimeHistogram, CommonHistogram theMiddleRangeTimeHistogram, CommonHistogram theUpperRangeTimeHistogram)
        {
            if (theNumberOfTimeDifferenceInstances > 0)
            {
                double theAverageTimeDifference =
                    theTotalOfTimeDifferences /
                    theNumberOfTimeDifferenceInstances;

                this.theDebugInformation.WriteTextLine(
                    "The minimum time difference was " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theMinTimeDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " ms for packet number " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theMinTimeDifferencePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)));

                this.theDebugInformation.WriteTextLine(
                    "The maximum time difference was " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theMaxTimeDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " ms for packet number " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,7}", theMaxTimeDifferencePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture)));

                this.theDebugInformation.WriteTextLine(
                    "The average time difference was " +
                    string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0,19}", theAverageTimeDifference.ToString(System.Globalization.CultureInfo.CurrentCulture)) +
                    " ms");

                if (this.outputTimeHistograms)
                {
                    //// Output the histogram

                    this.theDebugInformation.WriteBlankLine();

                    this.theDebugInformation.WriteTextLine(
                        new string('-', 144));

                    this.theDebugInformation.WriteBlankLine();

                    this.theDebugInformation.WriteTextLine(
                        "The histogram (" +
                        Constants.TimeBinsPerMillisecond.ToString(System.Globalization.CultureInfo.CurrentCulture) +
                        " bins per millisecond) for the time differences is:");

                    this.theDebugInformation.WriteBlankLine();

                    theLowerRangeTimeHistogram.OutputValues();
                    theMiddleRangeTimeHistogram.OutputValues();
                    theUpperRangeTimeHistogram.OutputValues();
                }
            }
        }

        /// <summary>
        /// Outputs additional information for the supplied type of message
        /// </summary>
        /// <param name="theHostId">The host Id for the additional information that is to be output</param>
        /// <param name="theTimeValuesRowsFound">The data table rows for which additional information is to be output</param>
        private void OutputAdditionalInformation(byte theHostId, EnumerableRowCollection<System.Data.DataRow> theTimeValuesRowsFound)
        {
            // Only attempt to output additional information if requested and if
            // there are a non-zero number of instances of timestamp differences
            // and a non-zero number of instances of time differences
            if (this.outputAdditionalInformation)
            {
                System.Text.StringBuilder theOutputAdditionalInformationLines =
                    new System.Text.StringBuilder();

                //// Add a column titles line to the debug output file

                string theOutputAdditionalInformationTitleLine = string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
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
                                System.Globalization.CultureInfo.CurrentCulture,
                                "{0},{1,19},{2,19}{3}",
                                thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture),
                                thePacketTimestamp.ToString(System.Globalization.CultureInfo.CurrentCulture),
                                thePacketTime.ToString(System.Globalization.CultureInfo.CurrentCulture),
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
                                System.Globalization.CultureInfo.CurrentCulture,
                                "{0},{1,19},{2,19},,{3,19},{4,19}{5}",
                                thePacketNumber.ToString(System.Globalization.CultureInfo.CurrentCulture),
                                thePacketTimestamp.ToString(System.Globalization.CultureInfo.CurrentCulture),
                                thePacketTime.ToString(System.Globalization.CultureInfo.CurrentCulture),
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
