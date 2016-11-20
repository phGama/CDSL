using ContentDeliverySystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC.DTO
{
    public class UserDTO
    {
        public UserDTO(Users UserEntity){
           
            foreach (var propertyInfo in typeof(UserDTO).GetProperties())
            {
                var EntityValue = typeof(Users).GetProperty(propertyInfo.Name).GetValue(UserEntity,null);
                propertyInfo.SetValue(this, EntityValue, null);
            }
        }
        public int Id { get; set; }
        public Nullable<int> IdGroup { get; set; }
        public Nullable<int> IdType { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public Nullable<System.DateTime> BirthDate { get; set; }
        public Nullable<byte> Gender { get; set; }
        public string Phone { get; set; }
        public string Cellphone { get; set; }
        public string Adress { get; set; }
        public string CEP { get; set; }
        public string State { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public bool Active { get; set; }
    }
}
