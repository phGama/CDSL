using ContentDeliverySystem.DAL;
using ContentDeliverySystem.SVC.DTO;
using ContentDeliverySystem.SVC.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ContentDeliverySystem.SVC
{
    public class UserService : IDisposable, IUserService
    {

        CDSEntities CdDb;
        IAuthServiceInternal AuthService;
        BatchMail SmtpService;
        private AuthResponse AuthResult;
        internal UserService(CDSEntities CdDb, AuthResponse AuthResult, BatchMail SmtpHelper, IAuthServiceInternal AuthService)
        {
            this.CdDb = CdDb;
            this.AuthService = AuthService;
            this.SmtpService = SmtpHelper;
            this.AuthResult = AuthResult;
        }

        public IEnumerable<UserDTO> GetUsers()
        {
            VerifyToken();
            var ResultList = new List<UserDTO>();
            var EntitiesList = CdDb.Users.ToList();
            foreach (var Item in EntitiesList)
            {
                ResultList.Add(new UserDTO(Item));
            }
            return ResultList;
        }

        public void Update(Users User)
        {
            VerifyToken();
            var DbUser = CdDb.Users.SingleOrDefault(x => x.Id == User.Id);
            var Group = CdDb.Groups.SingleOrDefault(x => x.Id == User.IdGroup);

            if (DbUser == null)
                throw new ArgumentException("Can't find specified user");
            if (Group == null)
                throw new ArgumentException("Invalid Id group");
            if (!Group.Active)
                throw new ArgumentException("Id must be of a active group");


            //if (!Validate(User))
            //    throw new ArgumentException("Invalid User");
            DbUser.Name = User.Name;
            DbUser.Adress = User.Adress;
            DbUser.BirthDate = User.BirthDate;
            DbUser.Phone = User.Phone;
            DbUser.State = User.State;
            DbUser.CEP = User.CEP;
            DbUser.Cellphone = User.Cellphone;
            DbUser.Gender = User.Gender;
            DbUser.IdGroup = User.IdGroup;

            CdDb.SaveChanges();
        }

        public int Create(Users User, string Password, string ActivateUrl)
        {
            //VerifyToken();

            if (!Validate(User))
                return 0;
            User.IdType = 2;
            using (var CdDb = new CDSEntities())
            {
                var Group = CdDb.Groups.SingleOrDefault(x => x.Id == User.IdGroup);

                if (Group == null)
                    throw new ArgumentException("Invalid Id group");
                if (!Group.Active)
                    throw new ArgumentException("Id must be of a active group");

                User.Password = AuthService.CreateEncryptedPassword(Password);
                User.Active = true;
                User.CreatedAt = DateTime.Now;
                CdDb.Users.Add(User);
                CdDb.SaveChanges();

                try
                {
                    SendActiveAccountEmail(User.Name, User.Email, User.Id, ActivateUrl);
                }
                catch { }

                return User.Id;
            }


        }

        public bool Validate(Users User)
        {
            if (!CPFValidator.Validate(User.CPF))
                return false;
            if (!IsValidEmail(User.Email))
                return false;
            if (CdDb.Users.Any(x => x.Email == User.Email || x.CPF == User.CPF))
                return false;
            return true;
        }

        public Users Find(int Id)
        {
            VerifyToken();
            return CdDb.Users.SingleOrDefault(x => x.Id == Id);
        }

        public Users Find(string Email)
        {
            VerifyToken();
            return CdDb.Users.SingleOrDefault(x => x.Email == Email);
        }

        public Users Find(Func<Users,bool> Filter)
        {
            return CdDb.Users.SingleOrDefault(Filter);
        }

        public bool Exists(string Cpf)
        {
            return CdDb.Users.Any(x => x.CPF == Cpf);
        }

        public void ResendActiveEmail(string Email, string ActiveteUrl)
        {
            VerifyToken();
            var User = Find(Email);
            if (User.Active)
                throw new Exception("This user is already activated");
            SendActiveAccountEmail(User.Name, Email, User.Id, ActiveteUrl);

        }

        public void ActivateUser(int Id)
        {
            VerifyToken();
            var User = Find(Id);
            if (User.Active)
                throw new Exception("This user is already activated");

            User.Active = true;
            CdDb.SaveChanges();
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var mail = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int Id)
        {

            CdDb.Tokens.RemoveRange(CdDb.Tokens.Where(x => x.IdUser == Id));
            CdDb.SaveChanges();
            CdDb.Users.Remove(CdDb.Users.SingleOrDefault(x => x.Id == Id));
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

        private void SendActiveAccountEmail(string Name, string UserEmail, int UserId, string ActivationEntPoint)
        {
            var EmailHtml = "Bom dia {0}<br/>Por favor clique <a href='{1}'>aqui</a> para ativar a sua conta no sistema de Distribuição de arquivos.<br/><br/>Caso não tenha conhecimento do assunto por favor desconsidere esse email";

            var ActivationUrl = ActivationEntPoint + "?ticket=" + UserId;

            SmtpService.EnableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["smtpSSL"]);
            SmtpService.From = ConfigurationManager.AppSettings["smtpHost"];
            SmtpService.isHtml = true;
            SmtpService.Message = string.Format(EmailHtml, Name, ActivationUrl);
            SmtpService.SmtpAdress = ConfigurationManager.AppSettings["smtpEmail"];
            SmtpService.SmtpMailUser = ConfigurationManager.AppSettings["smtpEmail"];
            SmtpService.SmtpPassword = ConfigurationManager.AppSettings["smtpPassword"];
            SmtpService.Subject = "Ativação de conta CDS";
            SmtpService.To = UserEmail;

            SmtpService.Send();

        }

        private void VerifyToken()
        {
            if (!AuthResult.IsAdmin)
                throw new InvalidTokenException();
        }

    }
}
