using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using PractitionerDirectory.API.Mappers;
using PractitionerDirectory.Services;
using PractitionerDirectory.Services.Models;

namespace PractitionerDirectory.API.Controllers
{
    [ApiController]
    [Route("fhir/practitioners")]
    public class FhirPractitionerController : Controller
    {
        private const string FhirContentType = "application/fhir+json";

        private readonly IPractitionerService _practitionerService;
        private readonly IFhirPractitionerMapper _fhirMapper;
        private readonly FhirJsonSerializer _serializer;

        public FhirPractitionerController(
            IPractitionerService practitionerService,
            IFhirPractitionerMapper fhirMapper,
            FhirJsonSerializer serializer)
        {
            _practitionerService = practitionerService;
            _fhirMapper = fhirMapper;
            _serializer = serializer;
        }

        /// <summary>Получить доктора по Id в формате FHIR R5</summary>
        /// <param name="id">Идентификатор доктора - Id</param>
        /// <response code="200">Возвращает информацию о докторе в формате FHIR</response>
        /// <response code="400">Некорректный запрос/response>
        /// <response code="404">Доктор не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Produces(FhirContentType)]
        [ProducesResponseType<PractitionerDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(GroupName = "Practitioner-v1")]
        [HttpGet("{id}")]
        public async Task<ActionResult> Read([FromRoute] Guid id)
        {
            var modelDTO = await _practitionerService.GetPractitionerByIdAsync(id);

            if (modelDTO == null)
            {
                var errorMessage = $"Доктор с Id = {id} не найден";
                return FhirError(404, OperationOutcome.IssueType.NotFound, errorMessage);
            }

            var model = _fhirMapper.PractitionerDTOToFhir(modelDTO);

            Response.Headers.ETag = $"W/\"{model.Meta?.VersionId}\"";
            Response.Headers.LastModified = model.Meta?.LastUpdated?.ToString("R");

            var json = _serializer.SerializeToString(model);

            return Content(json, FhirContentType);
        }

        /// <summary>Поиск докторов по имени и фамилиии</summary>
        /// <param name="firstName">Имя доктора (поиск по части строки)</param>
        /// <param name="lastName">Фамилия доктора (поиск по части строки)</param>
        /// <param name="count">Максимальное количество записей в ответе</param>
        /// <response code="200">Возвращает информацию о докторе</response>
        /// <response code="400">Некорректный запрос</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Produces(FhirContentType)]
        [ProducesResponseType<PractitionerDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(GroupName = "Practitioner-v1")]
        [HttpGet]
        public async Task<ActionResult> Search(
            [FromQuery] string? family,
            [FromQuery] string? give,
            [FromQuery(Name = "_count")] int count = 20)
        {
            count = Math.Clamp(count, 1, 50);

            var list = await _practitionerService.GetPractitionersAsync(give, family, count);

            var bundle = new Bundle
            {
                Type = Bundle.BundleType.Searchset,
                Total = list.Count,
                Entry = list.Select(p => new Bundle.EntryComponent
                {
                    Resource = _fhirMapper.PractitionerDTOToFhir(p)
                })
                .ToList()
            };

            var json = _serializer.SerializeToString(bundle);

            return Content(json, FhirContentType);
        }

        private ActionResult FhirError(int statusCode, OperationOutcome.IssueType code, string message)
        {
            var outcome = new OperationOutcome
            {
                Issue = new List<OperationOutcome.IssueComponent>()
                {
                    new OperationOutcome.IssueComponent
                    {
                        Severity = OperationOutcome.IssueSeverity.Error,
                        Code = code,
                        Diagnostics = message
                    }
                }
            };

            var json = _serializer.SerializeToString(outcome);

            return new ContentResult
            {
                Content = json,
                ContentType = FhirContentType,
                StatusCode = statusCode
            };
        }
    }
}
