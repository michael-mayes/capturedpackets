// $Id$
// $URL$
// <copyright file="CommonHistogram.cs" company="Public Domain">
//     Released into the public domain
// </copyright>

// This file is part of the C# Packet Capture application. It is free and
// unencumbered software released into the public domain as detailed in
// The UNLICENSE file in the top level directory of this distribution

namespace Analysis
{
    class CommonHistogram
    {
        //// Private entities 

        private Analysis.DebugInformation theDebugInformation;

        private double[] theValueBinBoundaries;
        private uint[] theValueBinCounts;

        private uint theNumberOfValuesAcrossAllBins = 0;

        private uint theNumberOfValuesLowerThanBins = 0;
        private uint theNumberOfValuesHigherThanBins = 0;

        private double theMinValueEncountered = double.MaxValue;
        private double theMaxValueEncountered = double.MinValue;

        /// <summary>
        /// Initializes a new instance of the CommonHistogram class
        /// </summary>
        /// <param name="theDebugInformation"></param>
        /// <param name="theNumOfValueBins"></param>
        /// <param name="theMinAllowedValue"></param>
        /// <param name="theMaxAllowedValue"></param>
        public CommonHistogram(Analysis.DebugInformation theDebugInformation, uint theNumOfValueBins, double theMinAllowedValue, double theMaxAllowedValue)
        {
            this.theDebugInformation = theDebugInformation;

            if (theMinAllowedValue == theMaxAllowedValue)
            {
                theDebugInformation.WriteErrorEvent("The minimum and maximum allowed values for the histogram are equal!!!");

                throw new System.ArgumentException("Error: The minimum and maximum allowed values for the histogram are equal!!!");
            }

            this.theValueBinCounts = new uint[theNumOfValueBins];

            if (theMaxAllowedValue > theMinAllowedValue)
            {
                this.CalculateValueBinBoundaries(theNumOfValueBins, theMinAllowedValue, theMaxAllowedValue);
            }
            else
            {
                theDebugInformation.WriteErrorEvent("The minimum value is greater than the maximum value!");

                this.CalculateValueBinBoundaries(theNumOfValueBins, theMaxAllowedValue, theMinAllowedValue);
            }
        }

        //// Public accessor methods

        public double[] GetValueBinBoundaries()
        {
            return this.theValueBinBoundaries;
        }

        public uint[] GetValueBinCounts()
        {
            return this.theValueBinCounts;
        }

        public uint GetValueBinCount(uint theValueBinNumber)
        {
            return this.theValueBinCounts[theValueBinNumber];
        }

        public int GetNumberOfValueBins()
        {
            return this.theValueBinCounts.Length;
        }

        public double GetMinAllowedValue()
        {
            return this.theValueBinBoundaries[0];
        }

        public double GetMaxAllowedValue()
        {
            return this.theValueBinBoundaries[this.theValueBinBoundaries.Length - 1];
        }

        public uint GetTheNumberOfValuesAcrossAllBins()
        {
            return this.theNumberOfValuesAcrossAllBins;
        }

        public uint GetNumberOfValuesLowerThanBins()
        {
            return this.theNumberOfValuesLowerThanBins;
        }

        public uint GetNumberOfValuesHigherThanBins()
        {
            return this.theNumberOfValuesHigherThanBins;
        }

        //// Private methods

        private void CalculateValueBinBoundaries(uint theNumOfValueBins, double theMinAllowedValue, double theMaxAllowedValue)
        {
            this.theValueBinBoundaries = new double[theNumOfValueBins + 1];

            this.theValueBinBoundaries[0] = theMinAllowedValue;

            this.theValueBinBoundaries[this.theValueBinBoundaries.Length - 1] = theMaxAllowedValue;

            double theSizeOfValueBins = (theMaxAllowedValue - theMinAllowedValue) / theNumOfValueBins;

            for (int i = 1; i < this.theValueBinBoundaries.Length - 1; ++i)
            {
                this.theValueBinBoundaries[i] = this.theValueBinBoundaries[0] + (i * theSizeOfValueBins);
            }

            // Check that the calculated bin boundaries are a strictly monotonically increasing sequence of values
            this.CheckBinBoundaries(this.theValueBinBoundaries);
        }

        // Check that the supplied bin boundaries are a strictly monotonically increasing sequence of values
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

        public void AddValue(double theValue)
        {
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
            }
            else if (theValue > this.GetMaxAllowedValue())
            {
                ++this.theNumberOfValuesHigherThanBins;
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
        }

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

        // Format the contents of the histogram and output it to the debug console
        public void OutputValues()
        {
            uint theNumberOfValuesProcessed = 0;

            bool theFirstPercentileFound = false;
            bool theNinetyNinthPercentileFound = false;

            if (this.theNumberOfValuesLowerThanBins > 0)
            {
                this.theDebugInformation.WriteTextLine("Number of values lower than bins: " +
                    this.theNumberOfValuesLowerThanBins.ToString());

                this.theDebugInformation.WriteBlankLine();
            }

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

                        this.theDebugInformation.WriteTextLine(new string('-', 144) + "  1%");
                    }
                }

                //// Correct the formatting for negative values

                if (this.theValueBinBoundaries[i] >= 0.0)
                {
                    this.theDebugInformation.WriteTextElement(this.theValueBinBoundaries[i].ToString(" 00.00000"));
                }
                else
                {
                    this.theDebugInformation.WriteTextElement(this.theValueBinBoundaries[i].ToString("00.00000"));
                }

                this.theDebugInformation.WriteTextElement(" to ");

                if (this.theValueBinBoundaries[i + 1] >= 0.0)
                {
                    this.theDebugInformation.WriteTextElement(this.theValueBinBoundaries[i + 1].ToString(" 00.00000"));
                }
                else
                {
                    this.theDebugInformation.WriteTextElement(this.theValueBinBoundaries[i + 1].ToString("00.00000"));
                }

                this.theDebugInformation.WriteTextElement(" | ");

                // Calculated a scaled count for this bin based on the percentage of the total number of values across all bins that is in this bin
                // The scaling of the count will the ensure that the output does not exceed 120 columns to ensure it fits on screen
                // Perform the calculations using floating point values to prevent rounding to zero due to integer division
                int theScaledBinCount = (int)(((float)this.theValueBinCounts[i] / (float)this.theNumberOfValuesAcrossAllBins) * 120.0);

                // Make sure that at least a single ) character is always output for a bin with a non-zero count
                if (this.theValueBinCounts[i] > 0 && theScaledBinCount == 0)
                {
                    theScaledBinCount = 1;
                }

                // Output a number of ) characters for this bin based on the scaled count
                this.theDebugInformation.WriteTextElement(new string(')', theScaledBinCount));

                // Except if there are no entries, leave a space after the last ) character
                // for this bin for clarity and then write out the number of entries in this
                // bin (the real value, not the scaled value)
                if (this.theValueBinCounts[i] > 0)
                {
                    this.theDebugInformation.WriteTextElement(" " + this.theValueBinCounts[i]);
                }

                // Complete the line for this bin
                this.theDebugInformation.WriteBlankLine();

                // Output an indication if the ninety ninth percentile of all entries encountered has been reached
                if (!theNinetyNinthPercentileFound)
                {
                    if (theNumberOfValuesProcessed >= (this.theNumberOfValuesAcrossAllBins * 0.99))
                    {
                        theNinetyNinthPercentileFound = true;

                        this.theDebugInformation.WriteTextLine(new string('-', 144) + " 99%");
                    }
                }

                // Do not continue processing further bins for the histogram if we've reached the maximum value encountered
                if (this.theValueBinBoundaries[i + 1] > this.theMaxValueEncountered)
                {
                    break;
                }
            }

            if (this.theNumberOfValuesHigherThanBins > 0)
            {
                this.theDebugInformation.WriteBlankLine();

                this.theDebugInformation.WriteTextLine("Number of values higher than bins: " +
                    this.theNumberOfValuesHigherThanBins.ToString());
            }
        }
    }
}
