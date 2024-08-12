using ApiGym.Models;
using ApiGym.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGym.Controllers {
    [Route("api/[controller]")]
    public class InstructorController: ControllerBase {
        private readonly IInstructorService _instructorService;
        public InstructorController(IInstructorService instructorService) {
            _instructorService = instructorService;
        }

        [HttpGet]
        public IActionResult GetInstructores() {
            return Ok(_instructorService.MostrarInstructores());
        }

        [HttpGet("/instructor/{id}")]
        public async Task<IActionResult> GetInstructorById(int id) {
            return Ok(await _instructorService.MostrarInstructorPorId(id));
        }

        [HttpGet("instructor/userid")]
        public async Task<IActionResult> GetInstructorByUserId(int userId) {
            return Ok(await _instructorService.MostrarInstructorPorUserId(userId));
        }

        [HttpPost]
        public async Task<IActionResult> PostInstructor([FromBody] InstructorDTO instructorDTO) {
            try{
                await _instructorService.CrearInstructor(instructorDTO);
                return Ok();
            } catch (Exception ex) {
                return BadRequest(new { message = "Error al crear instructor", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInstructor(int id, [FromBody] InstructorDTO instructorDTO) {
            try{
                await _instructorService.EditarInstructor(id, instructorDTO);
                return Ok();
            } catch(Exception ex) {
                return BadRequest(new { message = "Error al editar instructor", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstructor(int id) {
            try{
                await _instructorService.EliminarInstructor(id);
                return Ok();
            } catch(Exception ex) {
                return BadRequest(new { message = "Error al intentar eliminar el instructor", details = ex.Message });
            }
        }
    }
}