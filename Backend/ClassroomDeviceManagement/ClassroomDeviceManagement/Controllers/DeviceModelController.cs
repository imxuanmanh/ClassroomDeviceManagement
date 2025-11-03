using ClassroomDeviceManagement.Repositories;
using ClassroomDeviceManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClassroomDeviceManagement.Services.Interfaces;

namespace ClassroomDeviceManagement.Controllers
{

    /// <summary>
    /// Controller để quản lý các mẫu thiết bị (Device Models).
    /// </summary>
    [Route("api/models")]
    [ApiController]
    public class DeviceModelController : ControllerBase
    {
        private readonly IDeviceModelService _deviceModelService;
        private readonly IDeviceInstanceService _deviceInstanceService;

        /// <summary>
        /// Constructor khởi tạo controller quản lý mẫu thiết bị.
        /// </summary>
        public DeviceModelController(IDeviceModelService deviceModelService, IDeviceInstanceService deviceInstanceService   )
        {
            _deviceModelService = deviceModelService;
            _deviceInstanceService = deviceInstanceService;
        }

        /// <summary>
        /// Thêm một mẫu thiết bị mới.
        /// </summary>
        [HttpPost]
        public async Task AddModel([FromBody] DeviceModel model)
        {
            await _deviceModelService.AddModelAsync(model);
        }

        /// <summary>
        /// Lấy thông tin của một mẫu thiết bị theo ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceModelById(int id)
        {
            var model = await _deviceModelService.GetById(id);
            return Ok(model);
        }

        /// <summary>
        /// Lấy danh sách thiết bị cụ thể theo ID mẫu thiết bị.
        /// </summary>
        [HttpGet("{id}/instances")]
        public async Task<IActionResult> GetAllInstanceByModelId(int id)
        {
            var instances = await _deviceInstanceService.GetAllByModelIdAsync(id);
            return Ok(instances);
        }
    }
}
