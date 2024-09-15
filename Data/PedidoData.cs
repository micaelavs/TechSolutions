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
    public class PedidoData : IRepository<Pedido>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            Pedido pedido = db.Pedidos.Find(id);
            db.Pedidos.Remove(pedido);
            db.SaveChanges();

        }

        public Pedido GetById(int id)
        {
            var db = new ApiDbContext();
            return db.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.DetallesPedidos.Select(d => d.Producto)) // Incluye Producto
                .Include(p => p.HistorialPedidos)
                .FirstOrDefault(p => p.Id == id);

        }

        public void Insert(Pedido entity)
        {
            var db = new ApiDbContext();
            db.Pedidos.Add(entity);
            db.SaveChanges();
        }

        public int InsertReturnId(Pedido entity)
        {
            using (var db = new ApiDbContext())
            {
                db.Pedidos.Add(entity);
                db.SaveChanges(); 
                return entity.Id;
            }
        }

        public IEnumerable<Pedido> List()
        {
            var db = new ApiDbContext();
            var pedidos = db.Pedidos
                   .Include(p => p.Usuario)
                   .Include(p => p.DetallesPedidos) 
                   .Include(p => p.HistorialPedidos)
                   .ToList();
            return pedidos;


        }

        public void Update(Pedido entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
