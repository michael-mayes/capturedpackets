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
    class LatencyAnalysisStructures
    {
        //Dictionary Key
        
        public struct LatencyAnalysisDictionaryKey
        {
            public byte HostId;
            public LatencyAnalysisConstants.LatencyAnalysisProtocol Protocol;
            public ulong SequenceNumber;

            public LatencyAnalysisDictionaryKey(byte TheHostId, LatencyAnalysisConstants.LatencyAnalysisProtocol TheProtocol, ulong TheSequenceNumber)
            {
                HostId = TheHostId;
                Protocol = TheProtocol;
                SequenceNumber = TheSequenceNumber;
            }

            public override bool Equals(object obj)
            {
                if (obj is LatencyAnalysisDictionaryKey)
                {
                    LatencyAnalysisDictionaryKey Key = (LatencyAnalysisDictionaryKey)obj;

                    return
                        (
                        HostId == Key.HostId &&
                        Protocol == Key.Protocol &&
                        SequenceNumber == Key.SequenceNumber
                        );
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return
                    HostId.GetHashCode() ^
                    Protocol.GetHashCode() ^
                    SequenceNumber.GetHashCode();
            }
        }

        //Dictionary Value
        
        public struct LatencyAnalysisDictionaryValue
        {
            public ulong MessageId;
            public bool FirstInstanceFound;
            public bool SecondInstanceFound;
            public ulong FirstInstancePacketNumber;
            public ulong SecondInstancePacketNumber;
            public double FirstInstanceTimestamp;
            public double SecondInstanceTimestamp;
            public double TimestampDifference;
            public bool TimestampDifferenceCalculated;

            public override bool Equals(object obj)
            {
                if (obj is LatencyAnalysisDictionaryValue)
                {
                    LatencyAnalysisDictionaryValue Value = (LatencyAnalysisDictionaryValue)obj;

                    return
                        (
                        MessageId == Value.MessageId &&
                        FirstInstanceFound == Value.FirstInstanceFound &&
                        SecondInstanceFound == Value.SecondInstanceFound &&
                        FirstInstancePacketNumber == Value.FirstInstancePacketNumber &&
                        SecondInstancePacketNumber == Value.SecondInstancePacketNumber &&
                        FirstInstanceTimestamp == Value.FirstInstanceTimestamp &&
                        SecondInstanceTimestamp == Value.SecondInstanceTimestamp &&
                        TimestampDifference == Value.TimestampDifference &&
                        TimestampDifferenceCalculated == Value.TimestampDifferenceCalculated
                        );
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return
                    MessageId.GetHashCode() ^
                    FirstInstanceFound.GetHashCode() ^
                    SecondInstanceFound.GetHashCode() ^
                    FirstInstancePacketNumber.GetHashCode() ^
                    SecondInstancePacketNumber.GetHashCode() ^
                    FirstInstanceTimestamp.GetHashCode() ^
                    SecondInstanceTimestamp.GetHashCode() ^
                    TimestampDifference.GetHashCode() ^
                    TimestampDifferenceCalculated.GetHashCode();
            }
        }
    }
}