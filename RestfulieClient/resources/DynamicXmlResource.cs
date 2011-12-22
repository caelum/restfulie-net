﻿using System;
using System.Linq;
using System.Xml.Linq;
using System.Dynamic;
using RestfulieClient.service;
using System.Globalization;

namespace RestfulieClient.resources
{
    public class DynamicXmlResource : DynamicObject, IResource
    {
        private readonly StringValueConverter _converter = new StringValueConverter();

        public HttpRemoteResponse WebResponse { get; private set; }

        public bool IsEmpty {
            get { return WebResponse.HasNoContent(); } // should check if any nodes exist
        }

        public IRemoteResourceService RemoteResourceService { get; private set; }
        public NumberFormatInfo NumberFormatInfo { get; set; }
        public XElement XmlRepresentation
        {
            get
            {
                if (WebResponse.HasNoContent())
                    return null;
                
                return XElement.Parse(WebResponse.Content);
            }
        }

        public DynamicXmlResource(HttpRemoteResponse response)
        {
            WebResponse = response;
            NumberFormatInfo = NumberFormatInfo.CurrentInfo;
        }

        public DynamicXmlResource(HttpRemoteResponse response, IRemoteResourceService remoteService)
            : this(response)
        {
            RemoteResourceService = remoteService;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string fieldName = binder.Name.Replace("_", "-").ToLower();
            XElement firstElement = GetFirstElementWithName(fieldName);
            result = GetValueFromXmlElement(firstElement);
            return result != null ? true : false;
        }

        private object FollowLink(string rel, string href, string content = null) {
            var resource = (IResource)InvokeRemoteResource(href, rel, content);
            object result;

            if (resource.WebResponse.HasNoContent()) {
                result = XmlRepresentation;
                WebResponse = resource.WebResponse;
            }
            else
                result = resource;

            return result;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            object value = GetValueFromAttributeName(binder.Name, "href");
            if (value == null)
                throw new ArgumentException(string.Format("There is not method defined with name:", binder.Name));

            string content = null;

            if (args != null && args.Length == 1)
                content = args.First() as string;

            result = FollowLink(binder.Name, (string)value, content);

            return result != null ? true : false;
        }

        private object InvokeRemoteResource(string uri, string transitionName, string content)
        {
            try
            {
                return String.IsNullOrWhiteSpace(content)
                    ? RemoteResourceService.Execute(uri, transitionName)
                    : RemoteResourceService.Execute(uri, transitionName, content);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Error invoke remote resource method {0}.", ex.Message));
            }
        }

        private object GetValueFromXmlElement(XElement element)
        {
            if (element != null)
            {
                if (element.HasElements)
                {
                    return new DynamicXmlResource(WebResponse);
                }
                else
                {
                    object result = _converter.TransformText(element.Value).WithNumberFormatInfo(NumberFormatInfo).ToValue();
                    return result;
                }
            }
            return null;
        }

        private XElement GetFirstElementWithName(string name)
        {
            XElement firstElement = XmlRepresentation.Descendants(name).FirstOrDefault();

            return firstElement;
        }

        private object GetValueFromAttributeName(string name, string attributeName)
        {
            foreach (XElement element in XmlRepresentation.Elements())
            {
                XAttribute attribute = element.Attributes().Where(attr => attr.Name == "rel").SingleOrDefault();
                if ((attribute != null) && (attribute.Value.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    XAttribute attrib = element.Attributes().Where(attr => attr.Name == attributeName).SingleOrDefault();
                    return attrib.Value;
                }
            }
            return null;
        }

        public bool HasLink(string rel) {
            return GetValueFromAttributeName(rel, "href") != null;
        }

        public IResource Follow(string rel, string content) {
            var link = GetValueFromAttributeName(rel, "href");

            if (link == null)
                throw new ArgumentException(String.Format("There is no link defined with rel: {0}", rel));

            return FollowLink(rel, (string)link, content) as IResource; // Note: need to fix up XElement result
        }

        public T As<T>() where T : class {
            throw new NotImplementedException();
        }

        public T[] AsMany<T>() where T : class {
            throw new NotImplementedException();
        }
    }
}
