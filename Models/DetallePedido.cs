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
    public class DetallePedido: IEntity
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        public Producto Producto { get; set; }
        public Pedido Pedido { get; set; }

        [ForeignKey("Producto")]
        [Column(Order = 2)]
        [Required]
        public int IdProducto { get; set; }

        [ForeignKey("Pedido")]
        [Column(Order = 3)]
        [Required]
        public int IdPedido { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public float PrecioUnitario {  get; set; }
      
    }
}
