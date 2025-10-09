using ClassroomDeviceManagement.Repositories;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomDeviceManagement.Controllers
{
    [Route("api/models")]
    [ApiController]
    public class DeviceModelController : ControllerBase
    {
        private readonly IDeviceModelService _deviceModelService;
        private readonly IDeviceInstanceService _deviceInstanceService;
        public DeviceModelController(IDeviceModelService deviceModelService, IDeviceInstanceService deviceInstanceService   )
        {
            _deviceModelService = deviceModelService;
            _deviceInstanceService = deviceInstanceService;
        }

        [HttpPost]
        public async Task AddModel([FromBody] DeviceModel model)
        {
            await _deviceModelService.AddModelAsync(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceModelById(int id)
        {
            var model = await _deviceModelService.GetById(id);
            return Ok(model);
        }

        [HttpGet("{id}/instances")]
        public async Task<IActionResult> GetAllInstanceByModelId(int id)
        {
            var instances = await _deviceInstanceService.GetAllByModelIdAsync(id);
            return Ok(instances);
        }
    }
}
