using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Tabi.Shared.DataSync
{
    public class EndpointConfiguration
    {
        private static List<string> keyList = new List<string>();

        public static void AddPublicKeyString(string key)
        {
            keyList.Add(key);
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool valid = false;

            if (App.DebugMode)
            {
                valid = true;
            }
            else
            {
                foreach (string key in keyList)
                {
                    valid = key.ToUpper() == certificate?.GetPublicKeyString();
                    if (valid)
                    {
                        break;
                    }
                }
            }

            if (!valid)
            {
                Log.Info($"Connection was MITMed: {certificate?.Subject}, {certificate?.GetPublicKeyString()}");
            }

            return valid;
        }
    }
}
