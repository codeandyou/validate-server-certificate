# validate-server-certificate
Validate server certificate

Security is very important in software development. We have different ways to secure our application. Like Application Security, Network Security, Database Security etc.

Many times we use SSL Certificate to secure our application. For use SSL Certificate we install certificate into server and configure web server to serve our web pages over secure channel (https). After using secure channel all communication to server use secure channel by encrypt data. It’s more secure our communication to server by secure network.

Think about the scenario when you have a client application which sending data to server but server certificate have some problem or certificate not available on server. Now point is that we need first validate server certificate and if everything fine then start send and receive data from server. For that we need validate server certificate.
ServerCertificateValidationCallback Property is use to gets or sets the callback to validate a server certificate. When we doing certificate validation the sender parameter passed to the RemoteCertificateValidationCallback.
RemoteCertificateValidationCallback Parameters

sender
Type: System.Object
An object that contains state information for this validation.
certificate
Type: System.Security.Cryptography.X509Certificates.X509Certificate
The certificate used to authenticate the remote party.
chain
Type: System.Security.Cryptography.X509Certificates.X509Chain
The chain of certificate authorities associated with the remote certificate.
sslPolicyErrors
Type: System.Net.Security.SslPolicyErrors
One or more errors associated with the remote certificate.

Return Value
Type: System.Boolean
A Boolean value that determines whether the specified certificate is accepted for authentication.



Steps for validate server certificate

1.	Get Public Key from Server Certificate
2.	Create RemoteCertificateValidationCallback Delegate
3.	Match Public Key and Server Certificate Public Key
 
1.	Get Public Key from Server Certificate– We are getting data from following URL

https://private-634da8-test11074.apiary-mock.com/SubscriberByWeek
It’s returning JSON data
[{"RegistedDay":"Tuesday","SubscriberRegisted":4}, 
{"RegistedDay":"Tuesday","SubscriberRegisted":8},
{"RegistedDay":"Wednesday","SubscriberRegisted":10},
{"RegistedDay":"Friday","SubscriberRegisted":12},
{"RegistedDay":"Saturday","SubscriberRegisted":15},
{"RegistedDay":"Saturday","SubscriberRegisted":20}
]
For getting Public key access URL into chrome browser, you will see screen like this.
 






Click on lock icons   it will show you permission screen 
 
Go into “Connection” tab and then “Certificate Information” button
 


It will show you server certificate and its information and then click on “Details” tab
 
Select “All” from drop down and chose Public Key. It will show you Public key
 

Copy this Public Key and remove space between 

Now we have Public key

3082010a0282010100953b6be2bde72aae46a2c5a1af890ac29764444d27f69ec4745b674784bb3148550038d42f456851a2eac1c9a7ac8aebd8431c74875d4a2a61314047c3da3879bd4b57e932bc33ed3ae342fe500e18515e3e7a0fe682aae70ba04e7c718a49e1570e15b6bb6133a50813f9660d6f820487388c020944cf6ff8222d7213f06456f41985f4815895656ccac76764f2ec704cbce841d1d07e296d3123d4817e572eec8f317bef234677c7f474b56f95b986de5a0b898b54c2bb80d3605079cbb3c48fbe35671c4b467bed69cc6ed192a6b3d9bf916c4c8979fc9716fcb148c1c40ce4beabd4d128beca1759b76a78575b19d4572a9b1caef289ebd20ed85567460d0203010001

2.	Create RemoteCertificateValidationCallback Delegate - Create a RemoteCertificateValidationCallback delegate like this. When we doing certificate validation the sender parameter passed to the RemoteCertificateValidationCallback.

// Set remote certificate callBack validation delegate
ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;


3.	Match Public Key and Server Certificate Public Key – In Callback we have sender, certificate, chain, and sslPolicyErrors. First we need to check certificate and any errors in certificate. If yes then return false. 

Otherwise we need to call GetPublicKeyString() method to get Public Key of certificate. And then match of both Public Key first one which we have and second one we got from certificate. 

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

If both Public Key will not match then method will be return false and you will got SSL/TLS exception.

 

 







Example 
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


Output

 







Thanks
www.codeandyou.com

http://www.codeandyou.com/2015/11/how-to-validate-server-certificate.html

Keywords - How to validate server certificate
, validate server certificate, server certificate validation

