using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TechSolutions.Interfaces;
using TechSolutions.Models;

namespace TechSolutions.Data
{
    public class NotaDeCreditoData : IRepository<NotaDeCredito>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            NotaDeCredito NotadeCredito = db.NotasCreditos.Find(id);
            db.NotasCreditos.Remove(NotadeCredito);
            db.SaveChanges();

        }

        public NotaDeCredito GetById(int id)
        {
            var db = new ApiDbContext();
            return db.NotasCreditos.Find(id);

        }

        public void Insert(NotaDeCredito entity)
        {
            var db = new ApiDbContext();
            db.NotasCreditos.Add(entity);
            db.SaveChanges();
        }
        public int InsertReturnId(NotaDeCredito entity)
        {
            using (var db = new ApiDbContext())
            {
                db.NotasCreditos.Add(entity);
                db.SaveChanges();
                return entity.Id;
            }
        }

        public IEnumerable<NotaDeCredito> List()
        {
            var db = new ApiDbContext();
            var notasDeCreditos = db.NotasCreditos
                .Include(n => n.DetallesNotasCreditos.Select(d => d.Producto))
                .Include(n => n.SolicitudDevolucion) 
                .Include(n => n.EncabezadoFactura)
                .ToList();
            return notasDeCreditos;


        }

        public void Update(NotaDeCredito entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        public NotaDeCredito GetByNumero(int numeroNotaCredito)
        {
            var db = new ApiDbContext();
            return db.NotasCreditos
                .Include(n => n.DetallesNotasCreditos.Select(d => d.Producto))
                .Include(n => n.SolicitudDevolucion)
                .Include(n => n.EncabezadoFactura)
                .FirstOrDefault(n => n.Numero == numeroNotaCredito);
            
        }

        public NotaDeCredito GetByFacturaId(int facturaId)
        {
            using (var db = new ApiDbContext())
            {
                return db.NotasCreditos
                    .Include(n => n.DetallesNotasCreditos.Select(d => d.Producto))
                    .Include(n => n.SolicitudDevolucion)
                    .Include(n => n.EncabezadoFactura)
                    .FirstOrDefault(n => n.IdFactura == facturaId);
            }
        }
    }
}