using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace RestfulieClient.service
{
    class HttpRemoteResponseFactory
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
            BufferedStream stream = new BufferedStream(ResponseStream);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static Dictionary<string, string> GetHeadersDictionaryFrom(WebHeaderCollection headers)
        {
            Dictionary<string,string> dictionary = new Dictionary<string,string>();
            foreach( KeyValuePair<string,string> kvp in headers ){
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                dictionary.Add(kvp.Key.Replace("-","").ToUpper(), kvp.Value);
            }
            return dictionary;
        }
    }
}
