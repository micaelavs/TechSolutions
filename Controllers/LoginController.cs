using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechSolutions.Data;
using TechSolutions.Servicios;

namespace TechSolutions.Controllers
{
    public class LoginController : Controller
    {
        public readonly UsuarioData _usuarioData;

        public LoginController()
        {
            _usuarioData= new UsuarioData();
        }
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("", "Por favor ingrese su correo electrónico y contraseña.");
                // Retorna la vista actual con los datos ingresados
                return View("Index");
            }

            var usuario = _usuarioData.FindByEmail(Email);
            if (usuario == null || !PasswordHelper.VerifyPassword(Password, usuario.Password))
            {
                ModelState.AddModelError("", "Correo electrónico o contraseña incorrectos.");
                //TempData["ErrorMessage"] = "Correo electrónico o contraseña incorrectos.";
                // Retorna la vista actual con los datos ingresados
                return View("Index");
            }

            // Autenticación exitosa, establecer sesión o cookie
            Session["UserId"] = usuario.Id; // O usar un mecanismo de autenticación más seguro como una cookie de autenticación

            TempData["SuccessMessage"] = "Inicio de sesión exitoso.";
            return RedirectToAction("Listado", "Producto");
        }

    }
}
