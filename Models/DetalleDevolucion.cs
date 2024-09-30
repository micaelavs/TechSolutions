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
    public class DetalleDevolucion: IEntity
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        public SolicitudDevolucion SolicitudDevolucion { get; set; }

        [ForeignKey("SolicitudDevolucion")]
        [Column(Order = 2)]
        [Required]
        public int IdSolicitudDevolucion { get; set; }
        public Producto Producto { get; set; }

        [ForeignKey("Producto")]
        [Column(Order = 3)]
        [Required]
        public int IdProducto { get; set; }
        [Required]
        public int Cantidad   { get; set; }
        [Required]
        public float PrecioUnitario { get; set; }
        [Required]
        public Motivo Motivo { get; set; }

        public string Descripcion { get; set; } = null;







    }
}
