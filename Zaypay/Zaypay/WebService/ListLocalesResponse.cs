using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Zaypay.WebService;


namespace Zaypay.WebService
{
    public class ListLocalesResponse : Response
    {


        public ListLocalesResponse(Hashtable resp)
        {
            response = resp;
        }

        public List<Hashtable> Countries()
        {
            return (List<Hashtable>)((Hashtable)response["countries"])["country"];
        }

        public List<Hashtable> Languages()
        {
            return (List<Hashtable>)((Hashtable)response["languages"])["language"];
        }

        public bool CountrySupported(string cn)
        {
            List<Hashtable> countries = Countries();

            if (countries.Count > 0)
            {
                foreach (Hashtable country in countries)
                {
                    string code = country["code"].ToString();

                    if (code.ToLower().Equals(cn.ToLower()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
