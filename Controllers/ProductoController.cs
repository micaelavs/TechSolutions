using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
        private ApiDbContext db = new ApiDbContext();
        
        private readonly ProductoData _productoRepository;
        private readonly CategoriaProductoData _categoriaProductoData;
        public ProductoController()
        {
            _productoRepository = new ProductoData();
            _categoriaProductoData = new CategoriaProductoData();
        }
        // GET
        public ActionResult Index()
        {
            /*var productos = db.Productos
                 .Include(p => p.CategoriaProducto)
                 .Where(p => p.Activo)
                 .ToList();

            return View(productos);*/

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
            //ESTO despuestengo que traerlo de la bd
            //ViewBag.IdCategoriaProducto = new SelectList(db.Categorias, "Id", "Nombre");
            ViewBag.IdCategoriaProducto = new SelectList(_categoriaProductoData.List(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Descripcion,Precio,IdCategoriaProducto,Stock,Foto,Activo")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                /*db.Productos.Add(producto);
                db.SaveChanges();*/
                _productoRepository.Insert(producto);
                return RedirectToAction("Index");
            }


            ViewBag.IdCategoriaProducto = new SelectList(_categoriaProductoData.List(), "Id", "Nombre", producto.IdCategoriaProducto);
            return View(producto);
        }

        // GET: Productoes/Edit/5
        public ActionResult Edit(int id)
        {
            /*if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/
            Producto producto = _productoRepository.GetById(id);
            
            if (producto == null)
            {
                return HttpNotFound();
            }

            ViewBag.IdCategoriaProducto = new SelectList(_categoriaProductoData.List(), "Id", "Nombre", producto.IdCategoriaProducto);
            return View(producto);
        }

        // POST: Productoes/Edit/5
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Descripcion,Precio,IdCategoriaProducto,Stock,Foto,Activo")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                /*db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();*/
                _productoRepository.Update(producto);
                return RedirectToAction("Index");
            }
            //ViewBag.IdCategoriaProducto = new SelectList(db.Categorias, "Id", "Nombre", producto.IdCategoriaProducto);
            return View(producto);
        }

        // GET: Productoes/Delete/5
        public ActionResult Delete(int id)
        {
            /*if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/
            //Producto producto = db.Productos.Find(id);
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
            /*Producto producto = db.Productos.Find(id);
            db.Productos.Remove(producto);
            db.SaveChanges();*/
            Producto producto = _productoRepository.GetById(id);
            producto.Activo = false;
            _productoRepository.Update(producto);
            return RedirectToAction("Index");
        }

    }
}
