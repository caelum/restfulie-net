using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Dynamic;
using RestfulieClient.resources;
using System.Reflection;

namespace RestfulieClient.resources
{
    public class DynamicXmlResource : DynamicObject
    {

        private XElement xmlElement;
        public IRemoteResourceService remoteResourceService { get; set; }

        public DynamicXmlResource(XElement xml)
        {
            this.xmlElement = xml;  
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            XElement firstElement = this.GetFirstElementWithName(binder.Name);
            result = this.GetValueFromXmlElement(firstElement);
            return result != null ? true : false ; 
        }        

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            object value = this.GetValueFromAttributeName(binder.Name,"href");
            if (value == null)
                throw new ArgumentException(string.Format("There is not method defined with name:", binder.Name));
            result = this.InvokeRemoteResource(value.ToString(),binder.Name);
            return result != null ? true : false;
        }

        private object InvokeRemoteResource(string url, string  transitionName)
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
                    return new DynamicXmlResource(element);
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
            XElement firstElement = xmlElement.Descendants(name).FirstOrDefault();            
            return firstElement;
        }

        private object GetValueFromAttributeName(string name,string attributeName)
        {
             foreach (XElement element in xmlElement.Elements())
            {
                XAttribute attribute = element.Attributes().Where(attr => attr.Name == "rel").SingleOrDefault();
                if ((attribute != null) && (attribute.Value.Equals(name,StringComparison.CurrentCultureIgnoreCase)))
                {
                    XAttribute attrib = element.Attributes().Where(attr => attr.Name == attributeName).SingleOrDefault();
                    return attrib.Value;
                }
            }
            return null;
        }


    }
}
