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
    public class CategoriaProductoData : IRepository<CategoriaProducto>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            CategoriaProducto categoria = db.Categorias.Find(id);
            db.Categorias.Remove(categoria);
            db.SaveChanges();

        }

        public CategoriaProducto GetById(int id)
        {
            var db = new ApiDbContext();
            return db.Categorias.Find(id);

        }

        public void Insert(CategoriaProducto entity)
        {
            var db = new ApiDbContext();
            db.Categorias.Add(entity);
            db.SaveChanges();
        }

        //trae solo los activos
        public IEnumerable<CategoriaProducto> List()
        {
            var db = new ApiDbContext();
            var categorias = db.Categorias
                          .Where(c => c.Activo)
                          .ToList();
            return categorias;

        }

        public void Update(CategoriaProducto entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
