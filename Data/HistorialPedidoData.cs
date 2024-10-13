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
    public class HistorialPedidoData : IRepository<HistorialPedido>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            HistorialPedido historialPedido = db.HistorialPedidos.Find(id);
            db.HistorialPedidos.Remove(historialPedido);
            db.SaveChanges();

        }

        public HistorialPedido GetById(int id)
        {
            var db = new ApiDbContext();
            return db.HistorialPedidos
                 .Include(h => h.Pedido) 
                 .SingleOrDefault(h => h.Id == id);
        }

        public void Insert(HistorialPedido entity)
        {
            var db = new ApiDbContext();
            db.HistorialPedidos.Add(entity);
            db.SaveChanges();
        }
        public int InsertReturnId(HistorialPedido entity)
        {
            using (var db = new ApiDbContext())
            {
                db.HistorialPedidos.Add(entity);
                db.SaveChanges();
                return entity.Id;
            }
        }

        public IEnumerable<HistorialPedido> List()
        {
            var db = new ApiDbContext();
            var historialPedidos = db.HistorialPedidos
            .Include(h => h.Pedido)  // Incluye la entidad Pedido
            .ToList();

            return historialPedidos;


        }

        public void Update(HistorialPedido entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
