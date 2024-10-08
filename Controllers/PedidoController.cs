using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechSolutions.Data;
using TechSolutions.Models;
using TechSolutions.Servicios;
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
        private readonly HistorialPedidoData _historialPedidoRepository;
        private readonly SolicitudDevolucionData _solicitudDevolucionRepository;
        private readonly DetalleDevolucionData _detalleDevolucionRepository;
        private readonly NotaDeCreditoData _notaDeCreditoRepository;
        private readonly DetalleNotaCreditoData _detalleNotaCreditoRepository;

        public PedidoController()
        {

            _detallepedidoRepository = new DetallePedidoData();
            _productoRepository = new ProductoData();
            _pedidoRepository = new PedidoData();
            _encabezadoFacturaRepository = new EncabezadoFacturaData();
            _detallefacturaRepository = new DetalleFacturaData();
            _historialPedidoRepository = new HistorialPedidoData();
            _solicitudDevolucionRepository = new SolicitudDevolucionData();
            _detalleDevolucionRepository = new DetalleDevolucionData();
            _notaDeCreditoRepository = new NotaDeCreditoData();
            _detalleNotaCreditoRepository = new DetalleNotaCreditoData();
        }
       
        public ActionResult Details(int id)
        {
            Pedido pedido = _pedidoRepository.GetById(id);

            if (pedido == null)
            {
                return HttpNotFound();
            }

            return View(pedido);
        }

        public ActionResult DetallesDevolucion(int id)
        {
            // Obtiene el pedido por su ID
            var pedido = _pedidoRepository.GetById(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            var solicitudDevolucion = _solicitudDevolucionRepository.List()
            .FirstOrDefault(sd => sd.IdPedido == pedido.Id);

            if (solicitudDevolucion == null)
            {
                return HttpNotFound();
            }

            return View(solicitudDevolucion);

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
                        //logueo el estado del pedido.
                        HistorialPedido historialPedido = new HistorialPedido
                        {
                            IdPedido = idPedido,
                            EstadoPedido = EstadoPedido.Pago_aprobado,
                            FechaOperacion = DateTime.Now
                        };

                        // Insertar el historial del pedido
                        int historialPedidoId= _historialPedidoRepository.InsertReturnId(historialPedido);
                        
                        if (historialPedidoId<=0)
                        {
                            throw new Exception("Error al crear el historial pedido.");
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
                            TipoFactura = TipoFactura.C,
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


                        //para envio de mail
                        // Crear una lista con el producto
                        var productos = new List<Producto> { producto };
                        var cantidades = new Dictionary<int, int> { { producto.Id, pago.Cantidad } };
                        EnviarCorreoConfirmacionCompra(productos, cantidades, pedido, factura, pago, (string)Session["Email"]);


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
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                ModelState.AddModelError("", $"Error de validación en la entidad {validationErrors.Entry.Entity.GetType().Name}, propiedad {validationError.PropertyName}: {validationError.ErrorMessage}");
                            }
                        }

                        transaction.Rollback();
                        return View(pago);
                    }
                }
            }

            // Si el modelo no es válido
            return View(pago);
        }

        private void EnviarCorreoConfirmacionCompra(List<Producto> productos, Dictionary<int, int> cantidades, Pedido pedido, EncabezadoFactura factura, PagoViewModel pago, string emailUsuario)
        {
            var emailBody = new StringBuilder();
            emailBody.AppendLine("<h1>Confirmación de tu compra</h1>");
            emailBody.AppendLine("<p>Gracias por tu compra. Aquí te enviamos el resumen:</p>");
            emailBody.AppendLine($"<p>Número de Pedido: {pedido.Numero}</p>");
            emailBody.AppendLine($"<p>Número de Factura: {factura.Numero}</p>");
            emailBody.AppendLine("<ul>");

            foreach (var producto in productos)
            {
                if (producto != null && cantidades.ContainsKey(producto.Id))
                {
                    var cantidad = cantidades[producto.Id];
                    emailBody.AppendLine($"<li>Producto: {producto.Nombre} - Cantidad: {cantidad} - Precio Unitario: {producto.Precio}</li>");
                }
            }

            emailBody.AppendLine("</ul>");
            emailBody.AppendLine($"<p>Total: {pago.Total}</p>");
            emailBody.AppendLine($"<p>Fecha: {factura.Fecha.ToString("dd/MM/yyyy")}</p>");

            string ultimosDigitosTarjeta = factura.Nrotarjeta.Substring(factura.Nrotarjeta.Length - 4);
            emailBody.AppendLine($"<p>Medio de Pago: {pago.TipoTarjeta} terminada en {ultimosDigitosTarjeta} - {pago.Cuotas} cuotas</p>");
            emailBody.AppendLine("<p>Gracias por confiar en Tech Solutions!</p>");

            var emailService = new EmailService();
            emailService.SendEmail(emailUsuario, "Confirmación de tu compra", emailBody.ToString());
        }


        //get mustra la vista confirmacion de pago de un tipo de prodcto
        [HttpGet]
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

                        //loguear estado de pedido
                        HistorialPedido historialPedido = new HistorialPedido
                        {
                            IdPedido = idPedido,
                            EstadoPedido = EstadoPedido.Pago_aprobado,
                            FechaOperacion = DateTime.Now
                        };

                        // Insertar el historial del pedido
                        int historialPedidoId = _historialPedidoRepository.InsertReturnId(historialPedido);

                        if (historialPedidoId <= 0)
                        {
                            throw new Exception("Error al crear el historial pedido.");
                        }


                    // Crear encabezado de factura una sola vez
                    EncabezadoFactura factura = new EncabezadoFactura
                        {
                            Numero = GenerarNumeroPedido(),
                            TipoFactura = TipoFactura.C,
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

                    //para el envio del correo
                    EnviarCorreoConfirmacionCompra(productos, cantidades, pedido, factura, pago, (string)Session["Email"]);

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
        [HttpGet]
        public ActionResult ConfirmacionPagoCarrito()
        {
            return View();
        }

        //mustra el carrito
        public ActionResult Carrito()
        {
            return View();
        }
        public ActionResult Index()
        {
            var pedidos = _pedidoRepository.List();
            return View(pedidos);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var pedido =_pedidoRepository.GetById(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }
            ViewBag.Estados = Enum.GetValues(typeof(TechSolutions.SharedKernel.EstadoPedido))
                          .Cast<TechSolutions.SharedKernel.EstadoPedido>()
                          .Select(e => new SelectListItem
                          {
                              Value = ((int)e).ToString(),
                              Text = e.ToString()
                          }).ToList();
            return View(pedido);

        }

        [HttpPost]
        public ActionResult Edit(Pedido pedido)
        {
            string successMessage = ""; // Variable para almacenar el mensaje de éxito

            // Solo actualizar el estado del pedido
            using (var db = new ApiDbContext())
            {
                // Crear una instancia de Pedido con solo el ID
                var pedidoToUpdate = new Pedido { Id = pedido.Id };

                // Adjuntar el pedido al contexto
                db.Pedidos.Attach(pedidoToUpdate);

                // Modificar solo el estado
                pedidoToUpdate.Estado = pedido.Estado;

                // Indicar que solo el campo Estado ha sido modificado
                db.Entry(pedidoToUpdate).Property(p => p.Estado).IsModified = true;

                db.SaveChanges();

                // Loguear estado de pedido
                HistorialPedido historialPedido = new HistorialPedido
                {
                    IdPedido = pedido.Id,
                    EstadoPedido = pedido.Estado,
                    FechaOperacion = DateTime.Now
                };

                _historialPedidoRepository.Insert(historialPedido);

                // Obtener el pedido actualizado
                var pedidoResult = _pedidoRepository.GetById(pedido.Id);

                if (pedidoResult.Estado == EstadoPedido.Parcialmente_devuelto || pedidoResult.Estado == EstadoPedido.Devuelto)
                {
                    var solicitudDevolucion = _solicitudDevolucionRepository.GetSolicitudByPedidoId(pedido.Id);
                    if (solicitudDevolucion != null)
                    {
                        NotaDeCredito notaCredito = new NotaDeCredito
                        {
                            IdSolicitudDevolucion = solicitudDevolucion.Id,
                            Numero = GenerarNumeroDevolucion(),
                            IdFactura = solicitudDevolucion.IdFactura,
                            Monto = solicitudDevolucion.Monto,
                            EstadoNota = EstadoNotaCredito.Aplicada
                        };

                        var IdNotaCredito = _notaDeCreditoRepository.InsertReturnId(notaCredito);

                        foreach (var detalle in solicitudDevolucion.DetallesDevoluciones)
                        {
                            DetalleNotaCredito detalleNotaCredito = new DetalleNotaCredito
                            {
                                IdNotaCredito = IdNotaCredito,
                                IdProducto = detalle.IdProducto,
                                Cantidad = detalle.Cantidad,
                                PrecioUnitario = detalle.PrecioUnitario
                            };

                            _detalleNotaCreditoRepository.Insert(detalleNotaCredito);

                            var producto = db.Productos.Find(detalle.IdProducto);
                            if (producto != null)
                            {
                                producto.Stock += detalle.Cantidad; // Aumentar el stock con la cantidad devuelta
                                db.Entry(producto).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        solicitudDevolucion.EstadoSolicitud = EstadoSolicitud.Cerrada;
                        _solicitudDevolucionRepository.Update(solicitudDevolucion);

                        successMessage = "Estado de Pedido actualizado exitosamente y se ha generado la nota de crédito"; // Mensaje para el caso donde se genera la nota
                    }
                }
                else
                {
                    successMessage = "Estado de Pedido actualizado exitosamente"; // Mensaje para el caso donde no se genera la nota
                }
            }

            TempData["SuccessMessage"] = successMessage; // Asignar el mensaje al TempData

            return RedirectToAction("Index");
        }


        //pantalla devolucion de productos
        [HttpPost]
        public ActionResult Devolver(List<int> productos, int idPedido, int idFactura, int idUsuario)
        {
            var motivos = Enum.GetValues(typeof(Motivo)).Cast<Motivo>()
                .Select(m => new SelectListItem
                {
                    Value = ((int)m).ToString(),
                    Text = m.ToString().Replace("_", " ")
                })
                .ToList();

            ViewBag.Motivos = motivos;

            if (productos == null || !productos.Any())
            {
                TempData["ErrorMessage"] = "No se seleccionaron productos para devolver.";
                return RedirectToAction("ComprasUsuario", "EncabezadoFactura", new { idUsuario });
            }

            var detalles = _detallepedidoRepository.GetDetallesByPedidoId(idPedido);
            var pedido = _pedidoRepository.GetById(idPedido);

            if (pedido == null)
            {
                TempData["ErrorMessage"] = "El pedido no se encontró.";
                return RedirectToAction("ComprasUsuario", "EncabezadoFactura", new { idUsuario });
            }

            var productosADevolver = detalles
                .Where(d => productos.Contains(d.IdProducto))
                .Select(d => d.Producto)
                .ToList();

            var model = new DevolucionViewModel
            {
                ProductosADevolver = productosADevolver,
                NumeroPedido = pedido.Numero,
                IdFactura = idFactura,
                IdUsuario = idUsuario,
            };

            return View("Devolver", model);
        }

        [HttpPost]
        public ActionResult ConfirmarDevolucion(DevolucionViewModel model)
        {
            var db = new ApiDbContext();
            // Deserializar los datos serializados
            var productosJson = Request.Form["Productos"];
            var motivosJson = Request.Form["Motivos"];
            var descripcionesJson = Request.Form["Descripciones"];

            var productosADevolver = JsonConvert.DeserializeObject<List<Producto>>(productosJson);
            var motivos = JsonConvert.DeserializeObject<Dictionary<int, int>>(motivosJson);
            var descripciones = JsonConvert.DeserializeObject<Dictionary<int, string>>(descripcionesJson);
            // Verifica que descripciones no sea null
            if (descripciones == null)
            {
                descripciones = new Dictionary<int, string>(); // Inicializa si es null
            }
            // Crear una nueva solicitud de devolución
            SolicitudDevolucion solicitudDevolucion = new SolicitudDevolucion
            {
                NumeroDevolucion = GenerarNumeroDevolucion(),
                IdPedido = _pedidoRepository.GetIdByNumeroPedido(model.NumeroPedido),
                IdFactura = model.IdFactura,
                IdIUsuario = model.IdUsuario,
                FechaOperacion = DateTime.Now,
                EstadoSolicitud = EstadoSolicitud.Abierta,
                Monto = 0 // Inicializar en 0 y actualizar después
            };

            var detallesPedido = _detallepedidoRepository.GetDetallesByPedidoId(solicitudDevolucion.IdPedido);

            // Calcular el monto total
            float montoTotal = 0;

            foreach (var producto in productosADevolver)
            {
                var detallePedido = detallesPedido.FirstOrDefault(d => d.IdProducto == producto.Id);
                if (detallePedido != null)
                {
                    montoTotal += detallePedido.PrecioUnitario * detallePedido.Cantidad;
                }
            }

            // Asignar el monto total a la solicitud de devolución
            solicitudDevolucion.Monto = montoTotal;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Insertar la solicitud de devolución
                    int solicitudId = _solicitudDevolucionRepository.InsertReturnId(solicitudDevolucion);
                    //traigo el pedido, para buscar dentro del pedido los detallespedido que coincidan cn esos prod
                    var pedido = _pedidoRepository.GetById(solicitudDevolucion.IdPedido);

                    // Insertar el detalle de devolución
                    foreach (var producto in productosADevolver)
                    {
                        if (motivos.TryGetValue(producto.Id, out var motivoId))
                        {

                            var descripcion = descripciones.TryGetValue(producto.Id, out var desc) ? desc : null;
                            var detallePedido = pedido.DetallesPedidos.FirstOrDefault(d => d.IdProducto == producto.Id);

                            if (detallePedido != null)
                            {
                                var detalleDevolucion = new DetalleDevolucion
                                {   IdSolicitudDevolucion = solicitudId,
                                    IdProducto = producto.Id,
                                    Cantidad = detallePedido.Cantidad,
                                    PrecioUnitario = detallePedido.PrecioUnitario,
                                    Motivo = (Motivo)motivoId,
                                    Descripcion = descripcion
                                };

                                _detalleDevolucionRepository.Insert(detalleDevolucion);
                            }
                        }
                    }

                    // Actualizar el estado del pedido
                    pedido.Estado = EstadoPedido.En_proceso_devolucion;
                    _pedidoRepository.Update(pedido); // Asegúrate de tener un método para actualizar el pedido

                    // Loguear en historial de pedido
                    var historialPedido = new HistorialPedido
                    {
                        IdPedido = pedido.Id,
                        EstadoPedido = pedido.Estado,
                        FechaOperacion = DateTime.Now
                    };

                    _historialPedidoRepository.Insert(historialPedido);

                    // Confirmar la transacción
                    transaction.Commit();

                    TempData["SuccessMessage"] = "Devolución procesada con éxito.";
                    return RedirectToAction("ComprasUsuario", "EncabezadoFactura", new { model.IdUsuario });
                }
                catch (Exception ex)
                {
                    // Rollback si ocurre un error
                    transaction.Rollback();
                    TempData["ErrorMessage"] = "Error al procesar la devolución: " + ex.Message;
                    return RedirectToAction("ComprasUsuario", "EncabezadoFactura", new { model.IdUsuario });
                }
            }
        }
        public ActionResult NotasCredito(int id) //id es del pedido
        {
            var pedido = _pedidoRepository.GetById(id);
            if (pedido == null)
            {
                return HttpNotFound();
            }

            var solicitudesDevolucion = _solicitudDevolucionRepository.List()
           .Where(s => s.IdPedido == id) // Filtra las solicitudes por el IdPedido
           .ToList();

            var notasCredito = _notaDeCreditoRepository.List()
            .Where(n => solicitudesDevolucion.Any(s => s.Id == n.IdSolicitudDevolucion))
            .ToList();

            return View(notasCredito); 
        }

        private int GenerarNumeroDevolucion()
        {
            Random random = new Random();
            return random.Next(100000, 999999); // Genera un número entre 100000 y 999999
        }

    }   
}
