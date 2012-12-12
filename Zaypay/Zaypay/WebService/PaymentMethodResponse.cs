using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Zaypay.WebService;

namespace Zaypay.WebService
{
    public class PaymentMethodResponse : Response
    {

        public PaymentMethodResponse(Hashtable resp)
        {
            response = resp;
        }

        public List<Hashtable> PaymentMethods()
        {
            List<Hashtable> paymentMethodList = new List<Hashtable>();

            //if (response.ContainsKey("payment-methods"))
            //{
            Hashtable payMethods = (Hashtable)response["payment-methods"];
                
            if (payMethods["payment-method"] is System.Collections.Hashtable)                               
                paymentMethodList.Add((Hashtable)payMethods["payment-method"]);                
            else if (payMethods["payment-method"] is System.Collections.Generic.List<System.Collections.Hashtable>)                   
                paymentMethodList = (List<Hashtable>)payMethods["payment-method"];
                
            //}
            
            return paymentMethodList;
        }
    }
}
