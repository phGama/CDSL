using ContentDeliverySystem.DAL;
using System;
using System.Collections.Generic;
namespace ContentDeliverySystem.SVC
{
    public interface IGroupService:IDisposable
    {
        int Create(string Name);
        bool Delete(int Id);
        Groups Find(int Id);
        IEnumerable<Groups> GetAll();
        void Update(int Id, string Name, bool IsActive);
    }
}
