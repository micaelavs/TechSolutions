using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechSolutions.Models
{
    public class ClienteProductosReporteDTO
    {
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public string Mes { get; set; }
        public decimal Total { get; set; }
        public List<ProductoReporteDTO> Productos { get; set; } = new List<ProductoReporteDTO>(); 
    }
}