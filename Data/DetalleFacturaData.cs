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
    public class DetalleFacturaData : IRepository<DetalleFactura>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            DetalleFactura detallefactura = db.DetallesFacturas.Find(id);
            db.DetallesFacturas.Remove(detallefactura);
            db.SaveChanges();

        }

        public DetalleFactura GetById(int id)
        {
            var db = new ApiDbContext();
            return db.DetallesFacturas.Find(id);

        }

        public void Insert(DetalleFactura entity)
        {
            var db = new ApiDbContext();
            db.DetallesFacturas.Add(entity);
            db.SaveChanges();
        }

        public IEnumerable<DetalleFactura> List()
        {
            var db = new ApiDbContext();
            var detallesfacturas = db.DetallesFacturas.ToList();
            return detallesfacturas;

        }

        public void Update(DetalleFactura entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
