using OrderDispatcher.OrderManagement.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDispatcher.OrderManagement.Entities
{
    public class BasketDetail : EntityBase<int>
    {
        public int BasketMasterId { get; set; }
        public int ProductId { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public int UnitType { get; set; }
        public decimal Weight { get; set; } = 0;
    }
}
