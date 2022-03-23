using WsjtxUtils.Searchlight.Common.PskReporter;
using WsjtxUtils.WsjtxMessages.Messages;
using WsjtxUtils.WsjtxUdpServer;

namespace WsjtxUtils.Searchlight.Common
{
    public static class Utils
    {
        /// <summary>
        /// Determine the band based on the frequency in hertz
        /// </summary>
        /// <param name="frequencyInHertz"></param>
        /// <returns></returns>
        public static ulong ApproximateBandFromFrequency(double frequencyInHertz)
        {
            return Convert.ToUInt64(300 / (frequencyInHertz / 1000000));
        }

        /// <summary>
        /// Scale a value
        /// </summary>
        /// <param name="val"></param>
        /// <param name="minAllowed"></param>
        /// <param name="maxAllowed"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Scale(int val, int minAllowed, int maxAllowed, int min, int max)
        {
            return (maxAllowed - minAllowed) * (val - min) / (max - min) + minAllowed;
        }

        /// <summary>
        /// Where reception report receiver is callsign
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="receiverCallsign"></param>
        /// <returns></returns>
        public static IEnumerable<PskReceptionReport> WhereReceiverCallsign(this IEnumerable<PskReceptionReport> enumerable, string receiverCallsign)
        {
            return enumerable.Where(report => !string.IsNullOrEmpty(report.ReceiverCallsign) && report.ReceiverCallsign == receiverCallsign);
        }

        /// <summary>
        /// Where logged QSO local callsign
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="deCallsign"></param>
        /// <returns></returns>
        public static IEnumerable<QsoLogged> WhereDECallsign(this IEnumerable<QsoLogged> enumerable, string deCallsign)
        {
            return enumerable.Where(qso => qso.MyCall == deCallsign);
        }

        /// <summary>
        /// Where logged QSO is of mode
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IEnumerable<QsoLogged> WhereMode(this IEnumerable<QsoLogged> enumerable, string mode)
        {
            return enumerable.Where(qso => qso.Mode == mode);
        }

        /// <summary>
        /// Where reception report band
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="band"></param>
        /// <returns></returns>
        public static IEnumerable<PskReceptionReport> WhereBand(this IEnumerable<PskReceptionReport> enumerable, ulong band)
        {
            return enumerable.Where(report => report.Frequency != 0 && Utils.ApproximateBandFromFrequency(report.Frequency) == band);
        }

        /// <summary>
        /// Where reception report mode
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IEnumerable<PskReceptionReport> WhereMode(this IEnumerable<PskReceptionReport> enumerable, string mode)
        {
            return enumerable.Where(report => report.Mode == mode);
        }

        /// <summary>
        /// Where connected client
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, WsjtxConnectedClient>> WhereValidStatus(this IEnumerable<KeyValuePair<string, WsjtxConnectedClient>> enumerable)
        {
            return enumerable.Where(kvp => kvp.Value.Status != null && !string.IsNullOrEmpty(kvp.Value.Status.DECall) && kvp.Value.Status.DialFrequencyInHz != 0 && !string.IsNullOrEmpty(kvp.Value.Status.Mode));
        }

        /// <summary>
        /// Where not previously contacted
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="previousQsos"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, Wsjtx.WsjtxQso>> WhereNotPreviouslyContacted(this IEnumerable<KeyValuePair<string, Wsjtx.WsjtxQso>> enumerable, IEnumerable<QsoLogged> previousQsos)
        {
            return enumerable.Where(kvp => !previousQsos.Any(logged => kvp.Key == logged.DXCall));
        }

        /// <summary>
        /// Where not previously highlighted
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="highlightedCalls"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, Wsjtx.WsjtxQso>> WhereNotPreviouslyHiglighted(this IEnumerable<KeyValuePair<string, Wsjtx.WsjtxQso>> enumerable, IEnumerable<string> highlightedCalls)
        {
            return enumerable.Where(kvp => !highlightedCalls.Any(call => call == kvp.Value.DECallsign));
        }
    }
}
