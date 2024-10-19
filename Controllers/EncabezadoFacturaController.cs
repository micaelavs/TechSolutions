using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechSolutions.Data;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using System.IO;
using iText.Kernel.Pdf.Canvas.Draw;
using TechSolutions.Models;
using TechSolutions.Interfaces;
using PagedList;

namespace TechSolutions.Controllers
{
    public class EncabezadoFacturaController : Controller
    {
        private readonly EncabezadoFacturaData _facturaRepository;
        private readonly CalificacionProductoData _calificacionRepository;

        public EncabezadoFacturaController()
        {
            _facturaRepository = new EncabezadoFacturaData();
            _calificacionRepository = new CalificacionProductoData();
        }
        [HttpGet]
        public ActionResult GenerarFactura(int numeroFactura)
        {
            var factura = _facturaRepository.GetByNumero(numeroFactura);

            if (factura == null)
            {
                return HttpNotFound("Factura no encontrada");
            }

            // Crear un MemoryStream para almacenar el archivo PDF
            using (MemoryStream ms = new MemoryStream())
            {
                // Inicializar el documento PDF
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Definir estilos
                Style headerStyle = new Style()
                    .SetFontSize(24)
                    .SetFontColor(ColorConstants.BLUE)
                    .SetBold();

                Style subHeaderStyle = new Style()
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.BLACK);

                Style tableHeaderStyle = new Style()
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetBold();

                // Agregar logo de la empresa (si tienes uno)
                // Asumiendo que tienes una imagen en el directorio 'wwwroot/images/logo.png'
                string logoPath = Server.MapPath("~/Content/Images/logo.jpg");
                if (System.IO.File.Exists(logoPath))
                {
                    Image logo = new Image(ImageDataFactory.Create(logoPath));
                    logo.SetHeight(50);
                    document.Add(logo);
                }

                // Agregar detalles de la empresa
                Paragraph companyDetails = new Paragraph()
                    .Add("Tech Solutions\n")
                    .Add("Avenida Córdoba 877, Capital Federal\n")
                    .Add("CABA, Buenos Aires, AC4511l\n")
                     .Add("Sucursal: Casa Cental, Buenos Aires.\n")
                    .Add("Teléfono: 123-456-7890\n")
                    .Add("Correo electrónico: info@techsolutions.com.ar\n")
                    .Add("CUIT: 30-12345678-9")
                    .AddStyle(subHeaderStyle);

                document.Add(companyDetails);

                // Título de la factura
                Paragraph invoiceTitle = new Paragraph($"Factura {factura.TipoFactura} N° {factura.Numero}")
                    .AddStyle(headerStyle)
                    .SetTextAlignment(TextAlignment.CENTER);

                document.Add(invoiceTitle);

                // Línea separadora
                document.Add(new LineSeparator(new SolidLine()));

                // Información del cliente
                Paragraph clientDetails = new Paragraph()
                    .Add($"Fecha de emisión: {factura.Fecha.ToString("dd/MM/yyyy")}\n")
                    .Add($"Cliente: {factura.Usuario.Nombre} {factura.Usuario.Apellido}\n")
                    .Add($"Correo electrónico: {factura.Usuario.Email}\n")
                    .AddStyle(subHeaderStyle);

                document.Add(clientDetails);

                // Espacio antes de la tabla
                document.Add(new Paragraph("\n"));

                // Crear una tabla para los detalles de los productos
                float[] columnWidths = { 4, 1, 2, 2 };
                Table table = new Table(UnitValue.CreatePercentArray(columnWidths)).UseAllAvailableWidth();

                // Encabezados de la tabla
                table.AddHeaderCell(new Cell().Add(new Paragraph("Producto").AddStyle(tableHeaderStyle)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Cantidad").AddStyle(tableHeaderStyle)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Precio Unitario").AddStyle(tableHeaderStyle)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Subtotal").AddStyle(tableHeaderStyle)));

                // Añadir filas a la tabla
                decimal total = 0;
                foreach (var item in factura.DetallesFacturas)
                {
                    if (item.Producto != null)
                    {
                        decimal subtotal = item.Cantidad * (decimal)item.PrecioUnitario;
                        total += subtotal;

                        table.AddCell(new Cell().Add(new Paragraph(item.Producto.Nombre)));
                        table.AddCell(new Cell().Add(new Paragraph(item.Cantidad.ToString())).SetTextAlignment(TextAlignment.CENTER));
                        table.AddCell(new Cell().Add(new Paragraph(Convert.ToString("$ "+item.PrecioUnitario))).SetTextAlignment(TextAlignment.RIGHT));
                        table.AddCell(new Cell().Add(new Paragraph(Convert.ToString("$ "+subtotal))).SetTextAlignment(TextAlignment.RIGHT));
                    }
                    else
                    {
                        // Manejar el caso donde Producto es null
                        table.AddCell(new Cell().Add(new Paragraph("Producto no disponible")));
                        table.AddCell(new Cell().Add(new Paragraph(item.Cantidad.ToString())).SetTextAlignment(TextAlignment.CENTER));
                        table.AddCell(new Cell().Add(new Paragraph(Convert.ToString("$" + item.PrecioUnitario))).SetTextAlignment(TextAlignment.RIGHT));
                        table.AddCell(new Cell().Add(new Paragraph(Convert.ToString("$ " + item.Cantidad * (decimal)item.PrecioUnitario))).SetTextAlignment(TextAlignment.RIGHT));
                    }
                }

                // Añadir la tabla al documento
                document.Add(table);

                // Espacio antes de los totales
                document.Add(new Paragraph("\n"));

                // Calcular impuestos (por ejemplo, IVA 21%)
                //decimal iva = total * 0.21m;
                //decimal totalConIva = total + iva;

                // Mostrar los totales
                Paragraph totals = new Paragraph()
                    .Add($"Subtotal: $ {total}\n")
                    /*.Add($"IVA (21%): $ {iva}\n")*/
                    //.Add($"Total: $ {totalConIva}\n")
                    .Add($"Total: $ {total}\n")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(12)
                    .SetBold();

                document.Add(totals);

                // Línea separadora
                document.Add(new LineSeparator(new SolidLine()));

                // Información adicional o términos y condiciones
                Paragraph footer = new Paragraph()
                    .Add("Gracias por su compra.\n")
                    .Add("Esta factura es un comprobante legal de su transacción.\n")
                    .Add("Por favor, conserve este documento para sus registros.\n")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10);

                document.Add(footer);

                // Cerrar el documento PDF
                document.Close();

                // Devolver el archivo PDF como descargable
                byte[] file = ms.ToArray();
                return File(file, "application/pdf", $"Factura_{numeroFactura}.pdf");
            }
        }
        [HttpGet]
        public ActionResult ComprasUsuario(int idUsuario)
        {
            var facturas = _facturaRepository.GetAll()
            .Where(f => f.IdUsuario == idUsuario)
            .OrderByDescending(f => f.Fecha) 
            .ToList();

            var calificacionesUsuario = _calificacionRepository.GetCalificacionesByUsuario(idUsuario);

            var model = new MisComprasViewModel
            {
                Facturas = facturas,
                CalificacionesUsuario = calificacionesUsuario
            };

            if (!facturas.Any())
            {
                return RedirectToAction("SinCompras");
            }

            return View(model);
        }

        public ActionResult SinCompras()
        {
            return View();
        }

        public ActionResult Detalle(int id) //id es del pedido
        {
            var factura = _facturaRepository.GetFacturaPorPedidoId(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura); 
        }

        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var facturas = _facturaRepository.GetAll().ToList();
            var pagedList = facturas.ToPagedList(page, pageSize);
            return View(pagedList);
        }

        public ActionResult Details(int id)
        {
            var factura = _facturaRepository.GetById(id);
            if (factura == null)
            {
                return HttpNotFound("Factura no encontrada");
            }
            return View(factura); 

        }

        /******para los reportes****/
        public ActionResult VentasPorMes()
        {
            // Obtener las ventas agrupadas por mes
            var ventasPorMes = _facturaRepository.GetAll()
                .GroupBy(f => new { f.Fecha.Year, f.Fecha.Month })
                .Select(g => new VentaPorMesDTO
                {
                    Mes = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
                    Total = g.Sum(f => (decimal)f.Monto)
                })
                .ToList();

            // Meses del año
            var allMonths = Enumerable.Range(1, 12)
                .Select(m => new DateTime(DateTime.Now.Year, m, 1))
                .Select(d => d.ToString("MMMM yyyy")) // Formato de mes
                .ToList();

            // Combinar resultados
            var resultadosFinales = new List<VentaPorMesDTO>();
            var totalPorMes = new Dictionary<string, decimal>();

            foreach (var month in allMonths)
            {
                var venta = ventasPorMes.FirstOrDefault(v => v.Mes == month);
                totalPorMes[month] = venta != null ? venta.Total : 0; // Asigna el total o 0
            }

            // Crear el modelo para la vista
            foreach (var month in allMonths)
            {
                resultadosFinales.Add(new VentaPorMesDTO
                {
                    Mes = month,
                    Total = totalPorMes[month]
                });
            }

            return View(resultadosFinales);
        }


        public ActionResult VentasPorMesIndex()
        {
            var ventasPorMes = _facturaRepository.GetAll()
        .GroupBy(f => new { f.Fecha.Year, f.Fecha.Month })
        .Select(g => new VentaPorMesDTO
        {
            Mes = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy"),
            Total = (decimal)g.Sum(f => (double)f.Monto)
        })
        .ToList();

            // Meses del año
            var allMonths = Enumerable.Range(1, 12)
                .Select(m => new DateTime(DateTime.Now.Year, m, 1))
                .ToList();

            // Combinar resultados
            var resultadosFinales = new List<VentaPorMesDTO>();

            foreach (var month in allMonths)
            {
                var mesFormateado = month.ToString("MMMM yyyy"); // Aquí se usa correctamente
                var venta = ventasPorMes.FirstOrDefault(v => v.Mes == mesFormateado);
                resultadosFinales.Add(new VentaPorMesDTO
                {
                    Mes = mesFormateado,
                    Total = venta != null ? venta.Total : 0 // Usar el total real si existe
                });
            }

            return View(resultadosFinales);
        }

        public ActionResult VentasPorCategoria()
        {
            var ventasPorCategoria = _facturaRepository.GetAll()
                .SelectMany(f => f.DetallesFacturas, (factura, detalle) => new
                {
                    CategoriaNombre = detalle.Producto.CategoriaProducto.Nombre,
                    Monto = detalle.PrecioUnitario * detalle.Cantidad
                })
                .GroupBy(x => x.CategoriaNombre)
                .Select(g => new
                {
                    CategoriaNombre = g.Key,
                    Total = g.Sum(x => x.Monto)
                })
                .ToList();

            return Json(ventasPorCategoria, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VentasPorCategoriaIndex()
        {
            var ventasPorCategoria = _facturaRepository.GetAll()
                .SelectMany(f => f.DetallesFacturas, (factura, detalle) => new
                {
                    CategoriaNombre = detalle.Producto.CategoriaProducto.Nombre,
                    Monto = (decimal)detalle.PrecioUnitario * detalle.Cantidad // Asegúrate de que PrecioUnitario sea decimal
                })
                .GroupBy(x => x.CategoriaNombre)
                .Select(g => new VentaPorCategoriaDTO
                {
                    CategoriaNombre = g.Key,
                    Total = g.Sum(x => (decimal)x.Monto) // Forzar a decimal aquí
                })
                .ToList();

            return View(ventasPorCategoria);
        }

        public ActionResult VentasPorProducto()
        {
            var ventasPorProducto = _facturaRepository.GetAll()
                .SelectMany(f => f.DetallesFacturas, (factura, detalle) => new
                {
                    ProductoNombre = detalle.Producto.Nombre,
                    Monto = detalle.PrecioUnitario * detalle.Cantidad
                })
                .GroupBy(x => x.ProductoNombre)
                .Select(g => new
                {
                    ProductoNombre = g.Key,
                    Total = g.Sum(x => x.Monto)
                })
                .ToList();

            var resultadosFinales = new List<dynamic>();
            foreach (var producto in ventasPorProducto)
            {
                resultadosFinales.Add(new
                {
                    ProductoNombre = producto.ProductoNombre,
                    Total = producto.Total
                });
            }

            return Json(resultadosFinales, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VentasPorProductoIndex()
        {
            var ventasPorProducto = _facturaRepository.GetAll()
                .SelectMany(f => f.DetallesFacturas, (factura, detalle) => new VentaPorProductoDTO
                {
                    ProductoNombre = detalle.Producto.Nombre,
                    Total = (decimal)detalle.PrecioUnitario * (decimal)detalle.Cantidad // Asegúrate de la conversión
                })
                .GroupBy(x => x.ProductoNombre)
                .Select(g => new VentaPorProductoDTO
                {
                    ProductoNombre = g.Key,
                    Total = g.Sum(x => x.Total)
                })
                .ToList();

            return View(ventasPorProducto);
        }




    }
}
