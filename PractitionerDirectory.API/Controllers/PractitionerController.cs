using Microsoft.AspNetCore.Mvc;
using PractitionerDirectory.API.Attributes;
using PractitionerDirectory.API.Mappers;
using PractitionerDirectory.API.Models;
using PractitionerDirectory.Services;
using PractitionerDirectory.Services.Models;

namespace PractitionerDirectory.API.Controllers
{
    [ApiController]
    [Route("practitioners")]
    public class PractitionerController : Controller
    {
        private readonly IPractitionerService _practitionerService;
        private readonly IPractitionerMapper _mapper;

        public PractitionerController(
            IPractitionerService practitionerService,
            IPractitionerMapper mapper)
        {
            _practitionerService = practitionerService;
            _mapper = mapper;
        }

        /// <summary>Получить постраничный список докторов</summary>
        /// <param name="page">Номер старницы. Значение по умолчанию - 1</param>
        /// <param name="pageSize">Размер страницы. Размер по умолчанию - 50</param>
        /// <response code="200">Возвращает страницу докторов</response>
        /// <response code="400">Некорректный запрос</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Produces("application/json")]
        [ProducesResponseType<List<PractitionerDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(GroupName = "Practitioner-v1")]
        [HttpGet]
        public async Task<ActionResult> GetPractitionersPageAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var result = await _practitionerService.GetPractitionersPageAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>Получить доктора по Id в Json формате</summary>
        /// <param name="id">Идентификатор доктора - Id</param>
        /// <response code="200">Возвращает информацию о докторе</response>
        /// <response code="400">Некорректный запрос</response>
        /// <response code="404">Доктор не найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Produces("application/json")]
        [ProducesResponseType<PractitionerDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(GroupName = "Practitioner-v1")]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            var result = await _practitionerService.GetPractitionerByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>Поиск докторов по имени и фамилиии</summary>
        /// <param name="firstName">Имя доктора (поиск по части строки)</param>
        /// <param name="lastName">Фамилия доктора (поиск по части строки)</param>
        /// <param name="resultSize">Размер списка найденых докторов. Значение по умолчанию - 20</param>
        /// <response code="200">Возвращает информацию о докторе</response>
        /// <response code="400">Некорректный запрос</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Produces("application/json")]
        [ProducesResponseType<PractitionerDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(GroupName = "Practitioner-v1")]
        [HttpGet("search")]
        public async Task<ActionResult> GetPractitionersAsync(
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] int resultSize = 20)
        {
            resultSize = Math.Clamp(resultSize, 1, 50);

            var result = await _practitionerService.GetPractitionersAsync(firstName, lastName, resultSize);
            return Ok(result);
        }

        /// <summary>Добаление доктора</summary>
        /// <param name="model">Модель содеращая сведения о докторе</param>
        /// <response code="201">Возращает данные созданного доктора</response>
        /// <response code="400">Некорректный запрос</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Produces("application/json")]
        [ProducesResponseType<PractitionerDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(GroupName = "Practitioner-v1")]
        [ModelValidation]
        [HttpPost]
        public async Task<ActionResult> CreatePractitionerAsync([FromBody] CreatePractitionerRequest model)
        {
            var dtoModel = _mapper.CreatePractitionerRequestToPractitionerDTO(model);

            var result = await _practitionerService.UpsertAsync(dtoModel);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Id }, result);
        }

        /// <summary>Обновление доктора</summary>
        /// <param name="model">Модель содеращая сведения о доктора</param>
        /// <response code="200">Возращает данные обновленного доктора</response>
        /// <response code="400">Некорректный запрос</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Produces("application/json")]
        [ProducesResponseType<PractitionerDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(GroupName = "Practitioner-v1")]
        [ModelValidation]
        [HttpPut]
        public async Task<ActionResult> EditPractitionerAsync([FromBody] PractitionerDTO model)
        {
            var result = await _practitionerService.UpsertAsync(model);
            return Ok(result);
        }

        /// <summary>Удаление доктора по Id</summary>
        /// <param name="id">Идентификатор доктора - Id</param>
        /// <response code="204">Удаление прошло успешно</response>
        /// <response code="400">Некорректный запрос</response>
        /// <response code="404">Доктор на удаление не был найден</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [Produces("application/json")]
        [ProducesResponseType<bool>(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ApiExplorerSettings(GroupName = "Practitioner-v1")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePractitionerAsync([FromRoute] Guid id)
        {
            var result = await _practitionerService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
