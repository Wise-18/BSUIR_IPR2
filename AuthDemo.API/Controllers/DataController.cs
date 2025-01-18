using AuthDemo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthDemo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;

        public DataController(ILogger<DataController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DataModel>>> Get()
        {
            var data = new List<DataModel>
        {
            new() { Id = 1, Name = "Test Data", Description = "Public data" }
        };

            return Ok(data);
        }

        [HttpPost]
        [Authorize(Policy = "PowerUser")]
        public async Task<ActionResult<DataModel>> Create([FromBody] DataModel model)
        {
            try
            {
                _logger.LogInformation("User {User} creating new data item: {Name}",
                    User.Identity?.Name, model.Name);

                // Здесь была бы логика создания
                return CreatedAtAction(nameof(Get), new { id = model.Id }, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating data item");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "PowerUser")]
        public async Task<ActionResult<DataModel>> Update(int id, [FromBody] DataModel model)
        {
            try
            {
                _logger.LogInformation("User {User} updating data item: {Id}",
                    User.Identity?.Name, id);

                if (id != model.Id)
                    return BadRequest(new { message = "Id mismatch" });

                // Здесь была бы логика обновления
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating data item");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "PowerUser")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("User {User} deleting data item: {Id}",
                    User.Identity?.Name, id);

                // Здесь была бы логика удаления
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting data item");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
