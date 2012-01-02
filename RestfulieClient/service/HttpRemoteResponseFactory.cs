using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace RestfulieClient.service
{
    public class HttpRemoteResponseFactory
    {
        public static HttpRemoteResponse GetRemoteResponse(HttpWebResponse webResponse)
        {
            HttpRemoteResponse response = new HttpRemoteResponse(webResponse.StatusCode,
                GetHeadersDictionaryFrom(webResponse.Headers),
                GetContentFromStream(webResponse.GetResponseStream()));

            webResponse.Close();
            return response; 
        }

        private static String GetContentFromStream(Stream ResponseStream)
        {
#if SILVERLIGHT
            Stream stream = ResponseStream;
#else
            BufferedStream stream = new BufferedStream(ResponseStream);
#endif
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static Dictionary<string, string> GetHeadersDictionaryFrom(WebHeaderCollection headers)
        {
            string pattern = "\\s+";
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string rawKey in headers.AllKeys)
            {
                string key = rawKey.Replace("-", " ").ToUpper();
                key = Regex.Replace(key, pattern, "");
                string value = headers[rawKey];
                //System.Console.WriteLine("Key => " + key + " Value => " + value);
                dictionary.Add(key, value);
            }
            return dictionary;
        }
    }
}
