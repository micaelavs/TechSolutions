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
    public class DetallePedidoData : IRepository<DetallePedido>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            DetallePedido detallepedido = db.DetallesPedidos.Find(id);
            db.DetallesPedidos.Remove(detallepedido);
            db.SaveChanges();

        }

        public DetallePedido GetById(int id)
        {
            var db = new ApiDbContext();
            return db.DetallesPedidos.Find(id);

        }

        public void Insert(DetallePedido entity)
        {
            var db = new ApiDbContext();
            db.DetallesPedidos.Add(entity);
            db.SaveChanges();
        }

        public int InsertReturnId(DetallePedido entity)
        {
            using (var db = new ApiDbContext())
            {
                db.DetallesPedidos.Add(entity);
                db.SaveChanges();
                return entity.Id;
            }
        }

        public IEnumerable<DetallePedido> List()
        {
            var db = new ApiDbContext();
            var detallespedidos = db.DetallesPedidos.ToList();
            return detallespedidos;

        }

        public void Update(DetallePedido entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        public IEnumerable<DetallePedido> GetDetallesByPedidoId(int idPedido)
        {
            using (var db = new ApiDbContext())
            {
                return db.DetallesPedidos
                    .Include(d => d.Producto) 
                    .Where(d => d.IdPedido == idPedido)
                    .ToList();
            }
        }
    }
}
