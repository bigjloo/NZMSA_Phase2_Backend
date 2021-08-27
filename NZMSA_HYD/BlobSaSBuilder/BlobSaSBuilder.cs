using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NZMSA_HYD.KeyVault;

namespace NZMSA_HYD.BlobSaSBuilder
{
    public class BlobStorageService : Controller
    {
        private readonly string _accountName;

        public BlobStorageService()
        {
            _accountName = Startup.Configuration["AzureBlob:AccountName"];
        }


        public string GetAccountSasToken()
        {
            // TODO set proper resource restrictions
            var sasBuilder = new AccountSasBuilder()
            {
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-2),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30),
                Services = AccountSasServices.Blobs,
                ResourceTypes = AccountSasResourceTypes.All,
                Protocol = SasProtocol.HttpsAndHttp
            };

            sasBuilder.SetPermissions(AccountSasPermissions.All);

            //var blobAccountKey = Startup.Configuration["AzureBlob:Key"];
            var blobAccountKey = AzureKeyVault.GetKey(Startup.Configuration["KeyVault:AzureStorageBlob"]);
            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_accountName, blobAccountKey));

            return sasToken.ToString();
        }
    }
}
