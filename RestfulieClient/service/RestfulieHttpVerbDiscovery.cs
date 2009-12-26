using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestfulieClient.service
{
    public class RestfulieHttpVerbDiscovery
    {

        private const string GET = "GET";
        private const string POST = "POST";
        private const string UPDATE = "UPDATE";
        private const string DELETE = "DELETE";

        private Dictionary<string, string> verbNames;

        public RestfulieHttpVerbDiscovery()
        {
            InitializeHttpVerbsWithTransitionNameRelationship();
        }     

        public string GetHttpVerbByTransitionName(string transitionName)
        {            
            try
            { 
                string verbName = "";  
                if (!this.verbNames.TryGetValue(transitionName, out verbName))
                    return GET;
                return verbName.ToUpperInvariant() ;
            }catch(Exception)
            {
                throw new ArgumentException(string.Format("The transition {0} is not supported by Restfulie client", transitionName));
            }            
        }

        private void InitializeHttpVerbsWithTransitionNameRelationship()
        {
            verbNames = new Dictionary<string, string>();
            verbNames.Add("Cancel",DELETE);
            verbNames.Add("Destroy", DELETE);
            verbNames.Add("Delete", DELETE);
            verbNames.Add("Update", POST);
            verbNames.Add("Refresh", GET);
            verbNames.Add("Reload", GET);
            verbNames.Add("Show", GET);
            verbNames.Add("Latest", GET);
        }
    }
}
