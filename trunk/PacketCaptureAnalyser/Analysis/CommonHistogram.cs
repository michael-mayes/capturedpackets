//This file is part of the C# Packet Capture application. It is free and
//unencumbered software released into the public domain as detailed in
//the UNLICENSE file in the top level directory of this distribution

//$Id$
//$URL      : https://svn.code.sf.net/p/capturedpackets/code/trunk/PacketCaptureAnalyser/Analysis/CommonHistogram.cs $
//$Revision : 214 $
//$Date     : 2014-04-17 19:11:24 +0100 (Thu, 17 Apr 2014) $
//$Author   : michaelmayes $

namespace Analysis
{
    class CommonHistogram
    {
        //
        //Private entities
        //

        private Analysis.DebugInformation TheDebugInformation;

        private double[] TheValueBinBoundaries;
        private uint[] TheValueBinCounts;

        private uint TheNumberOfValuesAcrossAllBins = 0;

        private uint TheNumberOfValuesLowerThanBins = 0;
        private uint TheNumberOfValuesHigherThanBins = 0;

        private double TheMinValueEncountered = double.MaxValue;
        private double TheMaxValueEncountered = double.MinValue;

        //
        //Public constructor method
        //

        public CommonHistogram(Analysis.DebugInformation TheDebugInformation, uint TheNumOfValueBins, double TheMinAllowedValue, double TheMaxAllowedValue)
        {
            this.TheDebugInformation = TheDebugInformation;

            if (TheMinAllowedValue == TheMaxAllowedValue)
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The minimum and maximum allowed values for the histogram are equal!!!"
                    );

                throw new System.ArgumentException
                    (
                    "Error: " +
                    "The minimum and maximum allowed values for the histogram are equal!!!"
                    );
            }

            TheValueBinCounts = new uint[TheNumOfValueBins];

            if (TheMaxAllowedValue > TheMinAllowedValue)
            {
                CalculateValueBinBoundaries(TheNumOfValueBins, TheMinAllowedValue, TheMaxAllowedValue);
            }
            else
            {
                TheDebugInformation.WriteErrorEvent
                    (
                    "The minimum value is greater than the maximum value!"
                    );

                CalculateValueBinBoundaries(TheNumOfValueBins, TheMaxAllowedValue, TheMinAllowedValue);
            }
        }

        //
        //Public accessor methods
        //

        public double[] GetValueBinBoundaries()
        {
            return TheValueBinBoundaries;
        }

        public uint[] GetValueBinCounts()
        {
            return TheValueBinCounts;
        }

        public uint GetValueBinCount(uint TheValueBinNumber)
        {
            return TheValueBinCounts[TheValueBinNumber];
        }

        public int GetNumberOfValueBins()
        {
            return TheValueBinCounts.Length;
        }

        public double GetMinAllowedValue()
        {
            return TheValueBinBoundaries[0];
        }

        public double GetMaxAllowedValue()
        {
            return TheValueBinBoundaries[TheValueBinBoundaries.Length - 1];
        }

        public uint GetTheNumberOfValuesAcrossAllBins()
        {
            return TheNumberOfValuesAcrossAllBins;
        }

        public uint GetNumberOfValuesLowerThanBins()
        {
            return TheNumberOfValuesLowerThanBins;
        }

        public uint GetNumberOfValuesHigherThanBins()
        {
            return TheNumberOfValuesHigherThanBins;
        }

        //
        //Private methods
        //

        private void CalculateValueBinBoundaries(uint TheNumOfValueBins, double TheMinAllowedValue, double TheMaxAllowedValue)
        {
            TheValueBinBoundaries = new double[TheNumOfValueBins + 1];

            TheValueBinBoundaries[0] = TheMinAllowedValue;

            TheValueBinBoundaries[TheValueBinBoundaries.Length - 1] = TheMaxAllowedValue;

            double TheSizeOfValueBins = (TheMaxAllowedValue - TheMinAllowedValue) / TheNumOfValueBins;

            for (int i = 1; i < TheValueBinBoundaries.Length - 1; ++i)
            {
                TheValueBinBoundaries[i] = TheValueBinBoundaries[0] + i * TheSizeOfValueBins;
            }

            //Check that the calculated bin boundaries are a strictly monotonically increasing sequence of values
            CheckBinBoundaries(this.TheValueBinBoundaries);
        }

        //Check that the supplied bin boundaries are a strictly monotonically increasing sequence of values
        private void CheckBinBoundaries(double[] TheBinBoundaries)
        {
            for (int i = 0; i < TheBinBoundaries.Length - 1; ++i)
            {
                if (TheBinBoundaries[i] >= TheBinBoundaries[i + 1])
                {
                    string TheExceptionMessage =
                        "Bin boundary " +
                        TheBinBoundaries[i].ToString() +
                        " is >= Bin boundary " +
                        TheBinBoundaries[i + 1].ToString() +
                        "!!!";

                    TheDebugInformation.WriteErrorEvent
                        (
                        TheExceptionMessage
                        );

                    throw new System.ArgumentException(TheExceptionMessage);
                }
            }
        }

        public void AddValue(double TheValue)
        {
            //Check if the value is the lowest valid value encountered since the creation or reset of the histogram
            if (TheMinValueEncountered > TheValue &&
                TheValue >= GetMinAllowedValue())
            {
                TheMinValueEncountered = TheValue;
            }

            //Check if the value is the highest valid value encountered since the creation or reset of the histogram
            if (TheMaxValueEncountered < TheValue &&
                TheValue <= GetMaxAllowedValue())
            {
                TheMaxValueEncountered = TheValue;
            }

            if (TheValue < GetMinAllowedValue())
            {
                ++TheNumberOfValuesLowerThanBins;
            }
            else if (TheValue > GetMaxAllowedValue())
            {
                ++TheNumberOfValuesHigherThanBins;
            }
            else
            {
                //Loop while the supplied value is smaller than the next bin boundary
                //Once the supplied value is no longer smaller than the next bin boundary then we've found our bin
                //This ordering is more efficient when most supplied values are towards the lower end of the range
                for (int i = 0; i < TheValueBinBoundaries.Length; ++i)
                {
                    if (TheValue < TheValueBinBoundaries[i + 1])
                    {
                        ++TheValueBinCounts[i];
                        break;
                    }
                }

                //Increment the counter of the number of valid values encountered
                ++TheNumberOfValuesAcrossAllBins;
            }
        }

        public void ResetValues()
        {
            TheNumberOfValuesAcrossAllBins = 0;

            TheNumberOfValuesLowerThanBins = 0;
            TheNumberOfValuesHigherThanBins = 0;

            TheMinValueEncountered = double.MaxValue;
            TheMaxValueEncountered = double.MinValue;

            for (int i = 0; i < TheValueBinCounts.Length; ++i)
            {
                TheValueBinCounts[i] = 0;
            }
        }

        //Format the contents of the histogram and output it to the debug console
        public void OutputValues()
        {
            uint TheNumberOfValuesProcessed = 0;

            bool TheFirstPercentileFound = false;
            bool TheNinetyNinthPercentileFound = false;

            if (TheNumberOfValuesLowerThanBins > 0)
            {
                TheDebugInformation.WriteTextLine
                    (
                    "Number of values lower than bins: " +
                    TheNumberOfValuesLowerThanBins.ToString()
                    );

                TheDebugInformation.WriteBlankLine();
            }

            for (int i = 0; i < TheValueBinCounts.Length; ++i)
            {
                //Do not start processing bins for the histogram until we've reached the minimum value encountered
                if (TheValueBinBoundaries[i + 1] < TheMinValueEncountered)
                {
                    continue;
                }

                //Update the running total of entries found so far
                TheNumberOfValuesProcessed += TheValueBinCounts[i];

                //Output an indication if the first percentile of all entries encountered has been reached
                if (!TheFirstPercentileFound)
                {
                    if (TheNumberOfValuesProcessed >= (TheNumberOfValuesAcrossAllBins * 0.01))
                    {
                        TheFirstPercentileFound = true;

                        TheDebugInformation.WriteTextLine(new System.String('-', 144) + "  1%");
                    }
                }

                //Correct the formatting for negative values

                if (TheValueBinBoundaries[i] >= 0.0)
                {
                    TheDebugInformation.WriteTextElement(TheValueBinBoundaries[i].ToString(" 00.00000"));

                }
                else
                {
                    TheDebugInformation.WriteTextElement(TheValueBinBoundaries[i].ToString("00.00000"));
                }

                TheDebugInformation.WriteTextElement(" to ");

                if (TheValueBinBoundaries[i + 1] >= 0.0)
                {
                    TheDebugInformation.WriteTextElement(TheValueBinBoundaries[i + 1].ToString(" 00.00000"));
                }
                else
                {
                    TheDebugInformation.WriteTextElement(TheValueBinBoundaries[i + 1].ToString("00.00000"));
                }

                TheDebugInformation.WriteTextElement(" | ");

                //Calculated a scaled count for this bin based on the percentage of the total number of values across all bins that is in this bin
                //The scaling of the count will the ensure that the output does not exceed 120 columns to ensure it fits on screen
                //Perform the calculations using floating point values to prevent rounding to zero due to integer division
                int ScaledBinCount = (int)(((float)TheValueBinCounts[i] / (float)TheNumberOfValuesAcrossAllBins) * 120.0);

                //Make sure that at least a single ) character is always output for a bin with a non-zero count
                if (TheValueBinCounts[i] > 0 && ScaledBinCount == 0)
                {
                    ScaledBinCount = 1;
                }

                //Output a number of ) characters for this bin based on the scaled count
                TheDebugInformation.WriteTextElement(new System.String(')', ScaledBinCount));

                //Except if there are no entries, leave a space after the last ) character
                //for this bin for clarity and then write out the number of entries in this
                //bin (the real value, not the scaled value)
                if (TheValueBinCounts[i] > 0)
                {
                    TheDebugInformation.WriteTextElement(" " + TheValueBinCounts[i]);
                }

                //Complete the line for this bin
                TheDebugInformation.WriteBlankLine();

                //Output an indication if the ninety ninth percentile of all entries encountered has been reached
                if (!TheNinetyNinthPercentileFound)
                {
                    if (TheNumberOfValuesProcessed >= (TheNumberOfValuesAcrossAllBins * 0.99))
                    {
                        TheNinetyNinthPercentileFound = true;

                        TheDebugInformation.WriteTextLine(new System.String('-', 144) + " 99%");
                    }
                }

                //Do not continue processing further bins for the histogram if we've reached the maximum value encountered
                if (TheValueBinBoundaries[i + 1] > TheMaxValueEncountered)
                {
                    break;
                }
            }

            if (TheNumberOfValuesHigherThanBins > 0)
            {
                TheDebugInformation.WriteBlankLine();

                TheDebugInformation.WriteTextLine
                    (
                    "Number of values higher than bins: " +
                    TheNumberOfValuesHigherThanBins.ToString()
                    );
            }
        }
    }
}
