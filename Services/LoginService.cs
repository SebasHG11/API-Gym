using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiGym.Data;
using ApiGym.Models;
using Microsoft.IdentityModel.Tokens;

namespace ApiGym.Services {
    public class LoginService : ILoginService {
        private readonly GymContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccesor;
        public LoginService(GymContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor) 
        {
            _context = context;   
            _config = config;
            _httpContextAccesor = httpContextAccessor;
        }

        public Usuario Autenticar(LoginUser loginUser){
            var usuarioActual = _context.Usuarios.FirstOrDefault(u => u.UserName == loginUser.UserName);

            if(usuarioActual != null && usuarioActual.HashContraseña == loginUser.Contraseña) {
                return usuarioActual;
            }
            return null;
        }
        public string GenerarToken(Usuario usuario) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("Id", usuario.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, usuario.UserName),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Usuario ObtenerUsuarioAutenticado() {
            var httpContext = _httpContextAccesor.HttpContext;
            var identity = httpContext?.User.Identity as ClaimsIdentity;

            if(identity != null) {
                var userClaims = identity.Claims;
                return new Usuario
                {
                    Id = int.Parse(userClaims.FirstOrDefault(u => u.Type == "Id")?.Value),
                    UserName = userClaims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value,
                    Rol = userClaims.FirstOrDefault(u => u.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }
    }
    public interface ILoginService{
        Usuario Autenticar(LoginUser loginUser);
        string GenerarToken(Usuario usuario);
        Usuario ObtenerUsuarioAutenticado();
    }
}