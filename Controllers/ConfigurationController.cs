using AzureStorageDemo.Service;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly BlobsStorageService BlobsService;

        public ConfigurationController(BlobsStorageService blobsService)
        {
            BlobsService = blobsService;
        }

        [HttpGet("azureblob/connectionstring")]
        public IActionResult GetConnection()
        {
            return Ok(BlobsService.GetConnectionString());
        }
    }
}