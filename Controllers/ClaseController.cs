using ApiGym.Models;
using ApiGym.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiGym.Controllers {
    [Route("api/[controller]")]
    public class ClaseController: ControllerBase {
        private readonly IClaseService _claseService;
        public ClaseController(IClaseService claseService)
        {
            _claseService = claseService;
        }

        [HttpGet]
        public IActionResult GetClases() {
            try{
                return Ok(_claseService.MostrarClases);
            } catch(Exception ex) {
                return BadRequest(new { message = "Error al mostrar las clases", details = ex.Message });
            }
        }

        [HttpGet("/Clase/{id}")]
        public async Task<IActionResult> GetClaseById(int id) {
            try{
                return Ok(await _claseService.MostrarClasePorId(id));
            } catch (Exception ex) {
                return BadRequest(new { message = "Error al mostrar la clase", details = ex.Message });
            }
        }

        [HttpPost("/Clase/Crear")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> PostClase([FromBody] CrearClaseDTO crearClaseDTO) {
            try{
                await _claseService.CrearClase(crearClaseDTO);
                return Ok();
            } catch(Exception ex){
                return BadRequest(new { message = "Errol al intentar crear la clase", details = ex.Message });
            }
        }

        [HttpPost("/Clase/Inscribirse/{idClase}")]
        [Authorize(Roles = "Miembro")]
        public async Task<IActionResult> PostInscripcionClase(int idClase) {
            try{
                await _claseService.InscribirseClase(idClase);
                return Ok();
            } catch(Exception ex) {
                return BadRequest(new { message = "Error al intentar inscribirse a la clase", details = ex.Message });
            }
        }
    }
}