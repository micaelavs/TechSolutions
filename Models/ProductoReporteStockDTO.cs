using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechSolutions.Models
{
    public class ProductoReporteStockDTO
    {
        public string ProductoNombre { get; set; }
        public int Stock { get; set; }
        public string Categoria { get; set; }
    }
}