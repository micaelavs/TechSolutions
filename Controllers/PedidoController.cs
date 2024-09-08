using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechSolutions.Data;
using TechSolutions.Models;
using TechSolutions.SharedKernel;

namespace TechSolutions.Controllers
{
    public class PedidoController : Controller
    {
        private readonly DetallePedidoData _detallepedidoRepository;
        private readonly ProductoData _productoData;

        public PedidoController()
        {

            _detallepedidoRepository = new DetallePedidoData();
            _productoData = new ProductoData();
        }
        // Acción para cargar la vista de pago con una lista de productos
        [HttpGet]
        public ActionResult Pagar(int Id, int Cantidad) //ver como pasar la lista de productos cuando compras varios del carrito o hacer metodo aparte
        {
            PagoViewModel pago = new PagoViewModel();
            Producto producto = new Producto();
            producto = _productoData.GetById(Id); 
            
            pago.Producto = producto;
            pago.Subtotal = (float)(producto.Precio * Cantidad);
            pago.Total = pago.Subtotal;
            pago.Cantidad = Cantidad;

            // Configurar el ViewBag
            /*ViewBag.Producto = producto;
            ViewBag.Subtotal = subtotal;
            ViewBag.Cantidad = Cantidad;    
            ViewBag.Total = total;*/

            //Otros datos
            ViewBag.MediodePago = Enum.GetValues(typeof(MedioPago)).Cast<MedioPago>().Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            });

            ViewBag.TiposTarjeta = Enum.GetValues(typeof(TipoTarjeta)).Cast<TipoTarjeta>().Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            });


            ViewBag.CuotasList = Enum.GetValues(typeof(Cuotas)).Cast<Cuotas>().Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            });
 

            return View(pago);
        }
        /*
         public class Pedido: IEntity
     {
         [Key]
         [Column(Order = 1)]
         [Required]
         public int Id { get; set; }
         [Required]
         public int Numero { get; set; }

         [ForeignKey("Usuario")]
         [Column(Order = 2)]
         [Required]
         public int IdUsuario { get; set; }
         public Usuario Usuario { get; set; }
         public EstadoPedido Estado { get; set; }

         public float MontoTotal { get; set; }
         public DateTime FechaOperacion { get; set; } = DateTime.Now;

         public IList<DetallePedido> DetallesPedidos { get; private set; }

         public IList<HistorialPedido> HistorialPedidos { get; private set; }
     }

        public class DetallePedido: IEntity
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        public Producto Producto { get; set; }
        public Pedido Pedido { get; set; }

        [ForeignKey("Producto")]
        [Column(Order = 2)]
        [Required]
        public int IdProducto { get; set; }

        [ForeignKey("Pedido")]
        [Column(Order = 3)]
        [Required]
        public int IdPedido { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public float PrecioUnitario {  get; set; }
      
    }
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarPago(PagoViewModel pago)
        {
            /*if (ModelState.IsValid)
            {
              
                Pedido pedido = new Pedido();
                pedido.IdUsuario = (int)Session["UserId"];
                pedido.Numero = GenerarNumeroPedido();
                pedido.FechaOperacion = DateTime.Now;
                pedido.MontoTotal = total;
                pedido.Estado = EstadoPedido.Pago_aprobado;
                //pedido.Id = ped
                DetallePedido NuevoDetallePedido = new DetallePedido();

                foreach (var producto in productos)
                {
                    NuevoDetallePedido.IdProducto = producto.Id;
                    NuevoDetallePedido.IdPedido = 99;
                    NuevoDetallePedido.Cantidad = 
                }
                

                    DetallesPedido = productos.Select(p => new DetallePedido
                    {
                        ProductoId = p.Id,
                        Precio = p.Precio,
                        Cantidad = 1 // Aquí puedes manejar cantidades si las tienes en el modelo
                    }).ToList()
                

                // Guardar el pedido en la base de datos
                _pedidoData.Add(nuevoPedido);

                // Lógica para manejar el procesamiento del pago
                bool pagoExitoso = ProcesarPago(model);
                if (pagoExitoso)
                {
                    // Cambiar estado del pedido a pagado
                    nuevoPedido.Estado = EstadoPedido.Pagado;
                    _pedidoData.Update(nuevoPedido);

                    // Redirigir a una vista de confirmación de pago
                    return RedirectToAction("ConfirmacionPago", new { id = nuevoPedido.Id });
                }
                else
                {
                    // Si falla el pago, mostrar error
                    ModelState.AddModelError("", "Hubo un error procesando el pago. Por favor, inténtelo nuevamente.");
                }
            }*/

            // Si el modelo no es válido o falló el pago, volver a mostrar la vista de pago
            //return View(model);
            return View();
          
        }
        private int GenerarNumeroPedido()
        {
            Random random = new Random();
            return random.Next(10000, 99999); // Genera un número entre 10000 y 99999
        }


        //mustra el carrito
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Devolver()//id
        {
            // Recuperar el detalle del pedido desde la base de datos
            //var detallePedido = _detallepedidoRepository.GetById(id);

            // Preparar la lista de motivos
            //ViewBag.MotivoList = new SelectList(Enum.GetValues(typeof(Motivo)), "Value", "DisplayName");
            return View(/*detallePedido*/);
        }

        [HttpPost]
        public ActionResult DevolverProducto(DetallePedido detallePedido, string descripcion, Motivo motivo)
        {
            if (ModelState.IsValid)
            {
                // Lógica para procesar la devolución del producto
                var detalle = _detallepedidoRepository.GetById(detallePedido.Id);
                if (detalle != null)
                {
                    // Actualiza el detalle del pedido según el motivo de la devolución
                    // Añade lógica específica para manejar devoluciones aquí

                    //ver a donde redirige despues
                    return RedirectToAction("Index"); // Redirige a la página que consideres apropiada
                }
            }
            return View(detallePedido); // Devuelve a la vista en caso de error
        }
    }   
}
