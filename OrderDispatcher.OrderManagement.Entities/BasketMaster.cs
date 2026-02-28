using OrderDispatcher.OrderManagement.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderDispatcher.CatalogService.Entities
{
    public class BasketMaster : EntityBase<int> 
    {
        public string UserId { get; set; }
        public string StoreId { get; set; }
        public int DeliveryAddressId { get; set; }
        public int OrderId { get; set; } = 0;
        //public decimal SubTotal { get; set; } = 0;
        //public decimal GrandTotal { get; set; } = 0;
        //public DateTime LastActivityAt { get; set; }
    }
}
