using ApiGym.Data;
using ApiGym.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

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
    }
    public interface IInstructorService {
        IEnumerable<InstructorDTO> MostrarInstructores();
        InstructorDTO MostrarInstructorPorId(int id);
        Task CrearInstructor(InstructorDTO instructorDTO);
    }
}