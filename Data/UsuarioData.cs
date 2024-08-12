using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TechSolutions.Interfaces;
using TechSolutions.Models;

namespace TechSolutions.Data
{
    public class UsuarioData : IRepository<Usuario>
    {
        public void Delete(int id)
        {
            var db = new ApiDbContext();
            Usuario usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();

        }

        public Usuario GetById(int id)
        {
            var db = new ApiDbContext();
            return db.Usuarios.Find(id);

        }

        public void Insert(Usuario entity)
        {
            var db = new ApiDbContext();
            db.Usuarios.Add(entity);
            db.SaveChanges();
        }

        public IEnumerable<Usuario> List()
        {

            var db = new ApiDbContext();
            var usuarios = db.Usuarios.ToList();
            return usuarios;


        }

        public void Update(Usuario entity)
        {
            var db = new ApiDbContext();
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
