using ApiGym.Data;
using ApiGym.Models;

namespace ApiGym.Services {
    public class LoginService : ILoginService {
        private readonly GymContext _context;
        public LoginService(GymContext context) 
        {
            _context = context;    
        }

        public Usuario Autenticar(LoginUser loginUser){
            var usuarioActual = _context.Usuarios.FirstOrDefault(u => u.UserName == loginUser.UserName);

            if(usuarioActual != null && usuarioActual.HashContraseña == loginUser.Contraseña) {
                return usuarioActual;
            }
            return null;
        }
    }
    public interface ILoginService{
        Usuario Autenticar(LoginUser loginUser);
    }
}