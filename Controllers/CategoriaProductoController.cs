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
        public CategoriaProductoController()
        {
            
            _categoriaProductoData = new CategoriaProductoData();
        }

        // GET
        public ActionResult Index()
        {
            return View(_categoriaProductoData.List());
           
        }

        // GET: CategoriaProducto/Details/5
        public ActionResult Details(int id)
        {
            /*if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/
            
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
                /*db.Categorias.Add(categoriaProducto);
                db.SaveChanges();*/
                _categoriaProductoData.Insert(categoriaProducto);
                return RedirectToAction("Index");
            }

            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Edit/5
        public ActionResult Edit(int id)
        {
            /*if (id == null)
             * 
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/

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
                /*db.Entry(categoriaProducto).State = EntityState.Modified;
                db.SaveChanges();*/
                return RedirectToAction("Index");
            }
            return View(categoriaProducto);
        }

        // GET: CategoriaProducto/Delete/5
        public ActionResult Delete(int id)
        {
            /*if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/

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
            
            categoriaProducto.Activo= false;
            _categoriaProductoData.Update(categoriaProducto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /*protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }*/
    }
}
