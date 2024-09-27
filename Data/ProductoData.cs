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
    public class ProductoData: IRepository<Producto>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            Producto producto = db.Productos.Find(id);
            db.Productos.Remove(producto);
            db.SaveChanges();

        }

        public Producto GetById(int id)
        {
            var db = new ApiDbContext();
            /*return db.Productos
                 .Include(p => p.CategoriaProducto)
                 .Include(p => p.Calificaciones)
                 .SingleOrDefault(p => p.Id == id);*/

            return db.Productos
          .Include(p => p.CategoriaProducto)
          .Include(p => p.Calificaciones.Select(c => c.Usuario)) // Incluye el usuario de cada calificación
          .SingleOrDefault(p => p.Id == id);

        }

        public IEnumerable<Producto> GetByIds(List<int> ids)
        {
            var db = new ApiDbContext();
            return db.Productos
                     .Include(p => p.CategoriaProducto)
                     .Include(p => p.Calificaciones)
                     .Where(p => ids.Contains(p.Id) && p.Activo)
                     .ToList();
        }

        public void Insert(Producto entity)
        {
            var db = new ApiDbContext();
            db.Productos.Add(entity);
            db.SaveChanges();
        }

        //trae solo los activos
        public IEnumerable<Producto> List()
        {
            var db = new ApiDbContext();
            var productos = db.Productos
                          .Include(p => p.CategoriaProducto) 
                          .Include(p => p.Calificaciones) 
                          .Where(p => p.Activo) 
                          .ToList();

            return productos;

        }

        public IEnumerable<Producto> ListProductosDisponibles()
        {
            using (var db = new ApiDbContext())
            {
                var productos = db.Productos
                                  .Include(p => p.CategoriaProducto)
                                  .Include(p => p.Calificaciones) 
                                  .Where(p => p.Activo && p.Stock > 0) 
                                  .ToList();

                return productos;
            }
        }

        public void Update(Producto entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
