using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomDeviceManagement.Controllers
{
    [Route("api/instances")]
    [ApiController]

    public class DeviceInstanceController : ControllerBase
    {
        private readonly IDeviceInstanceService _instanceService;
        public DeviceInstanceController(IDeviceInstanceService instanceService)
        {
            _instanceService = instanceService;
        }

        [HttpPost]
        public async Task<IActionResult> AddDeviceInstance([FromBody] DeviceInstance instance)
        {
            var instanceDto = await _instanceService.AddInstanceAsync(instance);
            return Ok(instanceDto);
        }
    }
}
