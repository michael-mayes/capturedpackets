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

namespace AnalysisNamespace
{
    public class CommonAnalysisHistogram
    {
        //
        //Private entities
        //

        private double[] TheValueBinBoundaries;
        private int[] TheValueBinCounts;

        private int TheNumberOfValuesLowerThanBins = 0;
        private int TheNumberOfValuesHigherThanBins = 0;

        private double TheMinValueEncountered = double.MaxValue;
        private double TheMaxValueEncountered = double.MinValue;

        //
        //Public constructor method
        //

        public CommonAnalysisHistogram(int TheNumOfValueBins, double TheMinAllowedValue, double TheMaxAllowedValue)
        {
            if (TheMinAllowedValue == TheMaxAllowedValue)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "The minimum and maximum allowed values for the histogram are equal!!!"
                    );

                throw new System.ArgumentException
                    (
                    "The minimum and maximum allowed values for the histogram are equal!!!"
                    );
            }

            TheValueBinCounts = new int[TheNumOfValueBins];

            if (TheMaxAllowedValue > TheMinAllowedValue)
            {
                CalculateValueBinBoundaries(TheNumOfValueBins, TheMinAllowedValue, TheMaxAllowedValue);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine
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

        public int[] GetValueBinCounts()
        {
            return TheValueBinCounts;
        }

        public int GetValueBinCount(int TheValueBinNumber)
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

        public int GetNumberOfValuesLowerThanBins()
        {
            return TheNumberOfValuesLowerThanBins;
        }

        public int GetNumberOfValuesHigherThanBins()
        {
            return TheNumberOfValuesHigherThanBins;
        }

        //
        //Private methods
        //

        private void CalculateValueBinBoundaries(int TheNumOfValueBins, double TheMinAllowedValue, double TheMaxAllowedValue)
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

                    System.Diagnostics.Trace.WriteLine
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
            }
        }

        public void ResetValues()
        {
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
            int i, j;

            if (TheNumberOfValuesLowerThanBins > 0)
            {
                System.Diagnostics.Trace.WriteLine
                    (
                    "Number of values lower than bins: " +
                    TheNumberOfValuesLowerThanBins.ToString()
                    );

                System.Diagnostics.Trace.Write(System.Environment.NewLine);
            }

            for (i = 0; i < TheValueBinCounts.Length; ++i)
            {
                //Do not start processing bins for the histogram until we've reached the minimum value encountered
                if (TheValueBinBoundaries[i + 1] < TheMinValueEncountered)
                {
                    continue;
                }

                //Correct the formatting for negative values

                if (TheValueBinBoundaries[i] >= 0.0)
                {
                    System.Diagnostics.Trace.Write(TheValueBinBoundaries[i].ToString(" 00.00000"));

                }
                else
                {
                    System.Diagnostics.Trace.Write(TheValueBinBoundaries[i].ToString("00.00000"));
                }

                System.Diagnostics.Trace.Write(" to ");

                if (TheValueBinBoundaries[i + 1] >= 0.0)
                {
                    System.Diagnostics.Trace.Write(TheValueBinBoundaries[i + 1].ToString(" 00.00000"));
                }
                else
                {
                    System.Diagnostics.Trace.Write(TheValueBinBoundaries[i + 1].ToString("00.00000"));
                }

                System.Diagnostics.Trace.Write(" | ");

                //Output a single ) character for each entry in this bin
                for (j = 0; j <= TheValueBinCounts[i]; ++j)
                {
                    if (j < TheValueBinCounts[i])
                    {
                        //Truncate the output after 120 columns to ensure it fits on screen
                        if (j < 120)
                        {
                            System.Diagnostics.Trace.Write(')');
                        }
                        else
                        {
                            //Leave a space after the last ) character for this bin for clarity
                            System.Diagnostics.Trace.Write(" ");
                            System.Diagnostics.Trace.Write(TheValueBinCounts[i]);
                            break;
                        }
                    }
                    else
                    {
                        //Leave a space after the last ) character for this bin for clarity except if there are no entries
                        if (j != 0)
                        {
                            System.Diagnostics.Trace.Write(" ");
                        }

                        System.Diagnostics.Trace.Write(TheValueBinCounts[i]);
                        break;
                    }
                }

                //Complete the line for this bin
                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                //Do not continue processing further bins for the histogram if we've reached the maximum value encountered
                if (TheValueBinBoundaries[i + 1] > TheMaxValueEncountered)
                {
                    break;
                }
            }

            if (TheNumberOfValuesHigherThanBins > 0)
            {
                System.Diagnostics.Trace.Write(System.Environment.NewLine);

                System.Diagnostics.Trace.WriteLine
                    (
                    "Number of values higher than bins: " +
                    TheNumberOfValuesHigherThanBins.ToString()
                    );
            }
        }
    }
}
