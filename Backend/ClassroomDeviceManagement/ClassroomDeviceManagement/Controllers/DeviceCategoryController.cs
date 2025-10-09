using ClassroomDeviceManagement.Dto;
using ClassroomDeviceManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace ClassroomDeviceManagement.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class DeviceCategoryController : ControllerBase
    {
        private readonly IDeviceCategoryService _deviceCategoryServices;
        private readonly IDeviceModelService _deviceModelService;

        public DeviceCategoryController(IDeviceCategoryService services, IDeviceModelService deviceModelService )
        {
            _deviceCategoryServices = services;
            _deviceModelService = deviceModelService;
        }

        /// <summary>
        /// Lấy tất cả loại thiết bị.
        /// </summary>
        /// <returns>Danh sách Category</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllDeviceCategories() 
        {
            var categories = await _deviceCategoryServices.GetAllDeviceCategories();
            return Ok(categories);
        }

        /// <summary>
        /// Lấy thông tin của 1 loại thiết bị bằng Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            DeviceCategoryDto? result = await _deviceCategoryServices.GetCategoryById(id);

            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Lấy tất cả mẫu thiết bị bằng id loại
        /// </summary>
        [HttpGet("{id}/models")]
        public async Task<ActionResult> GetAllModelByCategoryId(int id)
        {
            var models = await _deviceModelService.GetAllByCategoryId(id);
            return Ok(models);
        }

        /// <summary>
        /// Thêm một loại thiết bị.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] DeviceCategoryDto category)
        {
            try
            {
                var addedCategory = await _deviceCategoryServices.AddCategoryAsync(category);
                if (addedCategory != null)
                {
                    return CreatedAtAction(nameof(GetCategoryById), new { id = addedCategory.Id }, addedCategory);
                }
                else
                {
                    return BadRequest(new { Message = "" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Thêm thất bại" });
            }
        }

    }
}
