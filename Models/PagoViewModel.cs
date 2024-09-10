using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechSolutions.SharedKernel;

namespace TechSolutions.Models
{
    public class PagoViewModel
    {
        public PagoViewModel()
        {
            Productos = new List<Producto>();
            Subtotales = new Dictionary<int, float>();
            Cantidades = new Dictionary<int, int>();

        }
        public Producto Producto { get; set; }
        public int ProductoId { get; set; }
        public List<Producto> Productos { get; private set; }
        public Dictionary<int,float> Subtotales { get; private set; }
        public Dictionary<int, int> Cantidades { get; private set; }
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