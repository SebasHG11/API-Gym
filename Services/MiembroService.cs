using ApiGym.Data;
using ApiGym.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGym.Services {
    public class MiembroService: IMiembroService {
        private readonly GymContext _context;
        public MiembroService(GymContext context)
        {
            _context = context;
        }
        public IEnumerable<MiembroDTO> MostrarMiembros() {
            var Miembros = _context.Miembros
                .Include(m => m.Usuario)
                .Select(m => new MiembroDTO{
                    Id = m.Id,
                    Nombre = m.Nombre,
                    UserName = m.Usuario.UserName,
                    Email = m.Usuario.Email,
                    Rol = m.Usuario.Rol
                }).ToList();

            return Miembros;
        }

        public MiembroDTO MostrarMiembroPorId(int id) {
            var MiembroActual = _context.Miembros
                .Include(m => m.Usuario)
                .FirstOrDefault(m => m.Id == id);

            if(MiembroActual != null) {
                return new MiembroDTO{
                    Id = MiembroActual.Id,
                    Nombre = MiembroActual.Nombre,
                    UserName = MiembroActual.Usuario.UserName,
                    Email = MiembroActual.Usuario.Email,
                    Rol = MiembroActual.Usuario.Rol
                };
            } else {
                throw new KeyNotFoundException("No se encontro el usuario");
            }
        }

        public async Task CrearMiembro(MiembroDTO miembroDTO) {
            if(miembroDTO == null) {
                throw new ArgumentNullException(nameof(miembroDTO), "El DTO del miembro no puede ser nulo.");
            }

            try{
                var usuarioNuevo = new Usuario {
                    UserName =  miembroDTO.UserName 
                        ?? throw new ArgumentNullException(nameof(miembroDTO.UserName), "El username no puede ser nulo"),
                    Email =  miembroDTO.Email
                        ?? throw new ArgumentNullException(nameof(miembroDTO.Email),"El email no puede ser nulo"),
                    HashContraseña =  miembroDTO.HashContraseña
                        ?? throw new ArgumentNullException(nameof(miembroDTO.HashContraseña), "La contraseña no puede ser nula"),
                    Rol = "Miembro"
                };

                await _context.Usuarios.AddAsync(usuarioNuevo);
                await _context.SaveChangesAsync();

                var miembroNuevo = new Miembro {
                UsuarioId = usuarioNuevo.Id,
                Nombre = miembroDTO.Nombre
                    ?? throw new ArgumentNullException(nameof(miembroDTO.Nombre), "El nombre no puede ser nulo")
                };

                await _context.Miembros.AddAsync(miembroNuevo);
                await _context.SaveChangesAsync();
                
            } catch(Exception ex) {
                throw new Exception("Ocurrio un error al intentar creat el miembro: ", ex);
            }
        }
    }
    public interface IMiembroService {
        IEnumerable<MiembroDTO> MostrarMiembros();
        MiembroDTO MostrarMiembroPorId(int id);
        Task CrearMiembro(MiembroDTO miembroDTO);
    }
}