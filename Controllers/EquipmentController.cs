using MicroserviceTest.Services;
using MicroserviceTest.View;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace MicroserviceTest.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly EquipmentDataService _equipmentDataService;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="equipmentDataService">Источник данных</param>
        public EquipmentController(EquipmentDataService equipmentDataService)
        {
            _equipmentDataService = equipmentDataService;
        }

        /// <summary>
        /// Получить ID по названию оборудования
        /// </summary>
        /// <remarks>
        /// *Пример запроса: "Турбогенерат 1/Турбогенератор/Ротор/Скорость вращения/Датчик 2"
        /// </remarks>
        /// <param name="path">Путь оборудования в дереве</param>
        /// <returns></returns>
        /// <response code="200"> уникальный идентификатор оборудования</response>
        /// <response code="400"> отсутствует наименование оборудования в запросе </response>
        /// <response code="404"> не правильно указан путь оборудования в дереве </response>
        /// <response code="406"> значение оборудования по заданному пути отсутствует </response>
        [HttpGet("equipment")]
        [ProducesResponseType(typeof(Equipment), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status406NotAcceptable)]
        public async Task<IActionResult> GetEquipmentID(string path) 
        {
            var serviceResult = await _equipmentDataService.GetIDAsync(path);

            if (serviceResult.IsSuccess) 
            {
                return Ok(serviceResult.Data);
            }
            
            return StatusCode(serviceResult.StatusCode);
        }
    }
}
