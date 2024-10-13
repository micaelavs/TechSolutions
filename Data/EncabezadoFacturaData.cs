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
            return db.EncabezadosFacturas
            .Include(f => f.Usuario)  
            .Include(f => f.DetallesFacturas.Select(d => d.Producto)) 
            .FirstOrDefault(f => f.Id == id);

        }
        public EncabezadoFactura GetByNumero(int numeroFactura)
        {
            var db = new ApiDbContext();
            return db.EncabezadosFacturas
             .Include(e => e.DetallesFacturas.Select(d => d.Producto)) // Incluye el producto relacionado
             .Include(e => e.Usuario)             // Incluye el usuario
             .Include(e => e.Pedido)              // Incluye el pedido
             .FirstOrDefault(e => e.Numero == numeroFactura);
        }

        public IEnumerable<EncabezadoFactura> GetAll()
        {
            var db = new ApiDbContext();
            return db.EncabezadosFacturas
                .Include(f => f.DetallesFacturas.Select(d => d.Producto))  
                .Include(f => f.Usuario)                                   
                .Include(f => f.Pedido)                                    
                .ToList();
        }

        public void Insert(EncabezadoFactura entity)
        {
            var db = new ApiDbContext();
            db.EncabezadosFacturas.Add(entity);
            db.SaveChanges();
        }

        public int InsertReturnId(EncabezadoFactura entity)
        {
            using (var db = new ApiDbContext())
            {
                db.EncabezadosFacturas.Add(entity);
                db.SaveChanges();
                return entity.Id;
            }
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

        public EncabezadoFactura GetFacturaPorPedidoId(int pedidoId)
        {
            using (var db = new ApiDbContext())
            {
                return db.EncabezadosFacturas
                    .Include(f => f.DetallesFacturas.Select(d => d.Producto)) 
                    .Include(f => f.Usuario)                                 
                    .FirstOrDefault(f => f.IdPedido == pedidoId);           
            }
        }
    }
}
