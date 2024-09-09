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

namespace TechSolutions.Controllers
{
    public class EncabezadoFacturaController : Controller
    {
        private readonly EncabezadoFacturaData _facturaRepository;

        public EncabezadoFacturaController()
        {
            _facturaRepository = new EncabezadoFacturaData();
        }
        [HttpGet]
        public ActionResult GenerarFactura(int numeroFactura)
        {
            // Obtener los datos de la factura desde la base de datos
            var factura = _facturaRepository.GetByNumero(numeroFactura);

            if (factura == null)
            {
                // Mostrar mensaje de error si la factura no se encuentra
                return HttpNotFound("Factura no encontrada");
            }

            // Crear un MemoryStream para almacenar el archivo PDF
            using (MemoryStream ms = new MemoryStream())
            {
                // Inicializar el documento PDF
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Añadir contenido al PDF
                document.Add(new Paragraph($"Factura #{factura.Numero}")
                    .SetFontSize(20)
                    .SetFontColor(ColorConstants.BLUE)
                    .SetBold());

                document.Add(new Paragraph($"Fecha de la factura: {factura.Fecha.ToString("dd/MM/yyyy")}"));
                document.Add(new Paragraph($"Cliente: {factura.Usuario.Nombre}"));

                // Crear una tabla para los detalles de los productos
                Table table = new Table(4, true);
                table.AddHeaderCell("Producto");
                table.AddHeaderCell("Cantidad");
                table.AddHeaderCell("Precio Unitario");
                table.AddHeaderCell("Subtotal");

                foreach (var item in factura.DetallesFacturas)
                {
                    table.AddCell(item.Producto.Nombre); // Asegúrate de que 'Nombre' esté presente en Producto
                    table.AddCell(item.Cantidad.ToString());
                    table.AddCell(item.PrecioUnitario.ToString("C"));
                    table.AddCell((item.Cantidad * item.PrecioUnitario).ToString("C"));
                }

                document.Add(table);

                // Total
                document.Add(new Paragraph($"Total: {factura.Monto.ToString("C")}")
                    .SetFontSize(16)
                    .SetBold());

                // Cerrar el documento PDF
                document.Close();

                // Devolver el archivo PDF como descargable
                byte[] file = ms.ToArray();
                return File(file, "application/pdf", $"Factura_{numeroFactura}.pdf");
            }
        }
    




    }
}
