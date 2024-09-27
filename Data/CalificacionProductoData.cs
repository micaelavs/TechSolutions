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
    public class CalificacionProductoData : IRepository<CalificacionProducto>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            CalificacionProducto calificacion = db.CalificacionesProductos.Find(id);
            db.CalificacionesProductos.Remove(calificacion);
            db.SaveChanges();

        }

        public CalificacionProducto GetById(int id)
        {
            var db = new ApiDbContext();
            return db.CalificacionesProductos.Find(id);

        }
       
        public void Insert(CalificacionProducto entity)
        {
            var db = new ApiDbContext();
            db.CalificacionesProductos.Add(entity);
            db.SaveChanges();
        }

        public IEnumerable<CalificacionProducto> List()
        {

            var db = new ApiDbContext();
            var calificaciones = db.CalificacionesProductos.ToList();
            return calificaciones;


        }

        public void Update(CalificacionProducto entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        //dado un usuario traer todas sus calificaciones
        public IEnumerable<CalificacionProducto> GetCalificacionesByUsuario(int idUsuario)
        {
            var db = new ApiDbContext();

            // Obtener todas las calificaciones del usuario
            var calificaciones = db.CalificacionesProductos
                .Include(c => c.Producto) 
                .Where(c => c.IdUsuario == idUsuario)
                .ToList();

            return calificaciones;
        }

    }
}
