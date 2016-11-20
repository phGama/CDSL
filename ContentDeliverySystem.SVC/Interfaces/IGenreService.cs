using ContentDeliverySystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC
{
    public interface IGenreService:IDisposable
    {
        int Create(string name, int? idParent = null);
        void Update(int id, Genres genre);
        bool Delete(Genres genre);
        IEnumerable<Genres> GetAll();
        Genres Find(int id);
    }
}
