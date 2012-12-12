using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections;
using System.Xml.Serialization;
using Zaypay;
using Zaypay.WebService;
using System.Collections.Specialized;
using Zaypay.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zaypay
{
    public class PriceSetting
    {
        int id;
        string key;
        string locale;
        int paymentMethodID;
        int payalogueID;
        
        string baseUrl = "https://secure.zaypay.com";


        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public int PAYALOGUE_ID
        {
            get { return payalogueID; }
            set { payalogueID = value; }
        }


        public int PAYMENT_METHOD_ID
        {
            get { return paymentMethodID; }
            set { paymentMethodID = value; }
        }


        public string LOCALE
        {
            get { return locale; }
            set { locale = value; }
        }

        public string KEY
        {
            get { return key; }
            set { key = value; }
        }

        public PriceSetting(int pId = 0, string pKey = null)
        {
            
            if (pId != 0 && pKey != null)
            {
                ID = pId;
                KEY = pKey;
            }
            else
            {

                try
                {
                    string path = HttpContext.Current.Server.MapPath("~/App_Data/Zaypay.json");
                    StreamReader reader = File.OpenText(path);
                    JToken config = JToken.ReadFrom(new JsonTextReader(reader));

                    ID = pId == 0 ? config.SelectToken("default").Value<int>() : pId;
                    KEY = config.SelectToken(ID.ToString()).Value<string>();
                }
                catch(Exception ex)
                {
                    ex.Data["type"] = "ZaypayConfig";
                    ex.Data["user_message"] = "Configuration Setting error";                   
                    throw ex;
                }
            }                       
            
        }

        public ListLocalesResponse ListLocales(int amount = 0)
        {

            CheckForInitialSettings();

            string amt = (amount == 0) ? "" : amount.ToString();
            string url = baseUrl + '/' + amt + "/pay/" + ID + "/list_locales?key=" + KEY;
            return new ListLocalesResponse(GetResponse(url));

        }

        public LocalForIPResponse LocaleForIP(string ip)
        {
            CheckForInitialSettings();

            string url = baseUrl + '/' + ip + "/pay/" + ID + "/locale_for_ip?key=" + KEY;

            if (!String.IsNullOrWhiteSpace(ip)){
                return new LocalForIPResponse(GetResponse(url));
            }                
            else
            {                
                throw CreateException("IpEmpty", message: "Ip cannot be an empty string", sameMessage: true);                
            }                
            
        }

        public PaymentMethodResponse ListPaymentMethods(int amount = 0)
        {

            CheckForInitialSettings();

            if (LocaleIsSet())
            {
                string amt = (amount == 0) ? "" : amount.ToString();
                string url = baseUrl + '/' + amt + "/" + locale + "/pay/" + ID + "/payments/new?key=" + KEY;
                return new PaymentMethodResponse(GetResponse(url));
            }
            else
            {                
                throw CreateException("LocaleNotSet", message: "Locale is not set", sameMessage: true);
            }                

        }

        
        public PaymentResponse CreatePayment(NameValueCollection options = null)
        {
            options = options == null ? new NameValueCollection() : options;

            CheckForInitialSettings();

            if (AllValuesAreSet())
            {

                string url = baseUrl + '/' + options["amount"] + '/' + locale + "/pay/" + ID + "/payments/create?";
                options.Remove("amount");

                string parameters = GetQueryString(ref options);
                url += parameters;

                return new PaymentResponse(GetResponse(url));
            }
            else
            {                
                throw CreateException("InitalsNotSet", message: "PayalogueId, PaymentMethodId or Locale is not set");
            }                

        }

        public PaymentResponse ShowPayment(int paymentID)
        {
            CheckForInitialSettings();

            if (paymentID > 0)
            {
                string url = baseUrl + "///pay/" + ID + "/payments/" + paymentID + "?key=" + KEY;
                return new PaymentResponse(GetResponse(url));
            }
            else
            {               
                throw CreateException("PaymentError", message: "Payment ID is not correct", userMessage: "Payment Authorization Error");
            }

        }
            
        public PaymentResponse VerificationCode(int paymentID, string code)
        {

            if (!String.IsNullOrWhiteSpace(code) && paymentID > 0)
            {
                string url = baseUrl + "///pay/" + ID + "/payments/" + paymentID + "/verification_code";
                string parameters = "key=" + KEY + "&verification_code=" + code;
                return new PaymentResponse(GetResponse(url, "POST", parameters));
            }
            else
            {                
                throw CreateException("VerificationError", message: "verification code or payment id are not correct", userMessage: "verification code cannot be an empty string");
            }                

        }

        public PaymentResponse MarkPayloadProvided(int paymentID)
        {

            if (paymentID > 0)
            {
                string url = baseUrl + "///pay/" + ID + "/payments/" + paymentID + "/mark_payload_provided";
                string parameters = "key=" + KEY;
                return new PaymentResponse(GetResponse(url, "POST", parameters));
            }
            else
            {                
                throw CreateException("PaymentError", "Payment ID is not correct", userMessage: "Payment Authorization Error");
            }
                
            

        }

        public virtual Hashtable GetResponse(string url, string method = "GET", string parameters = "")
        {
                        
            Hashtable response = new Hashtable();
            
            try
            {
                HttpRequestResp service = new HttpRequestResp(url, method, parameters);
                
                response = service.GetResponse();

                if (response["error"] != null)
                    throw new XmlException((string)response["error"]);
                    

            }
            catch (System.Net.WebException e)
            {              
                response["error"] = "Http exception occured with the following messag:  " + e.Message;
                throw e;
                //throw new System.Net.WebException((string)response["error"]);                
            }
            catch (XmlException e)
            {                
                response["error"] = "XML is not correct. Following error occured:  " + e.Message;                
                throw e;
                //throw new XmlException((string)response["error"]);                
            }
            catch (Exception e)
            {                
                response["error"] = "Exception occured with the following message:  " + e.Message;
                throw CreateException("UnknownError", e.Message, userMessage: "Request could not be processed");                
            }

            return response;

        }

        private string ConvertToQueryString(NameValueCollection qs)
        {        
            return string.Join("&", Array.ConvertAll(qs.AllKeys, key => string.Format("{0}={1}", key, qs[key])));
        }


        private string SetInitialQueryString()
        {
            string parameters = "key=" + KEY + "&payment_method_id=" + PAYMENT_METHOD_ID;

            if (PayalogueIdIsSet())
                parameters += "&payalogue_id=" + PAYALOGUE_ID;

            return parameters;
        }

        private string GetQueryString(ref NameValueCollection options)
        {

            string parameters = SetInitialQueryString();

            if (options.Count > 0)
            {
                parameters += "&" + ConvertToQueryString(options);                
            }
            return parameters;

        }
            

        private Hashtable BadResponse(string error)
        {

            Hashtable errorResponse = new Hashtable();
            errorResponse["error"] = error;
            return errorResponse;
        }

        private bool AllValuesAreSet()
        {
            
            return (PaymentMethodIdIsSet() && LocaleIsSet()) ? true : false;
        }

        private bool LocaleIsSet()
        {            
            return !String.IsNullOrEmpty(LOCALE);
        }

        private bool PayalogueIdIsSet()
        {
            return IsNullOrDefault(payalogueID) ? false : true;
        }

        private bool PaymentMethodIdIsSet()
        {
            return IsNullOrDefault(paymentMethodID) ? false : true;

        }

        private bool IsNullOrDefault<T>(T value)
        {
            return object.Equals(value, default(T));
        }        

        private void CheckForInitialSettings()
        {
            if (ID <= 0 || String.IsNullOrWhiteSpace(KEY))            
                throw CreateException("ZaypayConfig", "PriceSetting ID or PriceSetting KEY are not correct");
                
        }

        private Exception CreateException(string type, string message = "", string userMessage = "Config setting error", bool sameMessage = false)
        {
            Exception e = new Exception(message);
            
            e.Data["type"] = type;
            if (sameMessage)            
                e.Data["user_message"] = message; 
            else
                e.Data["user_message"] = userMessage;             
            
            return e;
        }
        
        public static void Main()
        {
            PriceSetting ps = new PriceSetting(pId: 140494, pKey: "4c60870f5906a9b16507a62e96f0860f");
            
            
        }
    }   
}
