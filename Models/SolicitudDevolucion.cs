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
    public class SolicitudDevolucion : IEntity
    {
        public SolicitudDevolucion()
        {
            DetallesDevoluciones = new List<DetalleDevolucion>();
           
        }
        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        [Required]
        public int NumeroDevolucion { get; set; }
        public Pedido Pedido { get; set; }
        
        [ForeignKey("Pedido")]
        [Column(Order = 2)]
        [Required]
        public int IdPedido { get; set; }
        public EncabezadoFactura EncabezadoFactura { get; set; }
     
        [ForeignKey("EncabezadoFactura")]
        [Column(Order = 3)]
        [Required]
        public int IdFactura { get; set; }

        public Usuario Usuario { get; set; }

        [ForeignKey("Usuario")]
        [Column(Order = 4)]
        [Required]
        public int IdIUsuario {get; set; }
     
        [Required]
        public DateTime FechaOperacion { get; set; } = DateTime.Now;
        [Required]
        public EstadoSolicitud EstadoSolicitud { get; set; }
        [Required]
        public float Monto { get; set; }

        public IList<DetalleDevolucion> DetallesDevoluciones { get; private set; }


    }
}
