using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechSolutions.SharedKernel;

namespace TechSolutions.Models
{
    public class PagoViewModel
    {
        public Producto Producto { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public float Subtotal { get; set; }
        public float Total { get; set; }
        public MedioPago MediodePago { get; set; }
        public TipoTarjeta TipoTarjeta { get; set; }
        public Cuotas Cuotas { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public string DNI { get; set; }
        public string NumeroTarjeta { get; set; }
        public string FechaExpiracion { get; set; }
        public string CodigoSeguridad { get; set; }
    }
}