using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Zaypay.WebService;
using System.Web;
using System.Collections.Specialized;

namespace Zaypay.WebService
{
    public class PaymentResponse : Response
    {

        public PaymentResponse(Hashtable resp)
        {
            response = resp;
        }

        public Hashtable Instructions()
        {
            Hashtable ins = new Hashtable();

            ins["short-instructions"] = response["short-instructions"];
            ins["long-instructions"] = response["long-instructions"];
            ins["very-short-instructions"] = response["very-short-instructions"];
            ins["very-short-instructions_with_amount"] = response["very-short-instructions-with-amount"];

            return ins;
        }

        public string StatusString()
        {
            return (String)response["status-string"];
        }

        public bool VerificationNeeded()
        {
            return Convert.ToBoolean(((Hashtable)response["payment"])["verification-needed"]);
            
            //return (string)((Hashtable)response["payment"])["verification-needed"];
        }

        public int VerificationTriesLeft()
        {
            int tries = 0;

            if (((Hashtable)(response["payment"])).ContainsKey("verification-tries-left"))            
              tries = Convert.ToInt32(((Hashtable)response["payment"])["verification-tries-left"]);
                
            return tries <= 0 ? 0 : tries;                
            
        }

        public Hashtable Payment()
        {
            return (Hashtable)response["payment"];
        }

        public string Status()
        {       
            return (string)((Hashtable)response["payment"])["status"];
        }

        public int PaymentMethodId()
        {
            return Convert.ToInt32(((Hashtable)response["payment"])["payment-method-id"]);
        }

        public string Platform()
        {
            return (string)(((Hashtable)response["payment"])["platform"]);
        }

        public string SubPlatform()
        {
            return (string)(((Hashtable)response["payment"])["sub-platform"]);
        }

        public int PaymentId()
        {
            return Convert.ToInt32(((Hashtable)response["payment"])["id"]);
        }

        public NameValueCollection GetCustomVariables()
        {
            string customVariableString = (string)((Hashtable)response["payment"])["your-variables"];

            if (customVariableString != null)
            {                
                return HttpUtility.ParseQueryString(customVariableString);
            }
            return new NameValueCollection();
            
        }

        public bool PayaloadProvided()
        {
            return Convert.ToBoolean(((Hashtable)response["payment"])["payload-provided"]);
        }

        public string PayalogueUrl()
        {
            return (string)((Hashtable)response["payment"])["payalogue-url"];
        }

    }
}
