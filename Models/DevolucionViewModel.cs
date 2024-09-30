using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechSolutions.SharedKernel;

namespace TechSolutions.Models
{
    public class DevolucionViewModel
    {
        public List<Producto> ProductosADevolver { get; set; }
        public int NumeroPedido { get; set; }
        public int IdFactura { get; set; } // Agrega esto si es necesario
        public int IdUsuario { get; set; } // Agrega esto para almacenar el ID del usuario

        // Agrega propiedades para motivos y descripciones
        public Dictionary<int, int> Motivos { get; set; } // Clave: Id del producto, Valor: Id del motivo
        public Dictionary<int, string> Descripciones { get; set; } // Clave: Id del producto, Valor: Descripción
    }
}