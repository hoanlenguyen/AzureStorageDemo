using Newtonsoft.Json;
using System.ComponentModel;

namespace AzureStorageDemo.Models
{
    public class FileModel
    {
        public string Data { get; set; }

        public string Filename { get; set; }

        public string FileType { get; set; }

        public string Directory { get; set; }

        public string Caption { get; set; }

        [DefaultValue(true)]
        public bool IsPublic { get; set; }

        [JsonIgnore]
        public byte[] FileBytes { get; set; }

        public int ResourceId { get; set; }
    }
}