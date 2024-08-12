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
    public class HistorialPedido : IEntity
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        [Required]
        public Pedido Pedido { get; set; }
        [ForeignKey("Pedido")]
        [Column(Order = 2)]
        [Required]
        public int IdPedido { get; set; }
        [Required]
        public EstadoPedido EstadoPedido { get; set;}

        public DateTime FechaOperacion { get; set; } = DateTime.Now;
    }
}
