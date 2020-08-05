using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

namespace AzureStorageDemo.Models
{
    public class FileModel
    {
        public IFormFile RawFile { get; set; }

        public string Filename { get; set; }

        public string FileType { get; set; }

        public string Directory { get; set; }

        public string Caption { get; set; }

        [DefaultValue(true)]
        public bool IsPublic { get; set; }

        [JsonIgnore]
        public byte[] FileBytes { get; set; }

        public string Data { get; set; }

        public int ResourceId { get; set; }

        public bool IsValid()
        {
            if (RawFile != null)
            {
                Filename = Filename ?? RawFile.FileName;
                using (var stream = new MemoryStream())
                {
                    RawFile.CopyTo(stream);
                    FileBytes = stream.ToArray();
                    var data = Convert.ToBase64String(FileBytes);
                    var index = data.LastIndexOf("base64", StringComparison.Ordinal);
                    Data = index == -1 ? data : data.Substring(index + 7);
                }
            }

            if (string.IsNullOrEmpty(FileType))
            {
                if (!string.IsNullOrEmpty(Filename))
                    FileType = Path.GetExtension(Filename).ToLower();

                if (string.IsNullOrEmpty(FileType))
                {
                    var match = Regex.Match(Data, "data:(.*?);");
                    if (!match.Success)
                        return false;

                    FileType = match.Groups[1].Value;
                }
            }

            if (string.IsNullOrEmpty(FileType))
                return false;

            return true;
        }

        public static byte[] ReadBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }
    }
}