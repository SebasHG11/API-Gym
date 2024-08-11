namespace ApiGym.Models
{
    public class MiembroDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string HashContraseña { get; set; }
        public string? Rol { get; set; }
    }
}