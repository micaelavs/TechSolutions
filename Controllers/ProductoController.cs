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

namespace TechSolutions.Controllers
{
    public class ProductoController : Controller
    {
        //private ApiDbContext db = new ApiDbContext();
        
        private readonly ProductoData _productoRepository;
        private readonly CategoriaProductoData _categoriaProductoData;
        private readonly CalificacionProductoData _calificacionProductoRepository;

        public ProductoController()
        {
            _productoRepository = new ProductoData();
            _categoriaProductoData = new CategoriaProductoData();
            _calificacionProductoRepository = new CalificacionProductoData();
        }
        // GET
        public ActionResult Index()
        {
            var productos = _productoRepository.List();
            //var productosActivos = productos.Where(p => p.Activo).ToList();
            return View(productos);
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
            ViewBag.IdCategoriaProducto = new SelectList(_categoriaProductoData.List(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Descripcion,Precio,IdCategoriaProducto,Stock,Foto")] Producto producto, HttpPostedFileBase Foto)
        {
            if (ModelState.IsValid)
            {

                if(Foto != null && Foto.ContentLength > 0)
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
            if (producto.Stock > 1)
            {
                producto.Stock -= 1;
            }
            else
            {
                producto.Stock = 0;
                producto.Activo = false;
            }

            _productoRepository.Update(producto);
            TempData["SuccessMessage"] = "Producto eliminado exitosamente!";
            return RedirectToAction("Index");
        }
        //get para mostrar la pantalla
        // GET: Producto/Calificar/5
        public ActionResult Calificar(int id)
        {
            var producto = _productoRepository.GetById(id);
            if (producto == null)
            {
                return HttpNotFound();
            }

            var calificacion = new CalificacionProducto
            {
                IdProducto = id,
                Producto = producto
            };

            return View(calificacion);
        }

        // POST: Producto/Calificar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Calificar([Bind(Include = "IdProducto,Puntaje,Comentario")] CalificacionProducto calificacion)
        {
            if (ModelState.IsValid)
            {
                _calificacionProductoRepository.Insert(calificacion);
                //vemos despues a donde redirije
                return RedirectToAction("Index");
            }

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
            var productos = _productoRepository.List();

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





    }


}
