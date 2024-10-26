using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechSolutions.Models
{
    public class ClienteReporteDTO
    {
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public decimal Total { get; set; }
    }
}