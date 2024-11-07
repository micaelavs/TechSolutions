using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TechSolutions.Data;
using TechSolutions.Models;
using PagedList;
using System.Diagnostics;

namespace TechSolutions.Controllers
{
    public class ProductoController : Controller
    {
        //private ApiDbContext db = new ApiDbContext();
        
        private readonly ProductoData _productoRepository;
        private readonly CategoriaProductoData _categoriaProductoData;
        private readonly CalificacionProductoData _calificacionProductoRepository;
        private readonly DetallePedidoData _detallePedidoRepository;
        private readonly DetalleFacturaData _detalleFacturaRepository;

        public ProductoController()
        {
            _productoRepository = new ProductoData();
            _categoriaProductoData = new CategoriaProductoData();
            _calificacionProductoRepository = new CalificacionProductoData();
            _detalleFacturaRepository = new DetalleFacturaData();
            _detallePedidoRepository = new DetallePedidoData();
        }
        // GET este es listado del abm
        //biblioteca PagedList para dividir esos datos en páginas
        //se utiliza el metodo ToPagedList page, es el num actual de la pagina, y pageSize cuantos datos vas a mostrar
        /*
         devolves el objeto pagedList a la vista. Este objeto incluye información sobre los elementos de la página actual y 
         metadatos sobre la paginación, como el número total de páginas y si hay páginas anteriores o siguientes.
         */
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var productos = _productoRepository.List().ToList();
            var pagedList = productos.ToPagedList(page, pageSize);
            return View(pagedList);
        }

        // GET: Productoes/Details/5
        public ActionResult Details(int id)
        {
            Producto producto = _productoRepository.GetById(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        public ActionResult Create()
        {
            var producto = new Producto();
            ViewBag.IdCategoriaProducto = new SelectList(_categoriaProductoData.List(), "Id", "Nombre");
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Descripcion,Precio,IdCategoriaProducto,Stock,Foto")] Producto producto, HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {
                /*foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(modelError.ErrorMessage);
                }*/
                /*if (!string.IsNullOrEmpty(producto.Precio.ToString()))
                {
                    // Asegurarse de que Precio sea un número válido
                    try
                    {
                        // Convertir el precio a float explícitamente si es necesario
                        producto.Precio = Convert.ToSingle(producto.Precio);
                    }
                    catch (FormatException)
                    {
                        ModelState.AddModelError("Precio", "El precio debe ser un número válido.");
                    }
                }*/

                if (Foto != null && Foto.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(Foto.FileName);
                    string uniqueFileName = $"{Path.GetFileNameWithoutExtension(Foto.FileName)}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                    string uploadsPath = Server.MapPath("~/Uploads/");

                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    // Define la ruta completa donde se guardará el archivo
                    string path = Path.Combine(uploadsPath, uniqueFileName);

                    //Guarda el archivo en la ruta especificada
                    Foto.SaveAs(path);

                    // Aquí se guarda la ruta relativa completa
                    producto.Foto = uniqueFileName;
                }

                _productoRepository.Insert(producto);
                TempData["SuccessMessage"] = "Producto creado exitosamente!";
                return RedirectToAction("Index");
            }


            ViewBag.IdCategoriaProducto = new SelectList(
                _categoriaProductoData.List(),
                "Id",
                "Nombre",
                producto?.IdCategoriaProducto,
                "--Seleccione--"
            );
            return View(producto);
        }

        // GET: Productoes/Edit/5
        public ActionResult Edit(int id)
        {
            Producto producto = _productoRepository.GetById(id);
            
            if (producto == null)
            {
                return HttpNotFound();
            }

            ViewBag.IdCategoriaProducto = new SelectList(_categoriaProductoData.List(), "Id", "Nombre", producto.IdCategoriaProducto);
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Producto producto, HttpPostedFileBase foto)
        {
            //como imagen es obligatorio en el modelo pero... en la edicion, si no agrega una nueva foto, se deja la que esta
            //comento esto, si no tira error de modelo al validar
            //if (ModelState.IsValid)
            //{
                // Obtener el producto actual desde la base de datos para preservar la foto actual si no se ha subido una nueva
                var existingProduct = _productoRepository.GetById(producto.Id);

                if (existingProduct == null)
                {
                    return HttpNotFound();
                }
                /*if (!string.IsNullOrEmpty(producto.Precio.ToString()))
                {
                    // Asegurarse de que Precio sea un número válido
                    try
                    {
                        // Convertir el precio a float explícitamente si es necesario
                        producto.Precio = Convert.ToSingle(producto.Precio);
                    }
                    catch (FormatException)
                    {
                        ModelState.AddModelError("Precio", "El precio debe ser un número válido.");
                    }
                }*/
            // Verificar si se ha proporcionado un archivo de foto
            if (foto != null && foto.ContentLength > 0)
                {
                    // Guardar la imagen en el servidor
                    var fileName = Path.GetFileName(foto.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    foto.SaveAs(path);
                    producto.Foto = fileName;
                }
                else
                {
                    // Si no se proporciona una foto, mantener la foto existente
                    producto.Foto = existingProduct.Foto;
                }

                // Actualizar el producto
                _productoRepository.Update(producto);
                TempData["SuccessMessage"] = "Producto editado exitosamente!";
                return RedirectToAction("Index");
            //}

            /*ViewBag.IdCategoriaProducto = new SelectList(_categoriaProductoData.List(), "Id", "Nombre", producto.IdCategoriaProducto);
            return View(producto);*/
        }

        /*descontar unidades de producto*/
        // GET: Productoes/Descontar/5
        public ActionResult Descontar(int id)
        {
            Producto producto = _productoRepository.GetById(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productoes/Descontar/5
        [HttpPost, ActionName("Descontar")]
        [ValidateAntiForgeryToken]
        public ActionResult DescontarConfirmed(int id)
        {
         
            Producto producto = _productoRepository.GetById(id);
            // Validar si el producto está asociado a algún pedido o factura
            var detallesPedidos = _detallePedidoRepository.List().Where(dp => dp.IdProducto == producto.Id).ToList();
            var detallesFacturas = _detalleFacturaRepository.List().Where(df => df.IdProducto == producto.Id).ToList();

            if (detallesPedidos.Any() || detallesFacturas.Any())
            {
                TempData["ErrorMessage"] = "No se puede eliminar el producto, está asociado a una compra.";
                return RedirectToAction("Index");
            }

            if (producto.Stock >= 1)
            {
                producto.Stock -= 1;
            }
            else
            {
                producto.Stock = 0;
                producto.Activo = false;
            }

            _productoRepository.Update(producto);
            TempData["SuccessMessage"] = "Unidad de Producto eliminada exitosamente!";
            return RedirectToAction("Index");
        }


        /*eliminar producto por completo*/
        // GET: Productoes/Delete/5
        public ActionResult Delete(int id)
        {
            Producto producto = _productoRepository.GetById(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            Producto producto = _productoRepository.GetById(id);
            // Validar si el producto está asociado a algún pedido o factura
            var detallesPedidos = _detallePedidoRepository.List().Where(dp => dp.IdProducto == producto.Id).ToList();
            var detallesFacturas = _detalleFacturaRepository.List().Where(df => df.IdProducto == producto.Id).ToList();

            if (detallesPedidos.Any() || detallesFacturas.Any())
            {
                TempData["ErrorMessage"] = "No se puede eliminar el producto, está asociado a una compra.";
                return RedirectToAction("Index");
            }

            if (producto.Activo)
            {
                producto.Activo = false;
            }

            _productoRepository.Update(producto);
            TempData["SuccessMessage"] = "Producto eliminado exitosamente!";
            return RedirectToAction("Index");
        }

        //get para mostrar la pantalla
        // GET: Producto/Calificar/5
        public ActionResult Calificar(int id,int idUsuario)
        {
            var producto = _productoRepository.GetById(id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            var calificacion = new CalificacionProducto
            {
                IdProducto = id,
                Producto = producto,
                IdUsuario = idUsuario
            };

            return View(calificacion);
        }

        // POST: Producto/Calificar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Calificar([Bind(Include = "IdProducto,IdUsuario,Puntaje,Comentario")] CalificacionProducto calificacion)
        {
            if (ModelState.IsValid)
            {
                _calificacionProductoRepository.Insert(calificacion);
                TempData["SuccessMessage"] = "Calificación cargada correctamente.";
                //var idUsuario = Session["UserId"];
                //redirige a la vista de compras de es usuario...porque califica desdeahi
                return RedirectToAction("ComprasUsuario", "EncabezadoFactura", new { idUsuario = calificacion.IdUsuario });
            }

            var producto = _productoRepository.GetById(calificacion.IdProducto);
            calificacion.Producto = producto;
            return View(calificacion);
        }

        // GET: Producto/ModificarCalificacion/5
        //el id es del registro de calificacion
        public ActionResult ModificarCalificacion(int id)
        {
            var calificacion = _calificacionProductoRepository.GetById(id);

            if (calificacion == null)
            {
                return HttpNotFound();
            }

            var producto = _productoRepository.GetById(calificacion.IdProducto);
            if (producto == null)
            {
                return HttpNotFound();
            }

            //Asignamos el producto a la calificación para mostrar información del producto en la vista.
            calificacion.Producto = producto;

            return View(calificacion);
        }

        // POST: Producto/ModificarCalificacion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModificarCalificacion([Bind(Include = "Id,IdProducto,IdUsuario,Puntaje,Comentario")] CalificacionProducto calificacion)
        {
            if (ModelState.IsValid)
            {
                _calificacionProductoRepository.Update(calificacion);  // Actualizamos la calificación existente.
                TempData["SuccessMessage"] = "Calificación modificada correctamente.";

                //Redirige nuevamente a la vista de compras del usuario.
                return RedirectToAction("ComprasUsuario", "EncabezadoFactura", new { idUsuario = calificacion.IdUsuario });
            }

            //Si hay errores, volvemos a cargar el producto para mostrar la vista con datos completos.
            var producto = _productoRepository.GetById(calificacion.IdProducto);
            calificacion.Producto = producto;

            return View(calificacion);
        }

        //get
        public ActionResult VerCalificaciones(int id)
        {
            var producto = _productoRepository.GetById(id);

            if (producto == null)
            {
                return HttpNotFound();
            }

           
            double promedioPuntaje = producto.Calificaciones.Any()
                ? producto.Calificaciones.Average(c => (int)c.Puntaje)
                : 0;

            ViewBag.PromedioPuntaje = promedioPuntaje;

            return View(producto);
        }

        //get
        public ActionResult Listado()
        {
            var productos = _productoRepository.ListProductosDisponibles();

            return View(productos);
        }
        public ActionResult Detalle(int id)
        {
            var producto = _productoRepository.GetById(id);

            if (producto == null)
            {
                return HttpNotFound();
            }

            return View(producto);
        }

        /*para reporte*/
        public ActionResult ProductosEnStockCriticoIndex()
        {
            // Definir el nivel crítico
            int stockCritico = 3;

            // Obtener los productos en stock crítico
            var productosCriticos = _productoRepository.List()
                .Where(p => p.Stock < stockCritico)
                .Select(p => new ProductoReporteStockDTO
                {
                    ProductoNombre = p.Nombre,
                    Stock = p.Stock,
                    Categoria = p.CategoriaProducto.Nombre // Asumiendo que tienes una propiedad para la categoría
                })
                .ToList();

            ViewBag.StockCritico = productosCriticos; // Para usar en la vista

            return View(productosCriticos);
        }




    }


}
