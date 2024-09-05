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

        public PedidoController()
        {

            _detallepedidoRepository = new DetallePedidoData();
        }
        // Acción para cargar la vista de pago con una lista de productos
        public ActionResult Pagar()
        {
            // Crear una lista de productos hardcodeada
            var productos = new List<Producto>
            {
                new Producto
                {
                    Id = 1,
                    Nombre = "Producto A",
                    Descripcion = "Descripción del Producto A",
                    Precio = 100.0f,
                    IdCategoriaProducto = 1,
                    Stock = 10,
                    Foto = "url-de-foto-a",
                    Activo = true
                },
                new Producto
                {
                    Id = 2,
                    Nombre = "Producto B",
                    Descripcion = "Descripción del Producto B",
                    Precio = 150.0f,
                    IdCategoriaProducto = 2,
                    Stock = 5,
                    Foto = "url-de-foto-b",
                    Activo = true
                }
                // Agregar más productos si es necesario
            };

            var subtotal = productos.Sum(p => p.Precio);
            var total = subtotal;

            // Configurar el ViewBag
            ViewBag.Productos = productos;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = total;

            // Configurar el ViewBag para el tipo de tarjeta
            ViewBag.MediodePago = Enum.GetValues(typeof(MedioPago)).Cast<MedioPago>().Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            });

            // Configurar el ViewBag para los tipos de tarjetas
            ViewBag.TiposTarjeta = Enum.GetValues(typeof(TipoTarjeta)).Cast<TipoTarjeta>().Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            });

            // Configurar el ViewBag para las cuotas
            ViewBag.CuotasList = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "1 cuota sin interés" },
                new SelectListItem { Value = "3", Text = "3 cuotas sin interés" },
                new SelectListItem { Value = "6", Text = "6 cuotas sin interés" }
            };

            return View();
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
