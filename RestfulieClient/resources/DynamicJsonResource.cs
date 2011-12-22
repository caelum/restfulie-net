using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestfulieClient.service;

namespace RestfulieClient.resources
{
    public class DynamicJsonResource : DynamicObject, IEnumerable, IResource
    {
        private readonly StringValueConverter _converter = new StringValueConverter();

        private readonly IRemoteResourceService _service;
        private readonly JContainer _root;
        private readonly IDictionary<string, JToken> _rootIndex = new Dictionary<string, JToken>(StringComparer.OrdinalIgnoreCase);

        public HttpRemoteResponse WebResponse { get; private set; }
        public bool IsEmpty { get { return !_root.HasValues; } }
        public NumberFormatInfo NumberFormatInfo { get; set; }

        public DynamicJsonResource(HttpRemoteResponse webResponse) {
            WebResponse = webResponse;
            _root = String.IsNullOrWhiteSpace(webResponse.Content)
                ? new JObject()
                : JObject.Parse(webResponse.Content);

            IndexRoot();
        }

        public DynamicJsonResource(HttpRemoteResponse webResponse, IRemoteResourceService service)
            : this(webResponse) {
            _service = service;
        }

        private DynamicJsonResource(HttpRemoteResponse webResponse, IRemoteResourceService service, JContainer root) {
            WebResponse = webResponse;
            _service = service;
            _root = root;

            IndexRoot();
        }

        private void IndexRoot() {
            _rootIndex.Clear();

            if (_root.Type != JTokenType.Object)
                return;

            var o = (JObject)_root;

            ReadLinks();

            foreach (var child in o.Properties())
                _rootIndex.Add(child.Name, child.Value);
        }

        private object InvokeRemoteResource(string uri, string transitionName, string content) {
            return String.IsNullOrWhiteSpace(content)
                ? _service.Execute(uri, transitionName)
                : _service.Execute(uri, transitionName, content);
        }

        private object GetValueFromToken(JToken token) {
            if (token == null)
                return null;

            switch (token.Type) {
                case JTokenType.Object:
                case JTokenType.Array:
                    return new DynamicJsonResource(WebResponse, _service, (JContainer)token);
                case JTokenType.Boolean:
                    return (bool)token;
                case JTokenType.Date:
                    return (DateTime)token;
                case JTokenType.Float:
                    return (float)token;
                case JTokenType.Integer:
                    return (int)token;
                case JTokenType.String:
                    return _converter.TransformText((string)token).WithNumberFormatInfo(NumberFormatInfo).ToValue();
                default:
                    return token;
            }
        }

        private readonly IDictionary<string, JObject> _links = new Dictionary<string, JObject>(StringComparer.OrdinalIgnoreCase);

        private void ReadLinks() {
            _links.Clear();

            var single = _root["link"] as JObject;

            if (single != null) {
                _links.Add((string)single["rel"], single);
                return;
            }

            var links = _root["links"] as JArray;

            if (links == null)
                return;

            foreach (var link in links.Where(l => l is JObject).Cast<JObject>())
                _links.Add((string)link["rel"], link);
        }

        private JObject GetLink(string rel) {
            if (_root.Type != JTokenType.Object)
                throw new InvalidOperationException(String.Format("Link scanning not supported for type: {0}", _root.Type));

            JObject link;

            return _links.TryGetValue(rel, out link) ? link : null;
        }

        private IResource FollowLink(JObject link, string content = null) {
            var resource = (IResource)InvokeRemoteResource((string)link["href"], (string)link["rel"], content);
            IResource result;

            if (resource.WebResponse.HasNoContent()) {
                result = WebResponse.HasNoContent() ? null : this;
                WebResponse = resource.WebResponse;
            }
            else
                result = resource;

            return result;
        }

        private string Serialize(object o) {
            return JsonConvert.SerializeObject(o);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            if (_root.Type != JTokenType.Object)
                throw new InvalidOperationException(String.Format("Cannot get member for {0}", _root.Type));

            JToken token;

            result = null;

            if (_rootIndex.TryGetValue(binder.Name.Replace("_", "-"), out token))
                result = GetValueFromToken(token);

            return result != null;
        }

        public JToken GetTokenByIndex(JArray array, int index) {
            if (index < 0 || index > array.Count - 1)
                throw new ArgumentOutOfRangeException("index");

            return array[index];
        }

        public JToken GetTokenById(JArray array, string id) {
            if (String.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("id");

            return array
                .Where(t => t.Type == JTokenType.Object)
                .Cast<JObject>()
                .FirstOrDefault(o => 
                    o.Property("id") != null &&
                    o.Property("id").Value.ToString().Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) {
            if (_root.Type != JTokenType.Array)
                throw new InvalidOperationException(String.Format("Cannot get index for {0}", _root.Type));
            if (indexes == null)
                throw new ArgumentNullException("indexes");
            if (indexes.Length <= 0 || indexes.Length >= 2)
                throw new IndexOutOfRangeException("Only one lookup index can be supplied");

            var array = (JArray)_root;
            var index = indexes.First();
            var token = index is int
                ? GetTokenByIndex(array, (int)index)
                : GetTokenById(array, index.ToString());

            result = GetValueFromToken(token);

            return result != null;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
            if (binder.Name.StartsWith("has", StringComparison.OrdinalIgnoreCase) && binder.Name.Length > 3) {
                result = HasLink(binder.Name.Substring(3));
                return true;
            }

            var link = GetLink(binder.Name);

            if (link == null)
                throw new ArgumentException(string.Format("There is no link defined with rel: {0}", binder.Name));

            string content = null;

            if (args != null && args.Length == 1)
                content = args.First() as string ?? Serialize(args.First());

            result = FollowLink(link, content);

            return result != null ? true : false;
        }
        
        public T As<T>() where T : class {
            if (_root is JObject)
                return new JsonSerializer().Deserialize<T>(new JTokenReader(_root));

            throw new InvalidOperationException("As can only be called on objects");
        }

        public T[] AsMany<T>() where T : class {
            if (_root is JArray)
                return new JsonSerializer().Deserialize<T[]>(new JTokenReader(_root));

            throw new InvalidOperationException("AsMany can only be called on arrays");
        }

        public bool HasLink(string rel) {
            return _links.ContainsKey(rel);
        }

        public IResource Follow(string rel, string content = null) {
            var link = GetLink(rel);

            if (link == null)
                throw new ArgumentException(String.Format("There is no link defined with rel: {0}", rel));

            return FollowLink(link, content);
        }

        public IEnumerator GetEnumerator() {
            if (_root.Type != JTokenType.Array)
                throw new InvalidOperationException(String.Format("Cannot get index for {0}", _root.Type));

            return _root.Select(GetValueFromToken).GetEnumerator();
        }
    }
}
