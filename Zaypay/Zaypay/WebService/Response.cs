using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Zaypay.WebService
{
    public class Response
    {
        protected Hashtable response;

        public Hashtable RESPONSE
        {
            get { return response; }
            set { response = value; }
        }

        public virtual bool IsError()
        {
            return response.ContainsKey("error");
        }

        public virtual String Error()
        {
            return (String)response["error"];
        }
    }
}
