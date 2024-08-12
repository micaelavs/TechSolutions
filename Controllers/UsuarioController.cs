using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechSolutions.Data;
using TechSolutions.Models;

namespace TechSolutions.Controllers
{
    public class UsuarioController : Controller
    {
     
        //vista registro
        public ActionResult Index()
        {
            return View();
        }

        // GET: Registro/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Registro/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,Password,Rol,Activo,Nombre,Apellido")] Usuario usuario)
        {
            /*if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }*/

            return View(usuario);
        }


    }
}
