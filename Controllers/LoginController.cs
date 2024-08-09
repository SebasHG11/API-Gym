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
    }
}