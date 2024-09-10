using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechSolutions.Data;
using TechSolutions.Models;
using TechSolutions.SharedKernel;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace TechSolutions.Controllers
{
    public class PedidoController : Controller
    {
      
        private readonly DetallePedidoData _detallepedidoRepository;
        private readonly PedidoData _pedidoRepository;
        private readonly ProductoData _productoRepository;
        private readonly EncabezadoFacturaData _encabezadoFacturaRepository;
        private readonly DetalleFacturaData _detallefacturaRepository;
        public PedidoController()
        {

            _detallepedidoRepository = new DetallePedidoData();
            _productoRepository = new ProductoData();
            _pedidoRepository = new PedidoData();
            _encabezadoFacturaRepository = new EncabezadoFacturaData();
            _detallefacturaRepository = new DetalleFacturaData();
        }
        // Acción para cargar la vista de pago con una lista de productos
        //vista previa de pagar 1 tipo de producto
        [HttpGet]
        public ActionResult Pagar(int Id, int Cantidad) 
        {
            PagoViewModel pago = new PagoViewModel();
            Producto producto = new Producto();
            producto = _productoRepository.GetById(Id); 
            
            pago.Producto = producto;
            pago.Subtotal = (float)(producto.Precio * Cantidad);
            pago.Total = pago.Subtotal;
            pago.Cantidad = Cantidad;
            pago.ProductoId = producto.Id;

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
        //cinfirmar pago de un tipo de producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarPago(PagoViewModel pago)
        {
            var db = new ApiDbContext();

            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var producto = _productoRepository.GetById(pago.ProductoId);
                        if (producto == null)
                        {
                            ModelState.AddModelError("", "Producto no encontrado.");
                            return View(pago);
                        }

                        // Crear pedido
                        Pedido pedido = new Pedido
                        {
                            IdUsuario = (int)Session["UserId"],
                            Numero = GenerarNumeroPedido(),
                            FechaOperacion = DateTime.Now,
                            MontoTotal = pago.Total,
                            Estado = EstadoPedido.Pago_aprobado
                        };

                        int idPedido = _pedidoRepository.InsertReturnId(pedido);
                        if (idPedido <= 0)
                        {
                            throw new Exception("Error al crear el pedido.");
                        }

                        // Crear detalle del pedido
                        DetallePedido detallePedido = new DetallePedido
                        {
                            IdProducto = producto.Id,
                            IdPedido = idPedido,
                            Cantidad = pago.Cantidad,
                            PrecioUnitario = producto.Precio
                        };

                        int detallePedidoId = _detallepedidoRepository.InsertReturnId(detallePedido);

                        if (detallePedidoId <= 0)
                        {
                            throw new Exception("Error al crear el detalle de pedido.");
                        }

                        // Crear factura
                        EncabezadoFactura factura = new EncabezadoFactura
                        {
                            Numero = GenerarNumeroPedido(),
                            TipoFactura = TipoFactura.A,
                            IdPedido = idPedido,
                            IdUsuario = (int)Session["UserId"],
                            MedioPago = pago.MediodePago,
                            TipoTarjeta = pago.TipoTarjeta,
                            NombreTarjeta = pago.Nombre,
                            ApellidoTarjeta = pago.Apellido,
                            DNI = pago.DNI,
                            Nrotarjeta = pago.NumeroTarjeta,
                            Cuota = pago.Cuotas,
                            Monto = pago.Total,
                            Fecha = DateTime.Now
                        };

                        int facturaid = _encabezadoFacturaRepository.InsertReturnId(factura);
                        if (facturaid <= 0)
                        {
                            throw new Exception("Error al crear la factura.");
                        }

                        // Crear detalle de factura
                        DetalleFactura detallefactura = new DetalleFactura
                        {
                            IdFactura = facturaid,
                            IdProducto = producto.Id,
                            Cantidad = pago.Cantidad,
                            PrecioUnitario = producto.Precio
                        };
                        int detallefacturaid = _detallefacturaRepository.InsertReturnId(detallefactura);

                        if (detallefacturaid <= 0)
                        {
                            throw new Exception("Error al crear el detallefactura.");
                        }

                        // Restar la cantidad al stock del producto
                        var stock_actual = producto.Stock - pago.Cantidad;
                        producto.Stock = stock_actual;

                        _productoRepository.Update(producto);

                        transaction.Commit();

                        // Pasar datos a la vista usando TempData
                        TempData["Producto"] = producto.Nombre;
                        TempData["Cantidad"] = pago.Cantidad;
                        TempData["Subtotal"] = producto.Precio * pago.Cantidad;
                        TempData["Total"] = pago.Total;
                        TempData["PrecioUnitario"] = producto.Precio;
                        TempData["NumeroPedido"] = pedido.Numero;
                        TempData["NumeroFactura"] = factura.Numero;
                        TempData["FechaFactura"] = factura.Fecha.ToString("dd/MM/yyyy");
                        TempData["MedioPago"] = $"{pago.TipoTarjeta} - {pago.Cuotas} cuotas";
                        TempData["MontoFactura"] = factura.Monto;
                        TempData["UserId"] = Session["UserId"];
                        return RedirectToAction("ConfirmacionPago");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", $"Hubo un error procesando el pago: {ex.Message}");
                        return View(pago);
                    }
                }
            }

            // Si el modelo no es válido
            return View(pago);
        }

        //get mustra la vista confirmacion de pago de un tipo de prodcto
        public ActionResult ConfirmacionPago()
        {
            return View();
        }

        //vista previa de carrito
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult PagarCarrito(string carrito)
        {
            var listaDeObjetos = JsonConvert.DeserializeObject<List<dynamic>>(carrito);

            PagoViewModel pago = new PagoViewModel();
            float subtotalGeneral = 0;

            foreach (var obj in listaDeObjetos)
            {
                int cantidad = int.Parse((string)obj.cantidad);
                var producto = new Producto
                {
                    Id = int.Parse((string)obj.id),
                    Nombre = (string)obj.nombre,
                    Precio = float.Parse((string)obj.precio),
                    Stock = int.Parse((string)obj.stock)
                };

                // Calcular subtotal para este producto
                float subtotalProducto = producto.Precio * cantidad;
                subtotalGeneral += subtotalProducto;

                // Agregar a la lista de productos
                pago.Productos.Add(producto);

                // Guardar la cantidad y el subtotal del producto
                pago.Cantidades[producto.Id] = cantidad;
                pago.Subtotales[producto.Id] = subtotalProducto;
            }

            // Calcular total general
            pago.Total = subtotalGeneral;

            // Configurar ViewBag para otros datos
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

            private int GenerarNumeroPedido()
        {
            Random random = new Random();
            return random.Next(10000, 99999); // Genera un número entre 10000 y 99999
        }

        //confirmacion pagar carrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmarPagoCarrito(PagoViewModel pago)
        {
            // Deserializar los datos JSON
            var cantidades = JsonConvert.DeserializeObject<Dictionary<int, int>>(Request.Form["Cantidades"]);
            var productos = JsonConvert.DeserializeObject<List<Producto>>(Request.Form["Productos"]);
            var subtotales = JsonConvert.DeserializeObject<Dictionary<int, float>>(Request.Form["Subtotales"]);

            var db = new ApiDbContext();

            //if (ModelState.IsValid)
            //{
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Crear pedido
                        Pedido pedido = new Pedido
                        {
                            IdUsuario = (int)Session["UserId"],
                            Numero = GenerarNumeroPedido(),
                            FechaOperacion = DateTime.Now,
                            MontoTotal = pago.Total,
                            Estado = EstadoPedido.Pago_aprobado
                        };

                        int idPedido = _pedidoRepository.InsertReturnId(pedido);
                        if (idPedido <= 0)
                        {
                            throw new Exception("Error al crear el pedido.");
                        }

                        // Crear encabezado de factura una sola vez
                        EncabezadoFactura factura = new EncabezadoFactura
                        {
                            Numero = GenerarNumeroPedido(),
                            TipoFactura = TipoFactura.A,
                            IdPedido = idPedido,
                            IdUsuario = (int)Session["UserId"],
                            MedioPago = pago.MediodePago,
                            TipoTarjeta = pago.TipoTarjeta,
                            NombreTarjeta = pago.Nombre,
                            ApellidoTarjeta = pago.Apellido,
                            DNI = pago.DNI,
                            Nrotarjeta = pago.NumeroTarjeta,
                            Cuota = pago.Cuotas,
                            Monto = pago.Total,
                            Fecha = DateTime.Now
                        };

                        int facturaId = _encabezadoFacturaRepository.InsertReturnId(factura);
                        if (facturaId <= 0)
                        {
                            throw new Exception("Error al crear la factura.");
                        }

                        // Crear detalles del pedido y de la factura
                        foreach (var producto in productos)
                        {
                            if (producto == null || !cantidades.ContainsKey(producto.Id))
                            {
                                continue;
                            }

                            var cantidad = cantidades[producto.Id];
                            var subtotal = subtotales[producto.Id];

                            // Crear detalle del pedido
                            DetallePedido detallePedido = new DetallePedido
                            {
                                IdProducto = producto.Id,
                                IdPedido = idPedido,
                                Cantidad = cantidad,
                                PrecioUnitario = producto.Precio
                            };

                            int detallePedidoId = _detallepedidoRepository.InsertReturnId(detallePedido);
                            if (detallePedidoId <= 0)
                            {
                                throw new Exception("Error al crear el detalle de pedido.");
                            }

                            // Crear detalle de factura
                            DetalleFactura detalleFactura = new DetalleFactura
                            {
                                IdFactura = facturaId,
                                IdProducto = producto.Id,
                                Cantidad = cantidad,
                                PrecioUnitario = producto.Precio
                            };

                            int detalleFacturaId = _detallefacturaRepository.InsertReturnId(detalleFactura);
                            if (detalleFacturaId <= 0)
                            {
                                throw new Exception("Error al crear el detalle de factura.");
                            }

                            // Restar la cantidad al stock del producto
                            var stock_actual = producto.Stock - cantidad;
                            var productoCompleto = _productoRepository.GetById(producto.Id);
                            productoCompleto.Stock = stock_actual;

                            _productoRepository.Update(productoCompleto);

                        }


                        transaction.Commit();

                        //para mostrar en la vista
                        var productosComprados = new List<dynamic>();


                        foreach (var prod in productos)
                        {
                            if (prod == null || !cantidades.ContainsKey(prod.Id))
                            {
                                continue;
                            }

                            var cantidad = cantidades[prod.Id];
                            var subtotal = subtotales[prod.Id];

                            productosComprados.Add(new
                            {
                                Nombre = prod.Nombre,
                                Cantidad = cantidad,
                                PrecioUnitario = prod.Precio,
                                Subtotal = subtotal
                            });

                        }


                    // Pasar datos a la vista usando TempData
                    TempData["ProductosComprados"] = JsonConvert.SerializeObject(productosComprados);
                    TempData["Total"] = pago.Total;
                    TempData["NumeroPedido"] = pedido.Numero;
                    TempData["NumeroFactura"] = factura.Numero;
                    TempData["FechaFactura"] = factura.Fecha.ToString("dd/MM/yyyy");
                    TempData["MedioPago"] = $"{pago.TipoTarjeta} - {pago.Cuotas} cuotas";
                    TempData["MontoFactura"] = factura.Monto;
                    TempData["UserId"] = Session["UserId"];
                    return RedirectToAction("ConfirmacionPagoCarrito");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", $"Hubo un error procesando el pago: {ex.Message}");
                        return View(pago);
                    }
                }
           // }

            // Si el modelo no es válido
           // return View(pago);
        }
        //vista de confirmacion del carrito
        public ActionResult ConfirmacionPagoCarrito()
        {
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
