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
    public class EncabezadoFacturaData : IRepository<EncabezadoFactura>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            EncabezadoFactura factura = db.EncabezadosFacturas.Find(id);
            db.EncabezadosFacturas.Remove(factura);
            db.SaveChanges();

        }

        public EncabezadoFactura GetById(int id)
        {
            var db = new ApiDbContext();
            return db.EncabezadosFacturas.Find(id);

        }

        public void Insert(EncabezadoFactura entity)
        {
            var db = new ApiDbContext();
            db.EncabezadosFacturas.Add(entity);
            db.SaveChanges();
        }

        public IEnumerable<EncabezadoFactura> List()
        {
            var db = new ApiDbContext();
            var encabezadosfacturas = db.EncabezadosFacturas.ToList();
            return encabezadosfacturas;

        }

        public void Update(EncabezadoFactura entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
