using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechSolutions.Interfaces;
using TechSolutions.SharedKernel;

namespace TechSolutions.Models
{
    public class CalificacionProducto:IEntity
    {

        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        public Producto Producto { get; set; }

        [ForeignKey("Producto")]
        [Column(Order = 2)]
        [Required]
        public int IdProducto { get; set; }
        [Required]
        public Puntaje Puntaje { get; set; }
        public string Comentario { get; set; } = null;
    }
}
