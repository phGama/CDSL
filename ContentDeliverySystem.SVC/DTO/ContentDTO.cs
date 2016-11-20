using ContentDeliverySystem.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentDeliverySystem.SVC.DTO
{
    public class ContentDTO
    {
        public ContentDTO(Contents Entity)
        {
            foreach (var propertyInfo in this.GetType().GetProperties())
            {
                var EntityValue = Entity.GetType().GetProperty(propertyInfo.Name).GetValue(Entity, null);
                propertyInfo.SetValue(this, EntityValue, null);
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public System.DateTime BeginDeliveryDate { get; set; }
        public System.DateTime EndDeliveryDate { get; set; }
        public bool IsBroadcast { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}
