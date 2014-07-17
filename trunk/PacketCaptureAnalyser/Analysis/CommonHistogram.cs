// $Id$
// $URL$
// <copyright file="CommonHistogram.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// the UNLICENSE file in the top level directory of this distribution

namespace Analysis
{
    /// <summary>
    /// This class provides common functionality associated with creating, maintaining and outputting histograms
    /// </summary>
    public class CommonHistogram
    {
        //// Private entities

        /// <summary>
        /// The object that provides for the logging of debug information
        /// </summary>
        private Analysis.DebugInformation theDebugInformation;

        /// <summary>
        /// The set of boundaries for the bins in the histogram
        /// </summary>
        private double[] theValueBinBoundaries;

        /// <summary>
        /// The number of values in each bin of the histogram
        /// </summary>
        private uint[] theValueBinCounts;

        /// <summary>
        /// The total number of values across all bins in the histogram
        /// </summary>
        private uint theNumberOfValuesAcrossAllBins = 0;

        /// <summary>
        /// The number of values added to the histogram that had a value lower than the minimum bin boundary
        /// </summary>
        private uint theNumberOfValuesLowerThanBins = 0;

        /// <summary>
        /// The number of values added to the histogram that had a value higher than the maximum bin boundary
        /// </summary>
        private uint theNumberOfValuesHigherThanBins = 0;

        /// <summary>
        /// The minimum value encountered during adding values to the histogram
        /// </summary>
        private double theMinValueEncountered = double.MaxValue;

        /// <summary>
        /// The maximum value encountered during adding values to the histogram
        /// </summary>
        private double theMaxValueEncountered = double.MinValue;

        /// <summary>
        /// Initializes a new instance of the CommonHistogram class
        /// </summary>
        /// <param name="theDebugInformation">The object that provides for the logging of debug information</param>
        /// <param name="theNumOfValueBins">The number of bins to use for the histogram</param>
        /// <param name="theMinAllowedValue">The minimum value allowed to be added to the bins for the histogram</param>
        /// <param name="theMaxAllowedValue">The maximum value allowed to be added to the bins for the histogram</param>
        public CommonHistogram(Analysis.DebugInformation theDebugInformation, uint theNumOfValueBins, double theMinAllowedValue, double theMaxAllowedValue)
        {
            this.theDebugInformation = theDebugInformation;

            if (theMinAllowedValue == theMaxAllowedValue)
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The minimum and maximum allowed values for the histogram are equal!!!");

                throw new System.ArgumentException(
                    "Error: The minimum and maximum allowed values for the histogram are equal!!!");
            }

            this.theValueBinCounts = new uint[theNumOfValueBins];

            if (theMaxAllowedValue > theMinAllowedValue)
            {
                this.CalculateValueBinBoundaries(
                    theNumOfValueBins,
                    theMinAllowedValue,
                    theMaxAllowedValue);
            }
            else
            {
                this.theDebugInformation.WriteErrorEvent(
                    "The minimum value is greater than the maximum value!");

                this.CalculateValueBinBoundaries(
                    theNumOfValueBins,
                    theMaxAllowedValue,
                    theMinAllowedValue);
            }
        }

        //// Public accessor methods

        /// <summary>
        /// Returns the minimum value allowed to be added to the bins for the histogram
        /// </summary>
        /// <returns>The minimum value allowed to be added to the bins for the histogram</returns>
        public double GetMinAllowedValue()
        {
            return this.theValueBinBoundaries[0];
        }

        /// <summary>
        /// Returns the maximum value allowed to be added to the bins for the histogram
        /// </summary>
        /// <returns>The maximum value allowed to be added to the bins for the histogram</returns>
        public double GetMaxAllowedValue()
        {
            return this.theValueBinBoundaries[this.theValueBinBoundaries.Length - 1];
        }

        /// <summary>
        /// Adds the supplied value to appropriate bin in the histogram and updates associated data
        /// </summary>
        /// <param name="theValue">The value to be added to the appropriate bin in the histogram</param>
        /// <returns>Boolean flag that indicates whether the values was allowed to be added to the bins for the histogram</returns>
        public bool AddValue(double theValue)
        {
            bool theResult = true;

            // Check if the value is the lowest valid value encountered since the creation or reset of the histogram
            if (this.theMinValueEncountered > theValue &&
                theValue >= this.GetMinAllowedValue())
            {
                this.theMinValueEncountered = theValue;
            }

            // Check if the value is the highest valid value encountered since the creation or reset of the histogram
            if (this.theMaxValueEncountered < theValue &&
                theValue <= this.GetMaxAllowedValue())
            {
                this.theMaxValueEncountered = theValue;
            }

            if (theValue < this.GetMinAllowedValue())
            {
                ++this.theNumberOfValuesLowerThanBins;

                theResult = false;
            }
            else if (theValue > this.GetMaxAllowedValue())
            {
                ++this.theNumberOfValuesHigherThanBins;

                theResult = false;
            }
            else
            {
                // Loop while the supplied value is smaller than the next bin boundary
                // Once the supplied value is no longer smaller than the next bin boundary then we've found our bin
                // This ordering is more efficient when most supplied values are towards the lower end of the range
                for (int i = 0; i < this.theValueBinBoundaries.Length; ++i)
                {
                    if (theValue < this.theValueBinBoundaries[i + 1])
                    {
                        ++this.theValueBinCounts[i];

                        break;
                    }
                }

                // Increment the counter of the number of valid values encountered
                ++this.theNumberOfValuesAcrossAllBins;
            }

            return theResult;
        }

        /// <summary>
        /// Resets all data and values for the histogram
        /// </summary>
        public void ResetValues()
        {
            this.theNumberOfValuesAcrossAllBins = 0;

            this.theNumberOfValuesLowerThanBins = 0;
            this.theNumberOfValuesHigherThanBins = 0;

            this.theMinValueEncountered = double.MaxValue;
            this.theMaxValueEncountered = double.MinValue;

            for (int i = 0; i < this.theValueBinCounts.Length; ++i)
            {
                this.theValueBinCounts[i] = 0;
            }
        }

        /// <summary>
        /// Formats the contents of the histogram and outputs it to the debug console
        /// </summary>
        public void OutputValues()
        {
            uint theNumberOfValuesProcessed = 0;

            bool theFirstPercentileFound = false;
            bool theNinetyNinthPercentileFound = false;

            for (int i = 0; i < this.theValueBinCounts.Length; ++i)
            {
                // Do not start processing bins for the histogram until we've reached the minimum value encountered
                if (this.theValueBinBoundaries[i + 1] < this.theMinValueEncountered)
                {
                    continue;
                }

                // Update the running total of entries found so far
                theNumberOfValuesProcessed += this.theValueBinCounts[i];

                // Output an indication if the first percentile of all entries encountered has been reached
                if (!theFirstPercentileFound)
                {
                    if (theNumberOfValuesProcessed >= (this.theNumberOfValuesAcrossAllBins * 0.01))
                    {
                        theFirstPercentileFound = true;

                        this.theDebugInformation.WriteTextLine(
                            new string('+', 144) + "  1%");
                    }
                }

                //// Correct the formatting for negative values

                if (this.theValueBinBoundaries[i] >= 0.0)
                {
                    this.theDebugInformation.WriteTextElement(
                        string.Format("{0,11: ###0.00000}", this.theValueBinBoundaries[i]));
                }
                else
                {
                    this.theDebugInformation.WriteTextElement(
                        string.Format("{0,11:###0.00000}", this.theValueBinBoundaries[i]));
                }

                this.theDebugInformation.WriteTextElement(" to ");

                if (this.theValueBinBoundaries[i + 1] >= 0.0)
                {
                    this.theDebugInformation.WriteTextElement(
                        string.Format("{0,11: ###0.00000}", this.theValueBinBoundaries[i + 1]));
                }
                else
                {
                    this.theDebugInformation.WriteTextElement(
                        string.Format("{0,11:###0.00000}", this.theValueBinBoundaries[i + 1]));
                }

                this.theDebugInformation.WriteTextElement(" | ");

                // Calculated a scaled count for this bin based on the percentage of the total number of values across all bins that is in this bin
                // The scaling of the count will the ensure that the output does not exceed 120 columns to ensure it fits on screen
                // Perform the calculations using floating point values to prevent rounding to zero due to integer division
                int theScaledBinCount =
                    (int)(((float)this.theValueBinCounts[i] / (float)this.theNumberOfValuesAcrossAllBins) * 120.0);

                // Make sure that at least a single ) character is always output for a bin with a non-zero count
                if (this.theValueBinCounts[i] > 0 && theScaledBinCount == 0)
                {
                    theScaledBinCount = 1;
                }

                // Output a number of ) characters for this bin based on the scaled count
                this.theDebugInformation.WriteTextElement(
                    new string(')', theScaledBinCount));

                // Except if there are no entries, leave a space after the last ) character
                // for this bin for clarity and then write out the number of entries in this
                // bin (the real value, not the scaled value)
                if (this.theValueBinCounts[i] > 0)
                {
                    this.theDebugInformation.WriteTextElement(
                        " " + this.theValueBinCounts[i]);
                }

                // Complete the line for this bin
                this.theDebugInformation.WriteBlankLine();

                // Output an indication if the ninety ninth percentile of all entries encountered has been reached
                if (!theNinetyNinthPercentileFound)
                {
                    if (theNumberOfValuesProcessed >= (this.theNumberOfValuesAcrossAllBins * 0.99))
                    {
                        theNinetyNinthPercentileFound = true;

                        this.theDebugInformation.WriteTextLine(
                            new string('+', 144) + " 99%");
                    }
                }

                // Do not continue processing further bins for the histogram if we've reached the maximum value encountered
                if (this.theValueBinBoundaries[i + 1] > this.theMaxValueEncountered)
                {
                    break;
                }
            }

            if (this.theNumberOfValuesLowerThanBins > 0)
            {
                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "Number of values lower than bins: " +
                    this.theNumberOfValuesLowerThanBins.ToString());
            }

            if (this.theNumberOfValuesHigherThanBins > 0)
            {
                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine(
                    "Number of values higher than bins: " +
                    this.theNumberOfValuesHigherThanBins.ToString());
            }
        }

        //// Private methods

        /// <summary>
        /// Calculates the bin boundaries to be used for the histogram based on the supplied number of bins, minimum allowed value, and maximum allowed value
        /// </summary>
        /// <param name="theNumOfValueBins">The number of bins to use for the histogram</param>
        /// <param name="theMinAllowedValue">The minimum value allowed to be added to the bins for the histogram</param>
        /// <param name="theMaxAllowedValue">The maximum value allowed to be added to the bins for the histogram</param>
        private void CalculateValueBinBoundaries(uint theNumOfValueBins, double theMinAllowedValue, double theMaxAllowedValue)
        {
            this.theValueBinBoundaries = new double[theNumOfValueBins + 1];

            this.theValueBinBoundaries[0] = theMinAllowedValue;

            this.theValueBinBoundaries[this.theValueBinBoundaries.Length - 1] = theMaxAllowedValue;

            double theSizeOfValueBins =
                (theMaxAllowedValue - theMinAllowedValue) / theNumOfValueBins;

            for (int i = 1; i < this.theValueBinBoundaries.Length - 1; ++i)
            {
                this.theValueBinBoundaries[i] =
                    this.theValueBinBoundaries[0] + (i * theSizeOfValueBins);
            }

            // Check that the calculated bin boundaries are a strictly monotonically increasing sequence of values
            this.CheckBinBoundaries(this.theValueBinBoundaries);
        }

        /// <summary>
        /// Checks that the supplied bin boundaries are a strictly monotonically increasing sequence of values
        /// </summary>
        /// <param name="theBinBoundaries">The bin boundaries to be checked</param>
        private void CheckBinBoundaries(double[] theBinBoundaries)
        {
            for (int i = 0; i < theBinBoundaries.Length - 1; ++i)
            {
                if (theBinBoundaries[i] >= theBinBoundaries[i + 1])
                {
                    string theExceptionMessage =
                        "Bin boundary " +
                        theBinBoundaries[i].ToString() +
                        " is >= Bin boundary " +
                        theBinBoundaries[i + 1].ToString() +
                        "!!!";

                    this.theDebugInformation.WriteErrorEvent(theExceptionMessage);

                    throw new System.ArgumentException(theExceptionMessage);
                }
            }
        }
    }
}
