using Microsoft.AspNetCore.WebUtilities;
using Serilog;
using System.Net;
using System.Xml;
using System.Xml.Serialization;

namespace WsjtxUtils.Searchlight.Common.PskReporter
{
    public static class PskReporterUtils
    {
        private const string PskReporterQueryUrl = "https://retrieve.pskreporter.info/query";
        private static readonly HttpClient httpClient = CreateHttpClientWithDecompressionSupport();

        /// <summary>
        /// Fetch PskReporter report data for the parameters specified
        /// </summary>
        /// <param name="queryParameters"></param>
        /// <returns></returns>
        public static async Task<PskReceptionReports?> GetPskReporterData(PskReporterDataRetrievalQueryParameters queryParameters, CancellationToken cancellationToken = default)
        {
            var url = QueryHelpers.AddQueryString(PskReporterQueryUrl, queryParameters.ToDictionary());
            Log.Debug("HTTP Get {url}.", url);

            var response = await httpClient.GetAsync(url, cancellationToken);
            Log.Debug("Request complete: {url} {code} {headers}", url, response.StatusCode, response.Headers);

            response.EnsureSuccessStatusCode();

            Log.Debug("Deserialize reception reports");
            var serializer = new XmlSerializer(typeof(PskReceptionReports));
            using var reader = new XmlTextReader(await response.Content.ReadAsStreamAsync(cancellationToken));
            return (PskReceptionReports?)serializer.Deserialize(reader);
        }

        /// <summary>
        /// Create a <see cref="HttpClient"/> that supports AutomaticDecompression
        /// </summary>
        /// <returns></returns>
        private static HttpClient CreateHttpClientWithDecompressionSupport()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };

            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.All;

            var client = new HttpClient(handler);

            //client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("searchlight","0.1"));

            return client;
        }
    }
}
