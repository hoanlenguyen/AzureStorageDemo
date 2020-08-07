using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using AzureStorageDemo.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageDemo.Service
{
    public class BlobsStorageService
    {
        private readonly string CONN_STRING = "AZURE_STORAGE_CONNECTION_STRING";
        private readonly string CONTAINER_NAME = "test";
        private readonly string AZURE_STORAGE_HOST = "http://127.0.0.1:10000";
        private readonly string ACCOUNT_NAME = "devstoreaccount1";
        private readonly string END_POINT = "https://127.0.0.1:10000/devstoreaccount1";
        private string connectionString { set; get; }

        private BlobContainerClient containerClient { set; get; }

        private BlobServiceClient blobServiceClient { set; get; }

        public BlobsStorageService()
        {
            connectionString = Environment.GetEnvironmentVariable(CONN_STRING);
            blobServiceClient = new BlobServiceClient(connectionString);
            containerClient = new BlobContainerClient(connectionString, CONTAINER_NAME);
            containerClient.CreateIfNotExistsAsync();
            //containerClient.SetAccessPolicy(PublicAccessType.Blob, permissions)
            //var con = blobServiceClient.GetBlobContainerClient(CONTAINER_NAME);
        }

        public string GetConnectionString()
        {
            return connectionString;
        }

        public async Task<bool> UploadFileAsync(FileModel file)
        {
            if (!file.IsValid())
                return false;

            BlobClient blobClient = containerClient.GetBlobClient(file.Filename);
            MemoryStream stream = new MemoryStream(file.FileBytes);
            await blobClient.UploadAsync(stream, true);
            return true;
        }

        public async Task<List<string>> GetAllFilesFromContainer()
        {
            var list = new List<string>();
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                list.Add(blobItem.Name);
            }
            return list;
        }

        public async Task<string> GetFileUrl(string fileName)
        {
            //BlobClient blobClient = containerClient.GetBlobClient(fileName);
            //if (! await blobClient.ExistsAsync())
            //    return null;

            //return blobClient.Uri.AbsoluteUri;

            ////var key = await blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow,
            ////                                                       DateTimeOffset.UtcNow.AddDays(7));
            //string blobEndpoint = string.Format("https://{0}.blob.core.windows.net", ACCOUNT_NAME);
            string blobEndpoint = END_POINT;
            BlobServiceClient blobClient = new BlobServiceClient(new Uri(blobEndpoint),
                                                                 new DefaultAzureCredential());

            UserDelegationKey key = await blobClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow,
                                                                   DateTimeOffset.UtcNow.AddDays(7));
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = CONTAINER_NAME,
                BlobName = fileName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };

            sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

            string sasToken = sasBuilder.ToSasQueryParameters(key, ACCOUNT_NAME).ToString();

            UriBuilder fullUri = new UriBuilder()
            {
                Scheme = "https",
                Host = /*string.Format("{0}.blob.core.windows.net", accountName)*/END_POINT,
                Path = string.Format("{0}/{1}", CONTAINER_NAME, fileName),
                Query = sasToken
            };

            return fullUri.Uri.AbsoluteUri;
        }

        public async Task<string> GetContainerPolicy()
        {
            var policy = await containerClient.GetAccessPolicyAsync();
            return policy.Value.BlobPublicAccess.ToString();
        }

        public async Task<Stream> DownloadAsync(string fileName)
        {
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            BlobDownloadInfo download = await blobClient.DownloadAsync();
            return download.Content;
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            if (blobClient == null)
                return false;

            await blobClient.DeleteIfExistsAsync();
            return true;
        }
    }
}