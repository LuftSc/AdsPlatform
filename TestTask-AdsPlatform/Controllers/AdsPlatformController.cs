using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TestTask_AdsPlatform.Abstractions;
using TestTask_AdsPlatform.Contracts.GetPlatformsList;
using TestTask_AdsPlatform.Contracts.UploadTextFile;
using TestTask_AdsPlatform.Filters;
using TestTask_AdsPlatform.Swagger;

namespace TestTask_AdsPlatform.Controllers
{
    [ApiController]
    [Route("ads-platfroms")]
    [TypeFilter(typeof(OperationResultFilterAttribute))]
    public class AdsPlatformController : ControllerBase
    {
        private readonly IAdsPlatformService _platformService;

        public AdsPlatformController(IAdsPlatformService platformService)
        {
            _platformService = platformService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Получить список рекламных площадок, работающих по данной локации",
                Description = "Возвращает список рекламных площадок, которые работают по указанной локации")]
        public ActionResult GetPlatforms
            ( [FromQuery] GetPlatformsListRequest request
            , CancellationToken cancellationToken = default)

            => Ok(_platformService.GetPlatformsByLocation(request.Location));

        [FileUploadOperation.FileContentType]
        [HttpPost]
        [SwaggerOperation(Summary = "Загрузить список рекламных площадок и локаций, в которых они работают",
                Description = "Поддерживается загрузка только файлов расширения .txt")]
        public async Task<IActionResult> UploadAdsPlatformsFromFile
            ( [FromForm] UploadTextFileRequest request
            , CancellationToken cancellationToken = default)
        {
            using var reader = new StreamReader(request.File.OpenReadStream());
            var content = await reader.ReadToEndAsync(cancellationToken);
            
            _platformService.InitializePlatformsDictionary(content);

            return Ok();
        }
    }
}
