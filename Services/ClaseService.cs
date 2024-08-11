using ApiGym.Data;
using ApiGym.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ApiGym.Services {
    public class ClaseService: IClaseService {
        private readonly GymContext _context;
        private readonly ILoginService _loginService;
        private readonly IMiembroService _miembroService;
        private readonly IInstructorService _instructoService;

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
            _instructoService = instructorService;
        }

        public IEnumerable<Clase> MostrarClases() {
            var listaClases = _context.Clases
            .Include(c => c.MiembrosClase)
            .ToList();

            return listaClases;
        }

        public async Task<Clase> MostrarClasePorId(int id) {
            var claseActual = await _context.Clases
            .Include(c => c.MiembrosClase)
            .FirstOrDefaultAsync(c => c.Id == id);

            if(claseActual == null) {
                throw new KeyNotFoundException("La clase con este ID no existe o no fue encontrada");
            }

            return claseActual;
        }

        [Authorize(Roles = "Instructor")]
        public async Task CrearClase(CrearClaseDTO crearClaseDTO) {
            var UsuarioLogged = _loginService.ObtenerUsuarioAutenticado();
            var instructorLogged = _instructoService.MostrarInstructorPorUserId(UsuarioLogged.Id);

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

        [Authorize(Roles = "Miembro")]
        public async Task InscribirseClase(int idClase) {
            var UsuarioLogged = _loginService.ObtenerUsuarioAutenticado();
            var miembroLogged = _miembroService.MostrarMiembroPorUserId(UsuarioLogged.Id);

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
    }

    public interface IClaseService {
        IEnumerable<Clase> MostrarClases();
        Task<Clase> MostrarClasePorId(int id);
        Task CrearClase(CrearClaseDTO crearClaseDTO);
        Task InscribirseClase(int idClase);
    }
}