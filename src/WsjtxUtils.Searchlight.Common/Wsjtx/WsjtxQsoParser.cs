using System.Text.RegularExpressions;
using WsjtxUtils.WsjtxMessages.Messages;

namespace WsjtxUtils.Searchlight.Common.Wsjtx
{
    /// <summary>
    /// WSJT-X decode message QSO parser
    /// </summary>
    public static class WsjtxQsoParser
    {
        /// <summary>
        /// Attempt to parse as much information as possible about the OSO of a WSJT-X decode packet
        /// </summary>
        /// <param name="decode"></param>
        /// <returns></returns>
        public static WsjtxQso ParseDecode(Decode decode)
        {
            // initialize the result object and extract the 'parts'
            // of the raw message and normalize for parsing
            WsjtxQso result = new WsjtxQso(decode);
            var parts = decode.Message
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(part => part != "?")
                .ToArray();
            var length = parts.Length;

            // if there is less than two parts, it's a message in a
            // unknown state that can't be parsed
            if (length < 2)
                return result;

            // setup the values and objects needed to decode the message
            var deCallSignIndex = 1;
            var dxCallSignIndex = 0;
            var gridSquareIndex = -1;
            string callingModifier = string.Empty;
            bool usesPriori = Regex.Match(parts.Last(), "a[1-6]").Success;

            // Check if the station CQ       
            if (parts[0] == "CQ" || parts[0] == "QRZ")
            {
                result.QsoState = QsoState.CallingCq;
                dxCallSignIndex = -1;

                // Check if the CQ call has a modifer (DX, POTA, TEST, etc.)
                if (length == 4 && !usesPriori || length == 5 && usesPriori)
                {
                    callingModifier = parts[1];
                    deCallSignIndex++;
                }

                // Find the grid if included with the CQ call
                var offset = (usesPriori) ? 2 : 1;
                if (deCallSignIndex + offset < length)
                    gridSquareIndex = deCallSignIndex + 1;
            }
            // not calling CQ and more than two or three parts
            else if (length > (usesPriori ? 3 : 2))
            {
                var lastPartIndex = length - (usesPriori ? 2 : 1);
                var targetPart = parts[lastPartIndex];

                if (Regex.Match(targetPart, @"R[+-][\d]+").Success)
                {
                    result.QsoState = QsoState.RogerReport;
                }
                else if (Regex.Match(targetPart, @"[+-][\d]+").Success)
                {
                    result.QsoState = QsoState.Report;
                }
                else if (targetPart == "RRR" || targetPart == "RR73")
                {
                    result.QsoState = QsoState.Rogers;
                }
                else if (targetPart == "73")
                {
                    result.QsoState = QsoState.Signoff;
                }
                else if (Regex.Match(targetPart, @"[A-Z]{2}[\d]{2}([A-Za-z]{2})?").Success)
                {
                    result.QsoState = QsoState.CallingStation;
                    gridSquareIndex = lastPartIndex;
                }
                else
                {
                    deCallSignIndex = dxCallSignIndex = -1;
                }
            }
            // Grab the callsign and gridsquare
            result.CallingModifier = callingModifier;
            result.UsedPriori = usesPriori;

            result.DXCallsign = (dxCallSignIndex == -1)
                ? string.Empty
                : parts[dxCallSignIndex].Replace("<", "").Replace(">", "");

            result.DECallsign = (deCallSignIndex == -1)
                ? string.Empty
                : parts[deCallSignIndex].Replace("<", "").Replace(">", "");

            result.GridSquare = (gridSquareIndex == -1)
                ? string.Empty
                : parts[gridSquareIndex];

            return result;
        }
    }
}
