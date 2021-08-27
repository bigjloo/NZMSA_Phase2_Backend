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
        // App's Azure Storage Blob account name
        private readonly string _accountName;

        public BlobStorageService()
        {
            _accountName = Startup.Configuration["AzureBlob:AccountName"];
        }


        // Returns a SaSToken for authorizing the user to create containers
        // and upload images in apps Azure Storage Blob account
        public string GetAccountSasToken()
        {
            var sasBuilder = new AccountSasBuilder()
            {
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-2),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30),
                Services = AccountSasServices.Blobs,
                ResourceTypes = AccountSasResourceTypes.All,
                Protocol = SasProtocol.HttpsAndHttp
            };

            sasBuilder.SetPermissions(AccountSasPermissions.All);

            var blobAccountKey = AzureKeyVault.GetKey(Startup.Configuration["KeyVault:AzureStorageBlob"]);
            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_accountName, blobAccountKey));

            return sasToken.ToString();
        }
    }
}
