using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using System;
namespace ContentDeliverySystem.SVC
{
    public interface IUserService:IDisposable
    {
        int Create(ContentDeliverySystem.DAL.Users User, string Password, string ActivateUrl);
        bool Delete(int Id);
        bool Exists(string Cpf);
        System.Collections.Generic.IEnumerable<UserDTO> GetUsers();
        void Update(ContentDeliverySystem.DAL.Users User);
        Users Find(int Id);
        void ResendActiveEmail(string Email, string ActiveteUrl);

        void ActivateUser(int Id);
        bool Validate(Users User);
    }
}
