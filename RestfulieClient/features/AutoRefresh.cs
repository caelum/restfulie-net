using System;
using System.Text.RegularExpressions;
using System.Threading;
using RestfulieClient.request;
using RestfulieClient.service;

namespace RestfulieClient.features
{
    /// <summary>
    /// Automatically performs a resource refresh based on the Refresh header sent by the server
    /// </summary>
    public class AutoRefresh : IResponseFeature
    {
        protected class Refresh
        {
            public int Seconds { get; set; }
            public string Uri { get; set; }
        }

        public HttpRemoteResponse Process(ResponseChain chain, HttpRemoteResponse response)
        {
            if (!chain.Service.IsAsynch)
                throw new NotSupportedException("AutoRefresh is not supported on synchronous requests");

            response = chain.Next(response);

            Refresh refresh;

            if (!ShouldRefresh(response, out refresh) || refresh == null)
                return response;

            Thread.Sleep(TimeSpan.FromSeconds(refresh.Seconds)); // TODO: use a better alternative

            response = chain.Service.Dispatcher.Process(chain.Service, "GET", new Uri(refresh.Uri), null);
            response = chain.Next(response);

            return response;
        }

        private static readonly Regex RefreshPattern = new Regex(@"^(\d+)\;\s?url\=(.*)$",
              RegexOptions.Singleline
            | RegexOptions.IgnoreCase
#if !SILVERLIGHT
            | RegexOptions.Compiled
#endif
            );

        protected bool ShouldRefresh(HttpRemoteResponse response, out Refresh refresh)
        {
            var should = (int)response.StatusCode / 100 == 2 && response.Headers.ContainsKey("Refresh");

            refresh = null;

            if (should)
            {
                var match = RefreshPattern.Match(response.Headers["Refresh"]);

                if (match.Success && match.Groups.Count == 3)
                    refresh = new Refresh
                    {
                        Seconds = int.Parse(match.Groups[1].Value),
                        Uri = match.Groups[2].Value
                    };
            }

            return should;
        }
    }
}
