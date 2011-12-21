using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace RestfulieClientTests.helpers
{
    public class LoadDocument
    {
        private static readonly IDictionary<string, string> ContentTypeNamespaces = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "application/xml", "xmls" }
        };

        private readonly string _contentType;

        public LoadDocument(string contentType) {
            _contentType = contentType;
        }

        public string GetDocumentContent(string fileName) {
            var filePath = String.Format("RestfulieClientTests.{0}.{1}", ContentTypeNamespaces[_contentType], fileName);

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath))
                return stream == null ? "" : new StreamReader(stream).ReadToEnd();
        }
    }
}
