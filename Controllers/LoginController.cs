using ApiGym.Models;
using ApiGym.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGym.Controllers {
    [Route("api/[controller]")]
    public class LoginController : ControllerBase {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService) {
            _loginService = loginService;
        }

        [HttpPost]
        public IActionResult PostLogin ([FromBody] LoginUser loginUser) {
            var usuario = _loginService.Autenticar(loginUser);

            if(usuario != null) {
                var token = _loginService.GenerarToken(usuario);
                return Ok(token);
            }

            return NotFound("Usuario no encontrado");
        }

        [HttpGet("current")]
        public IActionResult GetUsuarioAutenticado() {
            try{
                var usuarioAutenticado = _loginService.ObtenerUsuarioAutenticado();

                if(usuarioAutenticado == null) {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                return Ok(usuarioAutenticado);
            } catch(Exception ex) {
                return BadRequest(new { message = "Error al intentar traer el usuario actual", details = ex.Message });
            }
        }
    }
}