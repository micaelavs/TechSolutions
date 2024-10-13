using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechSolutions.Data;

namespace TechSolutions.Controllers
{
    public class HistorialPedidoController : Controller
    {
        private readonly HistorialPedidoData _historialPedidoData;

        public HistorialPedidoController()
        {
            _historialPedidoData = new HistorialPedidoData();
        }
        public ActionResult Index()
        {
            var historialPedidos = _historialPedidoData.List();
            return View(historialPedidos);
        }

       /*public ActionResult Details(int id)
       {
            var historialPedido = _historialPedidoData.GetById(id);
            if (historialPedido == null)
            {
                return HttpNotFound();
            }
            return View(historialPedido);
       }*/

        // GET: HistorialPedido2/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HistorialPedido2/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: HistorialPedido2/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HistorialPedido2/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: HistorialPedido2/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HistorialPedido2/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
