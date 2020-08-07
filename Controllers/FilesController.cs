using AzureStorageDemo.Data.Model;
using AzureStorageDemo.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AzureStorageDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly BlobsStorageService BlobsService;

        public FilesController(BlobsStorageService blobsService)
        {
            BlobsService = blobsService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] FileModel file)
        {
            return Ok(await BlobsService.UploadFileAsync(file));
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFileNames()
        {
            return Ok(await BlobsService.GetAllFilesFromContainer());
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var stream = await BlobsService.DownloadAsync(fileName);
            var contentType = "APPLICATION/octet-stream";
            return File(stream, contentType, fileName);
        }

        [HttpGet("getUrl/{fileName}")]
        public async Task<IActionResult> GetUrl(string fileName)
        {
            return Ok(await BlobsService.GetFileUrl(fileName));
        }

        [HttpGet("container/policy")]
        public async Task<IActionResult> GetAccessPolicy()
        {
            return Ok(await BlobsService.GetContainerPolicy());
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            return Ok(await BlobsService.DeleteAsync(fileName));
        }
    }
}