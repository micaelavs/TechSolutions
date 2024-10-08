using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TechSolutions.Interfaces;
using TechSolutions.Models;

namespace TechSolutions.Data
{
    public class DetalleNotaCreditoData : IRepository<DetalleNotaCredito>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            DetalleNotaCredito detalleNotaCredito = db.DetallesNotasCreditos.Find(id);
            db.DetallesNotasCreditos.Remove(detalleNotaCredito);
            db.SaveChanges();

        }

        public DetalleNotaCredito GetById(int id)
        {
            var db = new ApiDbContext();
            return db.DetallesNotasCreditos.Find(id);

        }

        public void Insert(DetalleNotaCredito entity)
        {
            var db = new ApiDbContext();
            db.DetallesNotasCreditos.Add(entity);
            db.SaveChanges();
        }
        public int InsertReturnId(DetalleNotaCredito entity)
        {
            using (var db = new ApiDbContext())
            {
                db.DetallesNotasCreditos.Add(entity);
                db.SaveChanges();
                return entity.Id;
            }
        }

        public IEnumerable<DetalleNotaCredito> List()
        {
            var db = new ApiDbContext();
            var detallesNotasCreditos = db.DetallesNotasCreditos.ToList();
            return detallesNotasCreditos;


        }

        public void Update(DetalleNotaCredito entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}