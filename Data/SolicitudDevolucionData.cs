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
    public class SolicitudDevolucionData : IRepository<SolicitudDevolucion>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            SolicitudDevolucion solicitudDev = db.SolicitudesDevoluciones.Find(id);
            db.SolicitudesDevoluciones.Remove(solicitudDev);
            db.SaveChanges();

        }

        public SolicitudDevolucion GetById(int id)
        {
            var db = new ApiDbContext();
            return db.SolicitudesDevoluciones.Find(id);

        }

        public void Insert(SolicitudDevolucion entity)
        {
            var db = new ApiDbContext();
            db.SolicitudesDevoluciones.Add(entity);
            db.SaveChanges();
        }

        public int InsertReturnId(SolicitudDevolucion entity)
        {
            using (var db = new ApiDbContext())
            {
                db.SolicitudesDevoluciones.Add(entity);
                db.SaveChanges();
                return entity.Id; // Asumiendo que la propiedad del ID se llama "Id"
            }
        }

        public IEnumerable<SolicitudDevolucion> List()
        {
            var db = new ApiDbContext();
            var solicitudesdevolucion = db.SolicitudesDevoluciones.ToList();
            return solicitudesdevolucion;


        }

        public void Update(SolicitudDevolucion entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
