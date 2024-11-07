using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechSolutions.Data;
using TechSolutions.Servicios;
using TechSolutions.SharedKernel;

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

            // Verifica si el usuario está activo
            if (!usuario.Activo)
            {
                TempData["ErrorMessage"] = "El usuario está dado de baja. Contáctese con el Administrador del sistema.";
                return RedirectToAction("Contact", "Home");
            }


            // Autenticación exitosa, establecer sesión o cookie
            Session["UserId"] = usuario.Id; // O usar un mecanismo de autenticación más seguro como una cookie de autenticación
            Session["Email"] = usuario.Email;
            Session["Rol"] = usuario.Rol;

            TempData["SuccessMessage"] = "Inicio de sesión exitoso.";

            //Redirigir según el rol del usuario
            if (usuario.Rol == Rol.Cliente)
            {
                return RedirectToAction("Listado", "Producto");
                //podria redirigir a Mi perfil para qeu coincida con el escenario
            }
            else if (usuario.Rol == Rol.Administrador)
            {
                return RedirectToAction("Index", "Pedido");     
            }

            //Redirigir a una página predeterminada si el rol no coincide con los esperados
            return RedirectToAction("Index", "Home");
        }

        public JsonResult CheckSession()
        {
            // Devuelve un JSON que indica si la sesión está activa o no
            return Json(new { isValid = Session["UserId"] != null }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Logout()
        {

            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }

    }
}
