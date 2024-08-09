namespace ApiGym.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Contraseña { get; set; }
        public string Rol { get; set; }
        public Miembro Miembro { get; set; }
        public Instructor Instructor { get; set; }
    }
}