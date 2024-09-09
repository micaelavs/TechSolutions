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
    public class EncabezadoFactura: IEntity
    {
        public EncabezadoFactura()
        {
            DetallesFacturas = new List<DetalleFactura>();
           
        }
        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        [Required]
        public int Numero { get; set; }
        [Required]
        public TipoFactura TipoFactura { get; set; }
       
        [ForeignKey("Pedido")]
        [Column(Order = 2)]
        [Required]
        public int IdPedido { get; set; }
        public Pedido Pedido { get; set; }

        [ForeignKey("Usuario")]
        [Column(Order = 3)]
        [Required]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
        public MedioPago MedioPago { get; set; }
        public TipoTarjeta TipoTarjeta { get; set; }

        [Required]
        public string NombreTarjeta { get; set; }
        [Required]
        public string ApellidoTarjeta { get; set; }
        [Required]
        public string DNI { get; set; }
        [Required]
        public string Nrotarjeta { get; set; }

        [Required]
        public Cuotas Cuota { get; set; }
        [Required]
        public float Monto { get; set; }
        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        public IList<DetalleFactura> DetallesFacturas { get; private set; }



    }
}
