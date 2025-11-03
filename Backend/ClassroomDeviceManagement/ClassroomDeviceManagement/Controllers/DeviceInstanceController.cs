using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Models;
using ClassroomDeviceManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClassroomDeviceManagement.Controllers
{
    /// <summary>
    /// Controller quản lý danh mục loại thiết bị và các mẫu thiết bị tương ứng.
    /// </summary>
    [Route("api/instances")]
    [ApiController]

    public class DeviceInstanceController : ControllerBase
    {
        private readonly IDeviceInstanceService _instanceService;

        /// <summary>
        /// Constructor khởi tạo controller với các service phụ thuộc.
        /// </summary>
        public DeviceInstanceController(IDeviceInstanceService instanceService)
        {
            _instanceService = instanceService;
        }

        /// <summary>
        /// Thêm một instance
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddDeviceInstance([FromBody] DeviceInstance instance)
        {
            var instanceDto = await _instanceService.AddInstanceAsync(instance);
            return Ok(instanceDto);
        }
    }
}
