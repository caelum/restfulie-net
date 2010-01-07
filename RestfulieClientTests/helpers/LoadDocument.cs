using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;



namespace RestfulieClientTests.helpers
{
    public class LoadDocument
    {

        public string GetDocumentContent(string fileName)
        {
            string filePath = "RestfuliClientTests.xmls." + fileName;  
            Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);  
            XmlDocument data =null;
            if (fileStream != null)
            {
                 data = new XmlDocument();  
                 data.Load(fileStream);                      
            }
            return data.InnerXml;
        }
    }
}
