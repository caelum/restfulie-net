﻿using System;
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
        
        private Stream GetRequestStreamAsynch(HttpWebRequest request) {
            return AsynchHelper.WaitForAsynchResponse(
                c => request.BeginGetRequestStream(c, null),
                (r, s) => request.EndGetRequestStream(r)
            );
        }

        private void WriteContent(bool asynch, HttpWebRequest request, string content) {
            byte[] byteArray = Encoding.UTF8.GetBytes(content);
            request.ContentLength = byteArray.Length;
            Stream bodyStream;
            if (asynch) 
                bodyStream = GetRequestStreamAsynch(request);
            #if SILVERLIGHT
            else
                throw new InvalidOperationException("Silverlight must be run in async mode");
            #else
            else
                bodyStream = request.GetRequestStream();
            #endif
            bodyStream.Write(byteArray, 0, byteArray.Length);
            bodyStream.Close();
        }

        private HttpRemoteResponse GetRemoteResponse(HttpWebResponse response) {
            return new HttpRemoteResponse(
                response.StatusCode, 
                response.Headers.AllKeys.ToDictionary(k => k, k => response.Headers[k], StringComparer.OrdinalIgnoreCase), 
                GetContent(response));
        }

        private HttpWebResponse GetResponseAsynch(HttpWebRequest request) {
            return AsynchHelper.WaitForAsynchResponse(
                c => request.BeginGetResponse(c, null),
                (r, s) => (HttpWebResponse)request.EndGetResponse(r)
            );
        }

        private HttpWebResponse GetResponse(bool asynch, HttpWebRequest request) {
            if (asynch)
                return GetResponseAsynch(request);

#if SILVERLIGHT
            throw new InvalidOperationException("Silverlight must be run in async mode");
#else
            return (HttpWebResponse)request.GetResponse();
#endif
        }

        public HttpRemoteResponse Process(IRemoteResourceService service, string verb, Uri uri, string content) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            try
            {
                foreach (var header in service.Headers)
                    request.Headers[header.Key] = header.Value;

                request.Method = verb;
                if (!String.IsNullOrWhiteSpace(content))
                    WriteContent(service.IsAsynch, request, content);
                return GetRemoteResponse(GetResponse(service.IsAsynch, request));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("An error occurred while connecting to the resource in url {0} with message {1}", uri, ex.Message), ex);
            }
        }
    }
}
