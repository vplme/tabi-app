using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Tabi.Logging;

namespace Tabi.DataSync
{
    public static class EndpointValidator
    {
        public static void Init(IEnumerable<string> publicKeys)
        {
            keyList.AddRange(publicKeys);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 8;
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
        }

        private static List<string> keyList = new List<string>();

        public static void AddPublicKeyString(string key)
        {
            keyList.Add(key);
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool valid = false;

            if (keyList.Count == 0)
            {
                valid = true;
            }
            else
            {
                foreach (string key in keyList)
                {
                    valid = key == certificate?.GetPublicKeyString();
                    if (valid)
                    {
                        break;
                    }
                }
            }

            if (!valid)
            {
                Log.Info($"Connection denied: {certificate?.Subject}, {certificate?.GetPublicKeyString()}");
            }

            return valid && sslPolicyErrors == SslPolicyErrors.None;
        }
    }
}
