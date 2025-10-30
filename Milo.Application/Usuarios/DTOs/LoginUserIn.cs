using System.ComponentModel.DataAnnotations;

namespace Milo.Application.Usuarios.DTOs
{
    public class LoginUserIn
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "La contraseña no puede estar vacía")]
        public string Password { get; set; }
    }
}
