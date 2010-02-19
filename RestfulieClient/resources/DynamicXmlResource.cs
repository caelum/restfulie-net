using System;
using System.Linq;
using System.Xml.Linq;
using System.Dynamic;
using System.Reflection;
using RestfulieClient.service;

namespace RestfulieClient.resources
{
    public class DynamicXmlResource : DynamicObject
    {
        public HttpRemoteResponse WebResponse { get; private set; }
        public IRemoteResourceService remoteResourceService { get; private set; }
        public XElement XmlRepresentation
        {
            get
            {
                if (this.WebResponse.HasNoContent())
                    return null;
                else
                    return XElement.Parse(this.WebResponse.Content);
            }

        }

        public DynamicXmlResource(HttpRemoteResponse response)
        {
            this.WebResponse = response;
        }

        public DynamicXmlResource(HttpRemoteResponse response, IRemoteResourceService remoteService)
            : this(response)
        {
            this.remoteResourceService = remoteService;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string fieldName = binder.Name.Replace("_", "-").ToLower();
            XElement firstElement = this.GetFirstElementWithName(fieldName);
            result = this.GetValueFromXmlElement(firstElement);
            return result != null ? true : false;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            object value = this.GetValueFromAttributeName(binder.Name, "href");
            if (value == null)
                throw new ArgumentException(string.Format("There is not method defined with name:", binder.Name));

            DynamicXmlResource resource = (DynamicXmlResource)this.InvokeRemoteResource(value.ToString(), binder.Name);

            if (resource.WebResponse.HasNoContent())
            {
                result = this.XmlRepresentation;
                this.UpdateWebResponse(resource.WebResponse);
            }
            else
            {
                result = resource;
            }
            return result != null ? true : false;
        }

        private object InvokeRemoteResource(string url, string transitionName)
        {
            try
            {
                Type remoteResourceServiceType = this.remoteResourceService.GetType();
                return remoteResourceServiceType.InvokeMember("Execute",
                                                                BindingFlags.InvokeMethod |
                                                                BindingFlags.Public |
                                                                BindingFlags.Instance,
                                                                null, this.remoteResourceService, new Object[] { url, transitionName });
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
                    return new DynamicXmlResource(this.WebResponse);
                }
                else
                {
                    return element.Value;
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

        private void UpdateWebResponse(HttpRemoteResponse response)
        {
            this.WebResponse = response;
        }
    }
}
