using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechSolutions.Interfaces;

namespace TechSolutions.Models
{
    public class DetalleFactura:IEntity
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        public EncabezadoFactura EncabezadoFactura { get; set; }

        [ForeignKey("EncabezadoFactura")]
        [Column(Order = 2)]
        [Required]
        public int IdFactura { get; set; }
        public Producto Producto { get; set; }

        [ForeignKey("Producto")]
        [Column(Order = 3)]
        [Required]
        public int IdProducto { get; set; }
        public int Cantidad   { get; set; }

        public float PrecioUnitario { get; set; }

    }
}
