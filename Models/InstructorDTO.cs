namespace ApiGym.Models
{
    public class InstructorDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string HashContraseña { get; set; }
        public string? Rol { get; set; }
        public string Especialidad { get; set; }
    }
}