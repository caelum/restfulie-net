using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using RestfulieClient.request;
using RestfulieClient.service;

namespace RestfulieClientTests.helpers
{
    public class EmbeddedFileRequestDispatcher : IRequestDispatcher
    {
        private readonly string _contentType;

        public EmbeddedFileRequestDispatcher(string contentType) {
            _contentType = contentType;
        }

        private string GetContentFromFile(Uri uri) {
            var content = "";

            if (uri != null && uri.IsFile)
                content = new LoadDocument(_contentType).GetDocumentContent(Path.GetFileName(uri.OriginalString));

            return content;
        }

        private HttpRemoteResponse CreateRemoteResponse(string content, string verb) {
            var statusCode = HttpStatusCode.OK;
            var hasContent = !String.IsNullOrWhiteSpace(content);

            switch (verb.ToUpper()) {
                case "POST":
                    statusCode = hasContent ? HttpStatusCode.Created : HttpStatusCode.BadRequest;
                    break;
                case "PUT":
                    statusCode = hasContent ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                    break;
                case "DELETE":
                    statusCode = HttpStatusCode.NoContent;
                    break;
            }

            var response = new HttpRemoteResponse(statusCode, new Dictionary<string, string>(), content);

            if (hasContent) {
                response.Headers.Add("Content-Type", _contentType);
                response.Headers.Add("Content-Length", content.Length.ToString());
            }

            return response;
        }

        public HttpRemoteResponse Process(IRemoteResourceService service, string verb, Uri uri, string content) {
            return CreateRemoteResponse(GetContentFromFile(uri), verb);
        }
    }
}
