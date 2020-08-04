using AzureStorageDemo.Models;
using AzureStorageDemo.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AzureStorageDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly BlobsService BlobsService;

        public FilesController(BlobsService blobsService)
        {
            BlobsService = blobsService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] FileForm fileForm)
        {
            return Ok(await BlobsService.UploadFileAsync(fileForm));
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFileNames()
        {
            return Ok(await BlobsService.GetAllFilesFromContainer());
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var stream = await BlobsService.DownloadAsync(fileName);
            var contentType = "APPLICATION/octet-stream";
            return File(stream, contentType, fileName);
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            return Ok(await BlobsService.DeleteAsync(fileName));
        }
    }
}