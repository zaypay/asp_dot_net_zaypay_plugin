using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using Zaypay;
using Zaypay.WebService;
using System.Collections;


namespace Zaypay.Test
{

    [TestFixture]
    class PriceSettingTest
    {

        private Hashtable expectedHash;
        private PriceSetting ps;

        [SetUp]
        protected void SetUp()
        {
            
            expectedHash = new Hashtable();
            ps = Substitute.For<PriceSetting>(111, "thekey");
            ps.PAYALOGUE_ID = 111;
            ps.LOCALE = "nl-NL";
            ps.PAYMENT_METHOD_ID = 2;


        }
        
        [Test]
        public void LocalForIP()
        {

            expectedHash["locale"] = "nl-NL";
            ps.GetResponse("https://secure.zaypay.com/82.94.123.123/pay/111/locale_for_ip?key=thekey").Returns(expectedHash);

            
            LocalForIPResponse actualResponse = new LocalForIPResponse(expectedHash);

            CollectionAssert.AreEquivalent(actualResponse.RESPONSE, expectedHash);
            //Assert.AreEqual("nl-NL", ps.LocaleForIP("82.94.123.123"));

        }

        [Test]
        [ExpectedException(typeof(Exception))]

        public void LocaleForIPWithException()
        {
            ps.LocaleForIP("");

        }

        [Test]        
        public void ListLocales()
        {

            ps.GetResponse("https://secure.zaypay.com//pay/111/list_locales?key=thekey").Returns(expectedHash);

            ListLocalesResponse actualResponse = ps.ListLocales();
            CollectionAssert.AreEquivalent(actualResponse.RESPONSE, expectedHash);

            ChangeHashtable(ref expectedHash);

            ps.GetResponse("https://secure.zaypay.com/123/pay/111/list_locales?key=thekey").Returns(expectedHash);
            actualResponse = null;
            actualResponse = ps.ListLocales(123);
            CollectionAssert.AreEquivalent(actualResponse.RESPONSE, expectedHash);

            

        }

        [Test]
        [ExpectedException(typeof(Exception))]

        public void ListLocaleWithException()
        {
            ps.ID = 0;
            ps.KEY = "";
            ps.ListLocales();
        }

        [Test]
        public void ListPaymentMethods()
        {

            ps.GetResponse("https://secure.zaypay.com//nl-NL/pay/111/payments/new?key=thekey").Returns(expectedHash);

            PaymentMethodResponse actualResponse = ps.ListPaymentMethods();
            CollectionAssert.AreEquivalent(actualResponse.RESPONSE, expectedHash);

            ChangeHashtable(ref expectedHash);
            ps.GetResponse("https://secure.zaypay.com/123/nl-NL/pay/111/payments/new?key=thekey").Returns(expectedHash);
            actualResponse = null;
            actualResponse = ps.ListPaymentMethods(123);
            CollectionAssert.AreEquivalent(actualResponse.RESPONSE, expectedHash);

        }

        [Test]
        public void CreatePayment()
        {

            string parameters = "key=thekey&payment_method_id=2&payalogue_id=111";
            string url = "https://secure.zaypay.com//nl-NL/pay/111/payments/create?" + parameters;

            ps.GetResponse(url).Returns(expectedHash);
            
            // for post data
            //ps.GetResponse("https://secure.zaypay.com//nl-NL/pay/111/payments", "POST", parameters).Returns(expectedHash);

            PaymentResponse actualResponse = ps.CreatePayment();
            CollectionAssert.AreEquivalent(actualResponse.RESPONSE, expectedHash);

            //ChangeHashtable(ref expectedHash);

            //ps.GetResponse("https://secure.zaypay.com/123/nl-NL/pay/111/payments", "POST", parameters).Returns(expectedHash);
            //actualResponse = null;
            //actualResponse = ps.CreatePayment();
            //CollectionAssert.AreEquivalent(actualResponse.RESPONSE, expectedHash);

        }

        [ExpectedException(typeof(Exception), ExpectedMessage = "PayalogueId, PaymentMethodId or Locale is not set")]
        [Test]
        public void CreatePaymentWithLocaleNotSet()
        {

            // set wrong locale
            ps.LOCALE = "";
            PaymentResponse actualResponse = ps.CreatePayment();

        }

        [ExpectedException(typeof(Exception), ExpectedMessage = "PayalogueId, PaymentMethodId or Locale is not set")]
        [Test]
        public void CreatePaymentWithPaymentMethodIdNotSet()
        {

            // set wrong payment method id
            ps.PAYMENT_METHOD_ID = 0;
            PaymentResponse actualResponse = ps.CreatePayment();

        }


        [ExpectedException(typeof(Exception), ExpectedMessage = "PriceSetting ID or PriceSetting KEY are not correct")]
        [Test]
        public void CreatePaymentWithPriceSettingNotSet()
        {
            ps.ID = 0;
            PaymentResponse actualResponse = ps.CreatePayment();

        }

        [ExpectedException(typeof(Exception), ExpectedMessage = "PriceSetting ID or PriceSetting KEY are not correct")]
        [Test]
        public void CreatePaymentWithPriceSettingKeyNotSet()
        {
            ps.KEY = "   ";
            PaymentResponse actualResponse2 = ps.CreatePayment();
        }

        [Test]
        public void CreatePaymentWithObject()
        {
            Hashtable hash = new Hashtable();
            hash.Add("payment", new Hashtable());
            hash.Add("status-string", "Your payment is prepared");
            hash.Add("short-instructions", "short instructions");
            hash.Add("long-instructions", "long instructions");
            hash.Add("very-short-instructions", "very short instructions");
            hash.Add("very-short-instructions-with-amount", "very short instructions with amount");


            ((Hashtable)(hash["payment"])).Add("id", 123);
            ((Hashtable)(hash["payment"])).Add("locale", "nl-NL");
            ((Hashtable)(hash["payment"])).Add("verification-needed", "true");
            ((Hashtable)(hash["payment"])).Add("verification-tries-left", "2");
            ((Hashtable)(hash["payment"])).Add("status", "prepared");
            ((Hashtable)(hash["payment"])).Add("payment-method-id", "2");
            ((Hashtable)(hash["payment"])).Add("platform", "sms");
            ((Hashtable)(hash["payment"])).Add("payload-provided", "false");
            ((Hashtable)(hash["payment"])).Add("payalogue-url", "www.face.com");

            string parameters = "key=thekey&payment_method_id=2&payalogue_id=111";
            string url = "https://secure.zaypay.com//nl-NL/pay/111/payments/create?" + parameters;

            ps.GetResponse(url).Returns(hash);

            PaymentResponse response = ps.CreatePayment();

            Assert.AreEqual(123, response.PaymentId());
            Assert.AreEqual(2, response.VerificationTriesLeft());
            Assert.AreEqual(true, response.VerificationNeeded());
            Assert.AreEqual("Your payment is prepared", response.StatusString());


            

            
            
        }

        [Test]

        public void EmptyPaymentCalls()
        {
            string parameters = "key=thekey&payment_method_id=2&payalogue_id=111";
            string url = "https://secure.zaypay.com//nl-NL/pay/111/payments/create?" + parameters;

            ps.GetResponse(url).Returns(new Hashtable());

            PaymentResponse response = ps.CreatePayment();

            Assert.AreEqual(null, response.Payment());
            
            
        }

        private void ChangeHashtable(ref Hashtable hash)
        {
            hash = null;
            hash = new Hashtable();
            hash["amount"] = "123";
        }



    }
}


