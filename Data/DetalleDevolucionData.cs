using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechSolutions.Interfaces;
using TechSolutions.Models;

namespace TechSolutions.Data
{
    public class DetalleDevolucionData : IRepository<DetalleDevolucion>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            DetalleDevolucion detalleDev = db.DetallesDevoluciones.Find(id);
            db.DetallesDevoluciones.Remove(detalleDev);
            db.SaveChanges();

        }

        public DetalleDevolucion GetById(int id)
        {
            var db = new ApiDbContext();
            return db.DetallesDevoluciones.Find(id);

        }

        public void Insert(DetalleDevolucion entity)
        {
            var db = new ApiDbContext();
            db.DetallesDevoluciones.Add(entity);
            db.SaveChanges();
        }

        public IEnumerable<DetalleDevolucion> List()
        {
            var db = new ApiDbContext();
            var detalleDev = db.DetallesDevoluciones.ToList();
            return detalleDev;


        }

        public void Update(DetalleDevolucion entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
