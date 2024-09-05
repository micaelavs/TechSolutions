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
    public class CategoriaProductoController : Controller
    {
        private ApiDbContext db = new ApiDbContext();

        private readonly CategoriaProductoData _categoriaProductoData;
        private readonly Producto _productoData;
        public CategoriaProductoController()
        {
            
            _categoriaProductoData = new CategoriaProductoData();
            _productoData = new Producto();
        }

        // GET
        public ActionResult Index()
        {
            return View(_categoriaProductoData.List());
           
        }

        // GET: CategoriaProducto/Details/5
        public ActionResult Details(int id)
        {
            CategoriaProducto categoriaProducto = _categoriaProductoData.GetById(id);
            
            if (categoriaProducto == null)
            {
                return HttpNotFound();
            }
            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoriaProducto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Descripcion")] CategoriaProducto categoriaProducto)
        {
            if (ModelState.IsValid)
            {
                _categoriaProductoData.Insert(categoriaProducto);
                TempData["SuccessMessage"] = "Categoría creada exitosamente!";
                return RedirectToAction("Index");
            }

            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Edit/5
        public ActionResult Edit(int id)
        {

            CategoriaProducto categoriaProducto = _categoriaProductoData.GetById(id);

            if (categoriaProducto == null)
            {
                return HttpNotFound();
            }
            return View(categoriaProducto);
        }

        // POST: CategoriaProducto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Descripcion")] CategoriaProducto categoriaProducto)
        {
            if (ModelState.IsValid)
            {
                _categoriaProductoData.Update(categoriaProducto);
                TempData["SuccessMessage"] = "Categoría editada exitosamente!";
                return RedirectToAction("Index");
            }
            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Delete/5
        public ActionResult Delete(int id)
        {
            CategoriaProducto categoriaProducto = _categoriaProductoData.GetById(id);

            if (categoriaProducto == null)
            {
                return HttpNotFound();
            }
            return View(categoriaProducto);
        }

        // POST: CategoriaProducto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CategoriaProducto categoriaProducto = _categoriaProductoData.GetById(id);
            //antes de eliminar hay que revuisar que la categoria no tenga asociados productos, de lo contrario hay que mostrar un mensdaje
            var productosAsociados = db.Productos.Where(p => p.IdCategoriaProducto == id).ToList();
            
            if (productosAsociados.Any())
            {
                // Si hay productos asociados, mostrar un mensaje de error y redirigir a la vista de detalles
                TempData["ErrorMessage"] = "No se puede eliminar la categoría porque está asociada a uno o más productos.";
                return RedirectToAction("Index", new { id = id });
            }


            categoriaProducto.Activo= false;
            _categoriaProductoData.Update(categoriaProducto);
            TempData["SuccessMessage"] = "Categoría dada de baja exitosamente!";
            return RedirectToAction("Index");
        }

    }
}
