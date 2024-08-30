using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Helpers;
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
        public Usuario FindByEmail(string Email)
        {
            var db = new ApiDbContext();
            return db.Usuarios.FirstOrDefault(u => u.Email == Email);
        }
     

        public void Insert(Usuario entity)
        {
            var db = new ApiDbContext();
            db.Usuarios.Add(entity);
            db.SaveChanges();
        }

        public bool InsertUsuario(Usuario entity)
        {
            try
            {
                using (var db = new ApiDbContext())
                {
                    db.Usuarios.Add(entity);
                    int result = db.SaveChanges();
                    // Retornar true si se afectó al menos una fila, de lo contrario false.
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción según tus necesidades.
                // Por ejemplo, podrías registrar el error o enviar una alerta.
                return false;
            }
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
