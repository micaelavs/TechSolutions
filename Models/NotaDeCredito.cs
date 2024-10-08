using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TechSolutions.Interfaces;
using TechSolutions.SharedKernel;

namespace TechSolutions.Models
{
    public class NotaDeCredito : IEntity
    {
        public NotaDeCredito()
        {
            DetallesNotasCreditos = new List<DetalleNotaCredito>();
            
        }

        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        public SolicitudDevolucion SolicitudDevolucion { get; set; }

        [ForeignKey("SolicitudDevolucion")]
        [Column(Order = 2)]
        [Required]
        public int IdSolicitudDevolucion { get; set; }
        [Required]
        public int Numero { get; set; }
        public EncabezadoFactura EncabezadoFactura { get; set; }

        [ForeignKey("EncabezadoFactura")]
        [Column(Order = 3)]
        [Required]
        public int IdFactura { get; set; }
        [Required]
        public DateTime Fecha_emision { get; set; } = DateTime.Now;
        [Required]
        public float Monto { get; set; }
        [Required]
        public EstadoNotaCredito EstadoNota { get; set; }

        public IList<DetalleNotaCredito> DetallesNotasCreditos { get; private set; }


    }
}