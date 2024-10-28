using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechSolutions.Data;
using TechSolutions.Interfaces;
using TechSolutions.Models;
using PagedList;

namespace TechSolutions.Controllers
{
    public class SolictudDevolucionController : Controller
    {
        private readonly SolicitudDevolucionData _solicitudDevolucionRepository;
        
        public SolictudDevolucionController()
        {
            _solicitudDevolucionRepository = new SolicitudDevolucionData();
        }
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var solicitudes = _solicitudDevolucionRepository.List().ToList();
            var pagedList = solicitudes.ToPagedList(page, pageSize);
            return View(pagedList);
        }


    }
}
