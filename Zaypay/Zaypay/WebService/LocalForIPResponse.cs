using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Zaypay.WebService;

namespace Zaypay.WebService
{
    public class LocalForIPResponse : Response
    {

        public LocalForIPResponse(Hashtable resp)
        {
            response = resp;
        }

        public string Locale()
        {            
            return (String)response["locale"];
        }
        
    }
}
