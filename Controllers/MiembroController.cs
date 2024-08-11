using ApiGym.Models;
using ApiGym.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGym.Controllers {
    [Route("api/[controller]")]
    public class MiembroController: ControllerBase {
        private readonly IMiembroService _miembroService;
        public MiembroController(IMiembroService miembroService) {
            _miembroService = miembroService;
        }

        [HttpGet]
        public IActionResult GetMiembros(){
            return Ok(_miembroService.MostrarMiembros());
        }

        [HttpGet("miembro")]
        public IActionResult GetMiembroById(int id) {
            return Ok(_miembroService.MostrarMiembroPorId(id));
        }

        [HttpGet("miembro/userid")]
        public IActionResult GetMiembroByUserId(int userId) {
            return Ok(_miembroService.MostrarMiembroPorUserId(userId));
        }

        [HttpPost]
        public async Task<IActionResult> PostMiembro([FromBody] MiembroDTO miembroDTO) {
            try{
                await _miembroService.CrearMiembro(miembroDTO);
                return Ok();
            } catch(Exception ex) {
                return BadRequest(new {message = "Ocurrio un error al intentar crear el usuario", details = ex.Message});
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMiembro(int id, [FromBody] MiembroDTO miembroDTO) {
            try{
                await _miembroService.EditarMiembro(id, miembroDTO);
                return Ok();
            } catch(Exception ex) {
                return BadRequest(new { message = "Error al intentar editar el miembro", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMiembro(int id) {
            try{
                await _miembroService.EliminarMiembro(id);
                return Ok();
            } catch(Exception ex) {
                return BadRequest(new { message = "Error al intentar eliminar el miembro.", details = ex.Message });
            }
        }
    }
}