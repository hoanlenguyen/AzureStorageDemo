using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureStorageDemo.Models;
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

        //private readonly CloudBlobClient _client;
        //private readonly CloudBlobContainer _container;
        private string connectionString { set; get; }

        private BlobContainerClient containerClient { set; get; }

        private BlobServiceClient blobServiceClient { set; get; }

        public BlobsStorageService()
        {
            connectionString = Environment.GetEnvironmentVariable(CONN_STRING);
            //blobServiceClient = new BlobServiceClient(connectionString);
            containerClient = new BlobContainerClient(connectionString, CONTAINER_NAME);
            containerClient.CreateIfNotExistsAsync();
        }

        public string GetConnectionString()
        {
            return connectionString;
        }

        public async Task<bool> UploadFileAsync(FileForm fileForm)
        {
            var file = fileForm.ToFileModel();
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