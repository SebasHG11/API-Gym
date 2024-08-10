using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiGym.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string HashContraseña { get; set; }
        [Required]
        public string Rol { get; set; }
        public Miembro? Miembro { get; set; }
        public Instructor? Instructor { get; set; }
    }
}