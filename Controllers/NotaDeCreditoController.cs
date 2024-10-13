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
using iText.Layout;
using iText.IO.Image;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas.Draw;
using TechSolutions.Models;


namespace TechSolutions.Controllers
{
    public class NotaDeCreditoController : Controller
    {
        private readonly NotaDeCreditoData _notaCreditoRepository;
        
        public NotaDeCreditoController(){
            _notaCreditoRepository = new NotaDeCreditoData();

        }

        [HttpGet]
        public ActionResult GenerarNotaCredito(int? numeroNotaCredito, int? idFactura)
        {
            NotaDeCredito notaCredito = null;

            if (numeroNotaCredito.HasValue)
            {
                // Si se pasa el número de nota de crédito, se obtiene de esa manera
                notaCredito = _notaCreditoRepository.GetByNumero(numeroNotaCredito.Value);
            }
            else if (idFactura.HasValue)
            {
                // Si se pasa el ID de factura, se obtiene la nota de crédito asociada
                notaCredito = _notaCreditoRepository.GetByFacturaId(idFactura.Value);
                numeroNotaCredito = notaCredito.Numero;
            }

            if (notaCredito == null)
            {
                return HttpNotFound("Nota de crédito no encontrada");
            }

            // Crear un MemoryStream para almacenar el archivo PDF
            using (MemoryStream ms = new MemoryStream())
            {
                // Inicializar el documento PDF
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Definir estilos (similar a la factura)
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

                // Agregar logo de la empresa
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

                // Título de la nota de crédito
                Paragraph creditNoteTitle = new Paragraph($"Nota de Crédito N° {notaCredito.Numero}")
                    .AddStyle(headerStyle)
                    .SetTextAlignment(TextAlignment.CENTER);

                document.Add(creditNoteTitle);

                // Línea separadora
                document.Add(new LineSeparator(new SolidLine()));

                // Información de la factura original
                Paragraph originalInvoiceDetails = new Paragraph()
                    .Add($"Factura Original: N° {notaCredito.EncabezadoFactura.Numero}\n")
                    .Add($"Fecha de emisión: {notaCredito.Fecha_emision.ToString("dd/MM/yyyy")}\n")
                    .AddStyle(subHeaderStyle);

                document.Add(originalInvoiceDetails);

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
                foreach (var item in notaCredito.DetallesNotasCreditos)
                {
                    if (item.Producto != null)
                    {
                        decimal subtotal = item.Cantidad * (decimal)item.PrecioUnitario;
                        total += subtotal;

                        table.AddCell(new Cell().Add(new Paragraph(item.Producto.Nombre)));
                        table.AddCell(new Cell().Add(new Paragraph(item.Cantidad.ToString())).SetTextAlignment(TextAlignment.CENTER));
                        table.AddCell(new Cell().Add(new Paragraph(Convert.ToString("$ " + item.PrecioUnitario))).SetTextAlignment(TextAlignment.RIGHT));
                        table.AddCell(new Cell().Add(new Paragraph(Convert.ToString("$ " + subtotal))).SetTextAlignment(TextAlignment.RIGHT));
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

                // Mostrar los totales
                Paragraph totals = new Paragraph()
                    .Add($"Total: $ {total}\n")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(12)
                    .SetBold();

                document.Add(totals);

                // Línea separadora
                document.Add(new LineSeparator(new SolidLine()));

                // Información adicional o términos y condiciones
                Paragraph footer = new Paragraph()
                    .Add("Gracias por su atención.\n")
                    .Add("Esta nota de crédito es un comprobante legal de su transacción.\n")
                    .Add("Por favor, conserve este documento para sus registros.\n")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10);

                document.Add(footer);

                // Cerrar el documento PDF
                document.Close();

                // Devolver el archivo PDF como descargable
                byte[] file = ms.ToArray();
                return File(file, "application/pdf", $"NotaCredito_{numeroNotaCredito}.pdf");
            }
        }

    }
}
