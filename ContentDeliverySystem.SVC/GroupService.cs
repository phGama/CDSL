using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC
{
    class GroupService : IDisposable, IGroupService
    {
        private CDSEntities CdDb;
        private IAuthService AuthService;
        private AuthResponse AuthResult;

        internal GroupService(CDSEntities DbEntities, IAuthService AuthService, string Token, TokenType tokenType)
        {
            CdDb = DbEntities;
            this.AuthService = AuthService;
            this.AuthResult = AuthService.GetAuthResponse(Token, tokenType);
        }

        public Groups Find(int Id)
        {
            //VerifyToken();
            return CdDb.Groups.SingleOrDefault(x => x.Id == Id);
        }

        public IEnumerable<Groups> GetAll()
        {
            //VerifyToken();
            var Groups = CdDb.Groups.ToList();
            return Groups;
        }



        public int Create(string Name)
        {
            VerifyToken();
            if (String.IsNullOrEmpty(Name))
                throw new ArgumentNullException("name");
            if (Name.Length > 50)
                throw new ArgumentException("max size is 50", "Name");
            Name = Name.ToLower();
            var Group = CdDb.Groups.SingleOrDefault(x => x.Name == Name);
            if (Group != null)
                throw new ArgumentException("There is already a group with this name");
            Group = new Groups()
            {
                Name = Name,
                Active = true,
                CreatedAt = DateTime.Now
            };
            CdDb.Groups.Add(Group);
            CdDb.SaveChanges();
            return Group.Id;

        }

        public void Update(int Id, string Name, bool IsActive = false)
        {
            VerifyToken();
            Name = Name.ToLower();
            var Group = CdDb.Groups.SingleOrDefault(x => x.Id == Id);
            if (CdDb.Groups.Any(x => x.Name == Name && x.Id != Id))
                throw new ArgumentException("There is already a group with this name");

            Group.Name = Name;
            Group.Active = IsActive;
            CdDb.SaveChanges();
        }

        public bool Delete(int Id)
        {
            VerifyToken();
            if (CdDb.GroupContents.Any(x => x.IdGroup == Id) || CdDb.Users.Any(x => x.IdGroup == Id))
                throw new InvalidOperationException("Can't Delete a Group with materials or users associated");
            CdDb.Groups.Remove(CdDb.Groups.SingleOrDefault(x => x.Id == Id));
            CdDb.SaveChanges();

            return true;
        }


        public void Dispose()
        {
            if (CdDb != null)
                CdDb.Dispose();
            if (AuthService != null)
                AuthService.Dispose();
        }

        private void VerifyToken()
        {
            if (!AuthResult.IsSuccess && !AuthResult.IsAdmin)
                throw new InvalidTokenException();
        }
    }
}
