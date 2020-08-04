using AzureStorageDemo.Service;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly BlobsService BlobsService;

        public ConfigurationController(BlobsService blobsService)
        {
            BlobsService = blobsService;
        }

        [HttpGet("connection")]
        public IActionResult GetConnection()
        {
            return Ok(BlobsService.GetConnectionString());
        }
    }
}