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
    public class UsuarioController : Controller
    {
        //private ApiDbContext db = new ApiDbContext();
        
        private readonly UsuarioData _usuarioRepository;
       
        public UsuarioController()
        {
            
            _usuarioRepository = new UsuarioData();
        }
        // GET: Usuario
        public ActionResult Index()
        {
            return View(_usuarioRepository.List());
        }

        // GET: Usuario/Details/5
        public ActionResult Details(int id)
        {
            /*if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/
            Usuario usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,Password,Rol,Nombre,Apellido")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _usuarioRepository.Insert(usuario);
                /*db.Usuarios.Add(usuario);
                db.SaveChanges();*/
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public ActionResult Edit(int id)
        {
            /*if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/

            Usuario usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Password,Rol,Nombre,Apellido")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                /*db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();*/
                _usuarioRepository.Update(usuario);
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public ActionResult Delete(int id)
        {   //al momento de eliminar un usuario hay que validar que
            //si es comprador no tenga compras asociadas.
            /*if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }*/
            Usuario usuario = _usuarioRepository.GetById(id);
            
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            /*Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();*/
            Usuario usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            usuario.Activo = false;
            _usuarioRepository.Update(usuario);
            return RedirectToAction("Index");
        }

    }
}
