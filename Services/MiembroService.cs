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

        public async Task<MiembroDTO> MostrarMiembroPorId(int id) {
            var MiembroActual = await _context.Miembros
                .Include(m => m.Usuario)
                .Include(m => m.MiembroClases)
                .FirstOrDefaultAsync(m => m.Id == id);

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

        public async Task<MiembroDTO> MostrarMiembroPorUserId(int userId) {
            var MiembroActual = await _context.Miembros
                .Include(m => m.Usuario)
                .Include(m => m.MiembroClases)
                .FirstOrDefaultAsync(m => m.UsuarioId == userId);

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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try{
                var usuarioNuevo = new Usuario {
                    UserName =  miembroDTO.UserName 
                        ?? throw new ArgumentNullException(nameof(miembroDTO.UserName), "El username no puede ser nulo"),
                    Email =  miembroDTO.Email
                        ?? throw new ArgumentNullException(nameof(miembroDTO.Email),"El email no puede ser nulo"),
                    HashContraseña =  miembroDTO.HashContraseña
                        ?? throw new ArgumentNullException(nameof(miembroDTO.HashContraseña), "La contraseña no puede ser nula"),
                    Rol = "Miembro" ?? "Miembro"
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
                
                await transaction.CommitAsync();

            } catch(Exception ex) {
                await transaction.RollbackAsync();
                throw new Exception("Ocurrio un error al intentar crear el miembro: ", ex);
            }
        }

        public async Task EditarMiembro(int id, MiembroDTO miembroDTO) {
            var miembroActual = await _context.Miembros
            .Include(m => m.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id);

            if(miembroActual == null) {
                throw new KeyNotFoundException("El miembro con el ID proporcionado no existe.");
            }

            try{
                miembroActual.Nombre = miembroDTO.Nombre;
                miembroActual.Usuario.UserName = miembroDTO.UserName;
                miembroActual.Usuario.Email = miembroDTO.Email;
                miembroActual.Usuario.HashContraseña = miembroDTO.HashContraseña;

                await _context.SaveChangesAsync();
            } catch(Exception ex) {
                throw new Exception("Error al intentar editar el miembro", ex);
            }
        }

        public async Task EliminarMiembro(int id) {
            var miembroActual = await _context.Miembros
            .Include(m => m.Usuario)
            .Include(m => m.MiembroClases)
            .FirstOrDefaultAsync(m => m.Id == id);

            if(miembroActual == null) {
                throw new KeyNotFoundException("El miembro con el ID proporcionado no existe.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try{
                var clasesRelacionadas = miembroActual.MiembroClases.ToList();

                if(clasesRelacionadas.Any()) {
                    _context.MiembroClases.RemoveRange(clasesRelacionadas);
                    await _context.SaveChangesAsync();
                }

                _context.Miembros.Remove(miembroActual);
                await _context.SaveChangesAsync();

                var usuarioActual = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == miembroActual.UsuarioId);
                _context.Usuarios.Remove(usuarioActual);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            } catch(Exception ex) {
                await transaction.RollbackAsync();
                throw new Exception("Error al intentar eliminar este miembro", ex);
            }
        }
    }
    public interface IMiembroService {
        IEnumerable<MiembroDTO> MostrarMiembros();
        Task<MiembroDTO> MostrarMiembroPorId(int id);
        Task<MiembroDTO> MostrarMiembroPorUserId(int userId);
        Task CrearMiembro(MiembroDTO miembroDTO);
        Task EditarMiembro(int id, MiembroDTO miembroDTO);
        Task EliminarMiembro(int id);
    }
}