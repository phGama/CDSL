using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC
{
    public class GenreService : IGenreService
    {

        CDSEntities cdDb;
        System.Data.Entity.DbSet<Genres> genres;
        IAuthService authService;

        internal GenreService(CDSEntities cdDb, IAuthService authService, string tokenCode, TokenType type)
        {
            this.authService = authService;
            var authResult = authService.GetAuthResponse(tokenCode, type);

            if (!authResult.IsSuccess || !authResult.IsAdmin)
                throw new InvalidTokenException();

            this.cdDb = cdDb;
            this.genres = cdDb.Genres;
        }

        public int Create(string name, int? idParent = null)
        {
            var newEntity = new Genres()
            {
                IdParent = idParent,
                Name = name
            };
            genres.Add(newEntity);
            Save();
            return newEntity.Id;
        }

        public void Update(int id, Genres genre)
        {
            var entity = Find(id);
            if (entity == null) return;

            entity.Name = genre.Name;
            entity.IdParent = genre.IdParent;

            Save();

        }

        public bool Delete(DAL.Genres genre)
        {
            try
            {
                var entity = Find(genre.Id);
                genres.Remove(entity);
                Save();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Genres> GetAll()
        {
            return cdDb.Genres.ToList();
        }

        public DAL.Genres Find(int id)
        {
            return genres.FirstOrDefault(x => x.Id == id);
        }

        private void Save()
        {
            cdDb.SaveChanges();
        }

        public void Dispose()
        {
            if (cdDb != null)
                cdDb.Dispose();
            if (authService != null)
                authService.Dispose();
        }
    }
}
