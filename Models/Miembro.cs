namespace ApiGym.Models
{
    public class Miembro
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public Usuario Usuario { get; set; }
    }
}