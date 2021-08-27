using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NZMSA_HYD.KeyVault
{
    public static class AzureKeyVault
    {
        public static string GetKey(string keyVaultName)
        {

            var options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                }
            };

            string keyVaultURI = Startup.Configuration["KeyVault:URI"];
            var secretClient = new SecretClient(new Uri(keyVaultURI), new DefaultAzureCredential(), options);

            KeyVaultSecret secret = secretClient.GetSecret(keyVaultName);
            var key = secret.Value;

            return key;
        }
    }
}
