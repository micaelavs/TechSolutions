using Newtonsoft.Json.Linq;
using PagedList;
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
    public class SolicitudDevolucionController : Controller
    {
        private readonly SolicitudDevolucionData _solicitudDevolucionRepository;

        public SolicitudDevolucionController()
        {
            _solicitudDevolucionRepository = new SolicitudDevolucionData();

        }
            
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var pedidos = _solicitudDevolucionRepository.List().ToList();
            var pagedList = pedidos.ToPagedList(page, pageSize);
            return View(pagedList);
        }

        public ActionResult Details(int id)
        {
            SolicitudDevolucion solicitudDevolucion = _solicitudDevolucionRepository.GetById(id);
            if (solicitudDevolucion == null)
            {
                return HttpNotFound();
            }
            return View(solicitudDevolucion);
        }

        // GET: SolicitudDevolucion/Edit/5
        public ActionResult Edit(int id)
        {
            var solicitud = _solicitudDevolucionRepository.GetById(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }

            ViewBag.Estados = Enum.GetValues(typeof(TechSolutions.SharedKernel.EstadoSolicitud))
              .Cast<TechSolutions.SharedKernel.EstadoSolicitud>()
              .Select(e => new SelectListItem
              {
                  Value = ((int)e).ToString(),
                  Text = e.ToString()
              }).ToList();

            return View(solicitud);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SolicitudDevolucion model)
        {
            
            // Obtener la solicitud existente
            var solicitudExistente = _solicitudDevolucionRepository.GetById(model.Id);
            if (solicitudExistente == null)
            {
                return HttpNotFound();
            }
            solicitudExistente.EstadoSolicitud = model.EstadoSolicitud;

                
            _solicitudDevolucionRepository.Update(solicitudExistente);
            TempData["SuccessMessage"] = "El estado de la solicitud se actualizó correctamente!";
            return RedirectToAction("Index"); 
            

           
        }



    }
}
