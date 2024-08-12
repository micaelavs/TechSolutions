using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace TechSolutions.SharedKernel
{
    public enum EstadoPedido
    {   
        Pago_pendiente = 1,
        Pago_aprobado = 2,
        Preparado = 3,
        Despachado = 4,
        En_entrega = 5,
        Entregado = 6,
        En_proceso_devolucion = 7,
        Devuelto = 8,
        Parcialmente_devuelto = 9
    }
}

