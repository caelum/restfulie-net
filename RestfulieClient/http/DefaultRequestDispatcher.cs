using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using RestfulieClient.request;
using RestfulieClient.service;

namespace RestfulieClient.http
{
    public class DefaultRequestDispatcher : IRequestDispatcher
    {
        private string GetContent(HttpWebResponse response) {
            Stream stream = response.GetResponseStream();

            if (stream == null || stream.Length == 0)
                return null;

            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        private void WriteContent(HttpWebRequest request, string content) {
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            request.ContentLength = byteArray.Length;
            Stream bodyStream = request.GetRequestStream();
            bodyStream.Write(byteArray, 0, byteArray.Length);
            bodyStream.Close();
        }

        private HttpRemoteResponse GetRemoteResponse(HttpWebResponse response) {
            return new HttpRemoteResponse(
                response.StatusCode, 
                response.Headers.AllKeys.ToDictionary(k => k, k => response.Headers[k], StringComparer.OrdinalIgnoreCase), 
                GetContent(response));
        }

        public HttpRemoteResponse Process(IRemoteResourceService service, string verb, Uri uri, string content) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            try
            {
                foreach (var header in service.Headers)
                    request.Headers.Add(header.Key, header.Value);

                request.Method = verb;
                if (!String.IsNullOrWhiteSpace(content))
                    WriteContent(request, content);
                return GetRemoteResponse((HttpWebResponse)request.GetResponse());
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("An error occurred while connecting to the resource in url {0} with message {1}.", uri, ex.Message), ex);
            }
        }
    }
}
