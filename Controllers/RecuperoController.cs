using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TechSolutions.Data;
using TechSolutions.Models;
using TechSolutions.Servicios;

namespace TechSolutions.Controllers
{
    public class RecuperoController : Controller
    {
        //private ApiDbContext db = new ApiDbContext();
        public readonly UsuarioData _usuarioData;
        private readonly string _encryptionKey = "TuClaveDeEncriptacionSecreta"; //Debe ser segura y almacenada correctamente

        public RecuperoController() { 

            _usuarioData = new UsuarioData();
        }
        /**
         * Mecanismo de como recuperar la cuenta:
         * Generar un Token Seguro: Al recibir la solicitud de recuperación de cuenta, se genera un token seguro que 
         * contiene información del usuario y una marca de tiempo, y se cifra utilizando una clave secreta conocida solo por el servidor.
         * Enviar el Token por Correo Electrónico: El token se envía al usuario como parte de un enlace en un correo electrónico.
         * Verificar el Token al Acceder al Enlace: Cuando el usuario hace clic en el enlace, el servidor descifra el token, 
         * valida la información contenida (por ejemplo, que no haya expirado y que coincida con la información del usuario) y permite al usuario restablecer su contraseña.
         * Puntos Clave del Enfoque:
         * SIN persistencia de tokens: El token generado no se guarda en la base de datos. En cambio, se confía en la encriptación segura para mantener la integridad del proceso de recuperación.
         * Cifrado y Descifrado: La información se cifra al generar el token y se descifra cuando el usuario accede al enlace de restablecimiento.
         * Validación de Expiración: Se verifica que el token no haya expirado antes de permitir el restablecimiento de la contraseña.
         * **/


        // GET
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecuperarCuenta(string email)
        {
            var usuario = _usuarioData.FindByEmail(email);
            
            if (usuario != null)
            {
                string token = GenerarTokenSeguro(usuario.Email);
                string resetLink = Url.Action("RestablecerContrasena", "Recupero", new { token = token }, protocol: Request.Url.Scheme);

                // Enviar el correo con el enlace
                var emailService = new EmailService();
                emailService.SendEmail(usuario.Email, "Recupera tu cuenta", $"Haz clic en el siguiente enlace para recuperar tu cuenta: <a href='{resetLink}'>Recuperar Cuenta</a>");

                ViewBag.Message = "Se ha enviado un correo con las instrucciones para recuperar su cuenta.";
                return View("Confirmacion");
            }
            else
            {
                ModelState.AddModelError("", "No se encontró una cuenta con ese correo electrónico.");
                return View("Index");
            }
        }

        private string GenerarTokenSeguro(string email)
        {
            var data = $"{email}|{DateTime.UtcNow}";
            return Encriptar(data, _encryptionKey);
        }

        private string Encriptar(string clearText, string encryptionKey)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x53, 0x47, 0x18, 0x30, 0x77, 0x21, 0x38, 0x44, 0x71, 0x28, 0x39, 0x59, 0x47, 0x21, 0x30, 0x44 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private string Desencriptar(string cipherText, string encryptionKey)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x53, 0x47, 0x18, 0x30, 0x77, 0x21, 0x38, 0x44, 0x71, 0x28, 0x39, 0x59, 0x47, 0x21, 0x30, 0x44 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        [HttpGet]
        public ActionResult RestablecerContrasena(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Login");
            }

            try
            {
                string decryptedToken = Desencriptar(token, _encryptionKey);
                string[] tokenParts = decryptedToken.Split('|');
                string email = tokenParts[0];
                DateTime requestTime = DateTime.Parse(tokenParts[1]);

                // Verificar si el enlace ha expirado (por ejemplo, 1 hora de validez)
                if ((DateTime.UtcNow - requestTime).TotalHours > 1)
                {
                    ViewBag.Message = "El enlace de recuperación ha expirado.";
                    return View("Error");
                }

                // Mostrar formulario para restablecer la contraseña
                ViewBag.Email = email;
                return View();
            }
            catch
            {
                ViewBag.Message = "Enlace de recuperación inválido.";
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult RestablecerContrasena(string email, string newPassword)
        {
            var usuario = _usuarioData.FindByEmail(email);
            if (usuario != null)
            {
                // Actualizar la contraseña del usuario
                usuario.Password = PasswordHelper.HashPassword(newPassword); // Asegúrate de encriptar la contraseña antes de guardarla
                _usuarioData.Update(usuario);

                ViewBag.Message = "Tu contraseña ha sido restablecida exitosamente.";
                return View("Confirmacion");
            }

            ViewBag.Message = "Error al restablecer la contraseña.";
            return View("Error");
        }


        // GET: Recupero/Details/5
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
        }

        // GET: Recupero/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Recupero/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,Password,Rol,Activo,Nombre,Apellido")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // GET: Recupero/Edit/5
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

        // POST: Recupero/Edit/5
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

        // GET: Recupero/Delete/5
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

        // POST: Recupero/Delete/5
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
