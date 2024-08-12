using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechSolutions.Data;

namespace TechSolutions.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsuarioData _usuarioData = new UsuarioData();

        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }
    }
}
