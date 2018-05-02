using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Tabi.Shared.DataSync
{
    public class EndpointConfiguration
    {
        private static List<string> keyList = new List<string>();
        public const string PUBKEY = "3082010A0282010100AAFE5A94120262937FCCDA778F0F470DE7EB72767599BEBDF21154215BBCADAF16EC336641FBA1E4C5DB88C5685F048C727D49E2B451CC45C5ACDF711BB1714BE1B13BF151664F9245ED5A8FB84BE727F615A3BD75A760AFC13533C8D9336308962C8A56DB354B300AB6ED131280D0720BC7DF2AB7BF658ADB4FC3F07CD7614DF9CB52F15A2FC42DF4B6630293DCD188A241E4BFA8440F47C17AD7B7C152FD5768350B005F18486D318BEF8680337FA90A1464964D499FD027286B9D9DEB2703FA0F8B4EE1DCABC6E4C35C9D5A0379D4524D93860C03F05382C83CA716BD0BB33C111892A108D2DDD35C70B587584D42101D8BE22000741FC4405345D69D307D0203010001";

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
            else {
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
