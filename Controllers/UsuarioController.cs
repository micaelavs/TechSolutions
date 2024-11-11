using iText.Kernel.Geom;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TechSolutions.Data;
using TechSolutions.Models;
using TechSolutions.Servicios;

namespace TechSolutions.Controllers
{
    public class UsuarioController : Controller
    {
        //private ApiDbContext db = new ApiDbContext();
        
        private readonly UsuarioData _usuarioRepository;
        private readonly PedidoData _pedidoRepository;
        private readonly EncabezadoFacturaData _facturaRepository;

        public UsuarioController()
        {
            
            _usuarioRepository = new UsuarioData();
            _pedidoRepository = new PedidoData();
            _facturaRepository = new EncabezadoFacturaData();
        }
        // GET: Usuario
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            
            var usuarios = _usuarioRepository.List().ToList();
            var pagedList = usuarios.ToPagedList(page, pageSize);
            return View(pagedList);
        }

        // GET: Usuario/Details/5
        public ActionResult Details(int id)
        {
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
                if (string.IsNullOrWhiteSpace(usuario.Nombre))
                {
                    ModelState.AddModelError("Nombre", "El campo nombre es obligatorio.");
                }

                if (string.IsNullOrWhiteSpace(usuario.Apellido))
                {
                    ModelState.AddModelError("Apellido", "El campo Apellido es obligatorio.");
                }

                if (usuario.Rol == 0)
                {
                    ModelState.AddModelError("Rol", "El campo Rol es obligatorio.");
                }

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
                        usuario.Password = PasswordHelper.HashPassword(usuario.Password); 
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
                        var mailAddress = new System.Net.Mail.MailAddress(email); // Verifica la validez del correo electrónico
                    }
                    catch
                    {
                        ModelState.AddModelError("Email", "El correo electrónico no es válido.");
                    }
                }

                var usuarioExistente = _usuarioRepository.FindByEmail(usuario.Email);

                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Email", "El correo electrónico ya está en uso.");
                }

                //usuario ya ingresado pero inactivo
                var usuarioExistenteInactivo = _usuarioRepository.FindByEmailInactivo(usuario.Email);

                if (usuarioExistenteInactivo != null)
                {
                    ModelState.AddModelError("Email", "El usuario con el Email ingresado ya está en el sistema pero Inactivo.");
                }

                if (!ModelState.IsValid)
                {
                    return View(usuario); 
                }

                _usuarioRepository.Insert(usuario);

                TempData["SuccessMessage"] = "El usuario se ha creado correctamente.";
                return RedirectToAction("Index");
            }

            // Si el modelo no es válido, devuelve la vista con los errores
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public ActionResult Edit(int id)
        {
            Usuario usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            var usuarioViewModel = new UsuarioEditViewModel
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Rol = usuario.Rol
                //no pongo la pass porque es opcional
            };

            return View(usuarioViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Password,Rol,Nombre,Apellido")] UsuarioEditViewModel usuarioViewModel)
        {
            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                // Obtiene el usuario actual desde el repositorio
                var usuarioActual = _usuarioRepository.GetById(usuarioViewModel.Id);

                if (usuarioActual == null)
                {
                    return HttpNotFound();
                }

                // Validación del nombre
                if (string.IsNullOrWhiteSpace(usuarioViewModel.Nombre))
                {
                    ModelState.AddModelError("Nombre", "El campo nombre es obligatorio.");
                }

                // Validación del apellido
                if (string.IsNullOrWhiteSpace(usuarioViewModel.Apellido))
                {
                    ModelState.AddModelError("Apellido", "El campo Apellido es obligatorio.");
                }

                // Validación de la contraseña
                if (string.IsNullOrWhiteSpace(usuarioViewModel.Password))
                {
                    usuarioViewModel.Password = usuarioActual.Password; // Mantiene la contraseña actual si no se ha ingresado una nueva
                }
                else
                {
                    var password = usuarioViewModel.Password;
                    var regex = new System.Text.RegularExpressions.Regex("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{7,}$");

                    if (!regex.IsMatch(password))
                    {
                        ModelState.AddModelError("Password", "La contraseña debe tener al menos una letra, un número y siete caracteres.");
                    }
                    else
                    {
                        usuarioViewModel.Password = PasswordHelper.HashPassword(usuarioViewModel.Password); // Encripta la nueva contraseña
                    }
                }

                // Validación del correo electrónico
                if (string.IsNullOrWhiteSpace(usuarioViewModel.Email))
                {
                    ModelState.AddModelError("Email", "El correo electrónico es obligatorio.");
                }
                else
                {
                    var email = usuarioViewModel.Email;

                    try
                    {
                        var mailAddress = new System.Net.Mail.MailAddress(email); // Verifica la validez del correo electrónico
                    }
                    catch
                    {
                        ModelState.AddModelError("Email", "El correo electrónico no es válido.");
                    }
                }

                // Verificar si el correo electrónico ya está en uso
                var usuarioExistente = _usuarioRepository.FindByEmailExcludingId(usuarioViewModel.Email, usuarioViewModel.Id);

                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Email", "El correo electrónico ya está en uso.");
                }

                //si es cliente y tiene compras asocuadas no puedo cambiar rol
                if (usuarioActual.Rol == SharedKernel.Rol.Cliente)
                {
                    // Verifica si el usuario tiene pedidos o facturas asociadas
                    bool tienePedido = _pedidoRepository.List().Any(p => p.IdUsuario == usuarioActual.Id);
                    bool tieneFactura = _facturaRepository.List().Any(f => f.IdUsuario == usuarioActual.Id);

                    if (tienePedido || tieneFactura)
                    {
                        ModelState.AddModelError("Rol", "No se puede cambiar el rol a 'Administrador' porque el usuario tiene pedidos / facturas asociadas.");
                    }
                }
                // Si el ModelState tiene errores, no continuar con la actualización
                if (!ModelState.IsValid)
                {
                    return View(usuarioViewModel); // Devuelve la vista actual con los mensajes de error
                }

                // Actualiza el usuario
                usuarioActual.Email = usuarioViewModel.Email;
                usuarioActual.Nombre = usuarioViewModel.Nombre;
                usuarioActual.Apellido = usuarioViewModel.Apellido;
                usuarioActual.Rol = usuarioViewModel.Rol;
                usuarioActual.Password = usuarioViewModel.Password; // Actualiza la contraseña (o mantiene la actual)

                _usuarioRepository.Update(usuarioActual);

                TempData["SuccessMessage"] = "El usuario se ha actualizado correctamente.";
                return RedirectToAction("Index");
            }

            // Si el modelo no es válido, devuelve la vista con los errores
            return View(usuarioViewModel);
        }

        // GET: Usuario/Delete/5
        public ActionResult Delete(int id)
        {  
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

            Usuario usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            // Verificar si el usuario tiene compras asociadas
            bool tienePedido = _pedidoRepository.List().Any(p => p.IdUsuario == id);
            bool tieneFactura = _facturaRepository.List().Any(f => f.IdUsuario == id);

            if (tienePedido || tieneFactura)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el usuario porque tiene compras asociadas.";
                return RedirectToAction("Index");
            }

            // Desactivar el usuario en lugar de eliminarlo
            usuario.Activo = false;
            _usuarioRepository.Update(usuario);

            TempData["SuccessMessage"] = "El usuario se ha desactivado correctamente.";
            return RedirectToAction("Index");
        }


        // GET: Usuario/MiPerfil
        public ActionResult MiPerfil(int id)
        {
            Usuario usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            var usuarioViewModel = new UsuarioEditViewModel
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Rol = usuario.Rol
                //no pongo la pass porque es opcional
            };

            //para que redirija a una vista u otra si presiona cancelar
            string redirectUrl = string.Empty;

            if (usuario.Rol == SharedKernel.Rol.Cliente)
            {
                redirectUrl = Url.Action("Listado", "Producto");
            }
            else
            {
                redirectUrl = Url.Action("Index", "Pedido");
            }

            ViewBag.RedirectUrl = redirectUrl;
            return View(usuarioViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MiPerfil([Bind(Include = "Id,Email,Password,Rol,Nombre,Apellido")] UsuarioEditViewModel usuarioViewModel)
        {

            // Verifica si el modelo es válido
            if (ModelState.IsValid)
            {
                // Obtiene el usuario actual desde el repositorio
                var usuarioActual = _usuarioRepository.GetById(usuarioViewModel.Id);

                if (usuarioActual == null)
                {
                    return HttpNotFound();
                }

                // Validación del nombre
                if (string.IsNullOrWhiteSpace(usuarioViewModel.Nombre))
                {
                    ModelState.AddModelError("Nombre", "El campo nombre es obligatorio.");
                }

                // Validación del apellido
                if (string.IsNullOrWhiteSpace(usuarioViewModel.Apellido))
                {
                    ModelState.AddModelError("Apellido", "El campo Apellido es obligatorio.");
                }

                // Validación de la contraseña
                if (string.IsNullOrWhiteSpace(usuarioViewModel.Password))
                {
                    usuarioViewModel.Password = usuarioActual.Password; // Mantiene la contraseña actual si no se ha ingresado una nueva
                }
                else
                {
                    var password = usuarioViewModel.Password;
                    var regex = new System.Text.RegularExpressions.Regex("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{7,}$");

                    if (!regex.IsMatch(password))
                    {
                        ModelState.AddModelError("Password", "La contraseña debe tener al menos una letra, un número y siete caracteres.");
                    }
                    else
                    {
                        usuarioViewModel.Password = PasswordHelper.HashPassword(usuarioViewModel.Password); // Encripta la nueva contraseña
                    }
                }

                // Validación del correo electrónico
                if (string.IsNullOrWhiteSpace(usuarioViewModel.Email))
                {
                    ModelState.AddModelError("Email", "El correo electrónico es obligatorio.");
                }
                else
                {
                    var email = usuarioViewModel.Email;

                    try
                    {
                        var mailAddress = new System.Net.Mail.MailAddress(email); // Verifica la validez del correo electrónico
                    }
                    catch
                    {
                        ModelState.AddModelError("Email", "El correo electrónico no es válido.");
                    }
                }

                // Verificar si el correo electrónico ya está en uso
                var usuarioExistente = _usuarioRepository.FindByEmailExcludingId(usuarioViewModel.Email, usuarioViewModel.Id);

                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Email", "El correo electrónico ya está en uso.");
                }

                // Si el ModelState tiene errores, no continuar con la actualización
                if (!ModelState.IsValid)
                {
                    return View(usuarioViewModel); // Devuelve la vista actual con los mensajes de error
                }

                // Actualiza el usuario
                usuarioActual.Email = usuarioViewModel.Email;
                usuarioActual.Nombre = usuarioViewModel.Nombre;
                usuarioActual.Apellido = usuarioViewModel.Apellido;
                usuarioActual.Rol = usuarioViewModel.Rol;
                usuarioActual.Rol = usuarioViewModel.Rol;
                usuarioActual.Password = usuarioViewModel.Password; // Actualiza la contraseña (o mantiene la actual)

                _usuarioRepository.Update(usuarioActual);

                TempData["SuccessMessage"] = "El usuario se ha actualizado correctamente.";
               
                if (usuarioActual.Rol == SharedKernel.Rol.Cliente)
                {
                    
                    return RedirectToAction("Listado", "Producto");
                    
                }
                else
                {
                    return RedirectToAction("Index", "Pedido");
                }
               

            }

            // Si el modelo no es válido, devuelve la vista con los errores
            return View(usuarioViewModel);
        }
        public ActionResult Reactivar(int id)
        {
            Usuario usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            if (usuario.Activo)
            {
                TempData["ErrorMessage"] = "El usuario ya está activo.";
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        [HttpPost, ActionName("Reactivar")]
        [ValidateAntiForgeryToken]
        public ActionResult ReactivarConfirmed(int id)
        {
            Usuario usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            usuario.Activo = true;
            _usuarioRepository.Update(usuario);

            TempData["SuccessMessage"] = "El usuario se ha reactivado correctamente.";
            return RedirectToAction("Index");
        }

    }
}
