using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechSolutions.Models
{
    public class MisComprasViewModel
    {
        public IEnumerable<EncabezadoFactura> Facturas { get; set; }
        public IEnumerable<CalificacionProducto> CalificacionesUsuario { get; set; }
    }
}