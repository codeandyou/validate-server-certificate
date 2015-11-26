using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ServerCertificateValidate
{
    class ValidateCertificate
    {
        // public key of certificate
        private static String _PUBLICKEY = "3082010a0282010100953b6be2bde72aae46a2c5a1af890ac29764444d27f69ec4745b674784bb3148550038d42f456851a2eac1c9a7ac8aebd8431c74875d4a2a61314047c3da3879bd4b57e932bc33ed3ae342fe500e18515e3e7a0fe682aae70ba04e7c718a49e1570e15b6bb6133a50813f9660d6f820487388c020944cf6ff8222d7213f06456f41985f4815895656ccac76764f2ec704cbce841d1d07e296d3123d4817e572eec8f317bef234677c7f474b56f95b986de5a0b898b54c2bb80d3605079cbb3c48fbe35671c4b467bed69cc6ed192a6b3d9bf916c4c8979fc9716fcb148c1c40ce4beabd4d128beca1759b76a78575b19d4572a9b1caef289ebd20ed85567460d0203010001";

        public static void Main(string[] args)
        {
            // Set remote certificate callBack validation delegate
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

            // Create request
            WebRequest request = WebRequest.Create("https://private-634da8-test11074.apiary-mock.com/SubscriberByWeek");
            request.Timeout = 10000 ;

            //Get response
            WebResponse response = request.GetResponse();

            // Get the stream associated with the response.
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
        }

        // This method will be invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // stop communicate with unauthenticated servers.
            if (certificate == null || chain == null)
                return false;

            // stop communicate with unauthenticated servers.
            if (sslPolicyErrors != SslPolicyErrors.None)
                return false;

            // match certificate public key and allow communicate with authenticated servers.
            String publicekey = certificate.GetPublicKeyString();
            if (publicekey.Equals(_PUBLICKEY.ToUpper()))
                return true;

            // stop communicate with unauthenticated servers.
            return false;
        }

    }
}
