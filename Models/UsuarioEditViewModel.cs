using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TechSolutions.SharedKernel;

namespace TechSolutions.Models
{
    //es una clase auxiliar, para poder guardar los datos del usaurio
    //necesito que la pass no sea obligatoria par apoder hacr la edicion
    public class UsuarioEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; } // No poner [Required] para la edición

        [Required]
        public Rol Rol { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }
    }
}