using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TechSolutions.Data;
using TechSolutions.Models;
using TechSolutions.Servicios;

namespace TechSolutions.Controllers
{
    public class RegistroController : Controller
    {
        private readonly UsuarioData _usuarioData;
       


        public RegistroController()
        {
            _usuarioData = new UsuarioData();
            
        }


        //GET: Registro
        public ActionResult Index()
        {
            return View();
        }

        // GET: Registro/Details/5
        /*public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }*/

        //GET: Registro/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Registro/Create
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,Password,Nombre,Apellido")] Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
            {
                ModelState.AddModelError("Nombre", "El campo nombre es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(usuario.Apellido))
            {
                ModelState.AddModelError("Apellido", "El campo Apellido es obligatorio.");
            }

            var passSinEncriptar = ""; 

            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                ModelState.AddModelError("Password", "La contraseña es obligatoria.");
            }
            else
            {
                var password = usuario.Password;
                var regex = new System.Text.RegularExpressions.Regex("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{7,}$");

                if (!regex.IsMatch(password))
                {
                    ModelState.AddModelError("Password", "La contraseña debe tener al menos una letra, un número y siete caracteres.");
                }
                else
                {
                    usuario.Password = PasswordHelper.HashPassword(password);
                    passSinEncriptar = password;
                }
            }

            if (string.IsNullOrWhiteSpace(usuario.Email))
            {
                ModelState.AddModelError("Email", "El correo electrónico es obligatorio.");
            }
            else
            {
                var email = usuario.Email;

                try
                {
                    var mailAddress = new System.Net.Mail.MailAddress(email);
                }
                catch
                {
                    ModelState.AddModelError("Email", "El correo electrónico no es válido.");
                }
            }

            //Verificar si el correo ya está en uso
            var usuarioExistente = _usuarioData.FindByEmail(usuario.Email);
            
            if (usuarioExistente != null)
            {
                ModelState.AddModelError("Email", "El correo electrónico ya está en uso.");
            }

            //el registro es siempre para el cliente.
            usuario.Rol = SharedKernel.Rol.Cliente;

            if (ModelState.IsValid)
            {
                bool isInserted = _usuarioData.InsertUsuario(usuario);

                if (isInserted)
                {
                    var emailService = new EmailService();
                    string subject = "Registro exitoso en Tech Solutions";
                    string body = $"Hola {usuario.Nombre},<br/><br/>" +
                                  "Tu cuenta ha sido creada exitosamente en Tech Solutions.<br/><br/>" +
                                  $"Correo electrónico: {usuario.Email}<br/>" +
                                  $"Contraseña: {passSinEncriptar}<br/><br/>" +
                                  "Gracias por registrarte con nosotros.";

                    // Enviar el correo al usuario registrado
                    emailService.SendEmail(usuario.Email, subject, body);

                    TempData["SuccessMessage"] = "¡Registro exitoso! Por favor, verifica tu correo electrónico para más detalles.";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    //Añadir un mensaje de error si la inserción falló
                    ModelState.AddModelError("", "Ocurrió un error al crear el usuario. Inténtelo nuevamente.");
                }
            }

            return View(usuario);

        }/*
        // GET: Registro/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Registro/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Password,Rol,Activo,Nombre,Apellido")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuario);
        }

        // GET: Registro/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Registro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }*/
    }
}
