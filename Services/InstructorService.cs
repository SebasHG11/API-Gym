using ApiGym.Data;
using ApiGym.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;

namespace ApiGym.Services {
    public class InstructorService: IInstructorService {
        private readonly GymContext _context;
        public InstructorService(GymContext context) {
            _context = context;
        }

        public IEnumerable<InstructorDTO> MostrarInstructores() {
            var Instructores = _context.Instructors
            .Include(i => i.Usuario)
            .Select(i => new InstructorDTO {
                Id = i.Id,
                Nombre = i.Nombre,
                UserName = i.Usuario.UserName,
                Email = i.Usuario.Email,
                HashContraseña = i.Usuario.HashContraseña,
                Rol = i.Usuario.Rol,
                Especialidad = i.Especialidad
            }).ToList();

            return Instructores;
        }

        public InstructorDTO MostrarInstructorPorId(int id) {
            var instructorActual = _context.Instructors
            .Include(i => i.Usuario)
            .FirstOrDefault(i => i.Id == id);

            if(instructorActual != null) {
                return new InstructorDTO {
                    Id = instructorActual.Id,
                    Nombre = instructorActual.Nombre,
                    UserName = instructorActual.Usuario.UserName,
                    Email = instructorActual.Usuario.Email,
                    HashContraseña = instructorActual.Usuario.HashContraseña,
                    Rol = instructorActual.Usuario.Rol,
                    Especialidad = instructorActual.Especialidad
                };
            } else {
                throw new KeyNotFoundException("No se encontro el usuario");
            }
        }

        public InstructorDTO MostrarInstructorPorUserId(int userId) {
            var instructorActual = _context.Instructors
            .Include(i => i.Usuario)
            .FirstOrDefault(i => i.UsuarioId == userId);

            if(instructorActual != null) {
                return new InstructorDTO {
                    Id = instructorActual.Id,
                    Nombre = instructorActual.Nombre,
                    UserName = instructorActual.Usuario.UserName,
                    Email = instructorActual.Usuario.Email,
                    HashContraseña = instructorActual.Usuario.HashContraseña,
                    Rol = instructorActual.Usuario.Rol,
                    Especialidad = instructorActual.Especialidad
                };
            } else {
                throw new KeyNotFoundException("No se encontro el usuario");
            }
        }

        public async Task CrearInstructor(InstructorDTO instructorDTO) {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try{
                var usuarioNuevo = new Usuario {
                    Email = instructorDTO.Email,
                    UserName = instructorDTO.UserName,
                    HashContraseña = instructorDTO.HashContraseña,
                    Rol = "Instructor",
                };

                await _context.Usuarios.AddAsync(usuarioNuevo);
                await _context.SaveChangesAsync();

                var instructorNuevo = new Instructor {
                    UsuarioId = usuarioNuevo.Id,
                    Nombre = instructorDTO.Nombre,
                    Especialidad = instructorDTO.Especialidad
                };

                await _context.Instructors.AddAsync(instructorNuevo);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                
            } catch(Exception ex) {
                await transaction.RollbackAsync();
                throw new Exception("Ocurrio un error al crear un instructor", ex);
            }
        }

        public async Task EditarInstructor(int id, InstructorDTO instructorDTO) {
            var instructorActual = await _context.Instructors
            .Include(i => i.Usuario)
            .FirstOrDefaultAsync(i => i.Id == id);

            if(instructorActual == null) {
                throw new KeyNotFoundException  ("El usuario con el ID ingresada no existe.");
            }

            try{
                instructorActual.Nombre = instructorDTO.Nombre ?? instructorActual.Nombre;
                instructorActual.Especialidad = instructorDTO.Especialidad ?? instructorActual.Especialidad;

                instructorActual.Usuario.UserName = instructorDTO.UserName ?? instructorActual.Usuario.UserName;
                instructorActual.Usuario.Email = instructorDTO.Email ?? instructorActual.Usuario.Email;
                instructorActual.Usuario.HashContraseña = instructorDTO.HashContraseña ?? instructorActual.Usuario.HashContraseña;

                await _context.SaveChangesAsync();
            } catch (DbUpdateException dbEx) {
                throw new Exception("Error al intentar actualizar la base de datos.", dbEx);
            } catch (Exception ex) {
                throw new Exception("Se presentó un error al editar el instructor.", ex);
            }
        }

        public async Task EliminarInstructor(int id) {
            var instructorActual = await _context.Instructors
            .Include(i => i.Usuario)
            .Include(i => i.Clases)
            .FirstOrDefaultAsync(i => i.Id == id);

            if(instructorActual == null) {
                throw new KeyNotFoundException  ("El usuario con el ID ingresada no existe.");
            }

            var clasesRelacionadas = instructorActual.Clases.ToList();

            var usuarioActual = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == instructorActual.UsuarioId);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try{
                foreach(var clase in clasesRelacionadas) {
                    var miembrosRelacionados = await _context.MiembroClases
                    .Where(mc => mc.ClaseId == clase.Id)
                    .ToListAsync();

                    if(miembrosRelacionados.Any()) {
                        _context.MiembroClases.RemoveRange(miembrosRelacionados);
                        await _context.SaveChangesAsync();
                    }
                }

                if(clasesRelacionadas.Any()) {
                    _context.Clases.RemoveRange(clasesRelacionadas);
                    await _context.SaveChangesAsync();
                }

                _context.Instructors.Remove(instructorActual);
                await _context.SaveChangesAsync();

                _context.Usuarios.Remove(usuarioActual);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            } catch(Exception ex) {
                await transaction.RollbackAsync();
                throw new Exception("Error al intentar eliminar el instructor", ex);
            }
        }
    }
    public interface IInstructorService {
        IEnumerable<InstructorDTO> MostrarInstructores();
        InstructorDTO MostrarInstructorPorId(int id);
        InstructorDTO MostrarInstructorPorUserId(int userId);
        Task CrearInstructor(InstructorDTO instructorDTO);
        Task EditarInstructor(int id, InstructorDTO instructorDTO);
        Task EliminarInstructor(int id);
    }
}