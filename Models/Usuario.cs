using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TechSolutions.Interfaces;
using System.Security.Policy;
using TechSolutions.SharedKernel;

namespace TechSolutions.Models
{
    public class Usuario : Persona, IEntity
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password {  get; set; }
        public Rol Rol { get; set; }
        //Activo = 1 esta activo, habilitado... Activo = 0  baja
        public bool Activo { get; set; } = true;


    }
}