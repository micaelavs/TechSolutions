using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TechSolutions.Interfaces;

namespace TechSolutions.Models
{
    public class DetalleNotaCredito: IEntity
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
     
        
        [ForeignKey("NotaDeCredito")]
        [Column(Order = 2)]
        [Required]
        public int IdNotaCredito { get; set; }

        public NotaDeCredito NotaDeCredito { get; set; }
        public Producto Producto { get; set; }

        [ForeignKey("Producto")]
        [Column(Order = 3)]
        [Required]
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }

        public float PrecioUnitario { get; set; }

    }
}