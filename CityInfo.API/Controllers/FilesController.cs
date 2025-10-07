using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Runtime.CompilerServices;

namespace CityInfo.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {

        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? throw new ArgumentNullException(nameof(fileExtensionContentTypeProvider));
        }
        [HttpGet("{fileid}")]
        public ActionResult GetFiles(int fileid)
        {
            var filePath = "Eureka Park Phase 2 - Update as on Sep 2025.pdf";

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            
            if(!_fileExtensionContentTypeProvider.TryGetContentType(filePath, out var contentType) == false)
            {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));

        }
    }
}
