using ApiGym.Data;
using ApiGym.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiGym.Services {
    public class ClaseService: IClaseService {
        private readonly GymContext _context;
        private readonly ILoginService _loginService;
        private readonly IMiembroService _miembroService;
        private readonly IInstructorService _instructorService;

        public ClaseService(
            GymContext context,
            ILoginService loginService,
            IMiembroService miembroService,
            IInstructorService instructorService
            )
        {
            _context = context;
            _loginService = loginService;
            _miembroService = miembroService;
            _instructorService = instructorService;
        }

        public IEnumerable<Clase> MostrarClases() {
            var listaClases = _context.Clases
            .Include(c => c.MiembrosClase)
            .ToList();

            return listaClases;
        }

        public async Task<Clase> MostrarClasePorId(int id) {
            var claseActual = await _context.Clases
            .Include(c => c.Instructor)
            .Include(c => c.MiembrosClase)
            .FirstOrDefaultAsync(c => c.Id == id);

            if(claseActual == null) {
                throw new KeyNotFoundException("La clase con este ID no existe o no fue encontrada");
            }

            return claseActual;
        }

        public async Task CrearClase(CrearClaseDTO crearClaseDTO) {
            var UsuarioLogged = _loginService.ObtenerUsuarioAutenticado();
            var instructorLogged = await _instructorService.MostrarInstructorPorUserId(UsuarioLogged.Id);

            if(instructorLogged == null) {
                throw new KeyNotFoundException("El instructor con este ID no fue encontrado");
            }

            var nuevaClase = new Clase {
                Nombre = crearClaseDTO.Nombre,
                FechaHora = crearClaseDTO.FechaHora,
                instructorId = instructorLogged.Id,
            };

            await _context.Clases.AddAsync(nuevaClase);
            await _context.SaveChangesAsync();
        }

        public async Task InscribirseClase(int idClase) {
            var UsuarioLogged = _loginService.ObtenerUsuarioAutenticado();
            var miembroLogged = await _miembroService.MostrarMiembroPorUserId(UsuarioLogged.Id);

            var claseActual = await _context.Clases
            .Include(c => c.MiembrosClase)
            .FirstOrDefaultAsync(c => c.Id == idClase);

            if(claseActual == null) {
                throw new KeyNotFoundException("La clase buscada con este ID no existe o no fue encontrada.");
            }

            if(claseActual.MiembrosClase.Any(mc => mc.MiembroId == miembroLogged.Id)) {
                throw new InvalidOperationException("El usuario ya esta inscrito en esta clase.");
            }

            var nuevaInscripcion = new MiembroClase {
                MiembroId = miembroLogged.Id,
                ClaseId = claseActual.Id
            };

            claseActual.MiembrosClase.Add(nuevaInscripcion);

            await _context.SaveChangesAsync();
        }

        public async Task DesuscribirseClase(int idClase) {
            var UsuarioLogged = _loginService.ObtenerUsuarioAutenticado();
            var miembroLogged = await _miembroService.MostrarMiembroPorUserId(UsuarioLogged.Id);

            var clase = await _context.Clases
            .Include(c => c.MiembrosClase)
            .FirstOrDefaultAsync(c => c.Id == idClase);

            if(clase == null) {
                throw new KeyNotFoundException("La clase buscada con este ID no existe.");
            }

            var miembroClase = clase.MiembrosClase.FirstOrDefault(mc => mc.MiembroId == miembroLogged.Id);

            if(miembroClase == null) {
                throw new KeyNotFoundException("El miembro no esta inscrito en esta clase.");
            }
   
            clase.MiembrosClase.Remove(miembroClase);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarClase(int idClase) {
            var UsuarioLogged = _loginService.ObtenerUsuarioAutenticado();
            var instructorLogged = await _instructorService.MostrarInstructorPorUserId(UsuarioLogged.Id);

            var claseActual = await _context.Clases
            .Include(c => c.MiembrosClase)
            .FirstOrDefaultAsync(c => c.Id == idClase);

            if (claseActual == null) {
                throw new KeyNotFoundException("La clase especificada no existe.");
            }   

            if(claseActual.instructorId != instructorLogged.Id) {
                throw new UnauthorizedAccessException("Solo el instructor que creo la clase puede eliminarla.");
            }

            using var transaccion = await _context.Database.BeginTransactionAsync();

            try{
                if(claseActual.MiembrosClase.Any()) {
                    _context.MiembroClases.RemoveRange(claseActual.MiembrosClase);
                    await _context.SaveChangesAsync();
                }

                _context.Clases.Remove(claseActual);
                await _context.SaveChangesAsync();

                await transaccion.CommitAsync();

            } catch(Exception ex) {
                await transaccion.RollbackAsync();
                throw new Exception("Error al intentar eliminar la clase.", ex);
            }
        }

        public async Task EditarClase(int idClase, EditarClaseDTO editarClaseDTO) {
            var UsuarioLogged = _loginService.ObtenerUsuarioAutenticado();
            var instructorLogged = _instructorService.MostrarInstructorPorUserId(UsuarioLogged.Id);

            var claseActual = await _context.Clases
            .FirstOrDefaultAsync(c => c.Id == idClase);

            if(claseActual == null) {
                throw new KeyNotFoundException("La clase especificada no existe.");
            }

            if(instructorLogged.Id != claseActual.instructorId) {
                throw new UnauthorizedAccessException("Solo el instructor que creo la clase puede editarla.");
            }

            try{
                claseActual.Nombre = editarClaseDTO.Nombre ?? claseActual.Nombre;
                claseActual.FechaHora = editarClaseDTO.FechaHora ?? claseActual.FechaHora;

                await _context.SaveChangesAsync();
            } catch (Exception ex) {
                throw new Exception("Error al intentar editar la clase.", ex);
            }
        }
    }

    public interface IClaseService {
        IEnumerable<Clase> MostrarClases();
        Task<Clase> MostrarClasePorId(int id);
        Task CrearClase(CrearClaseDTO crearClaseDTO);
        Task InscribirseClase(int idClase);
        Task DesuscribirseClase(int idClase);
        Task EliminarClase(int idClase);
        Task EditarClase(int idClase, EditarClaseDTO editarClaseDTO);
    }
}