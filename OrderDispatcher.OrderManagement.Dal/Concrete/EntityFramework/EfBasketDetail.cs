using OrderDispatcher.OrderManagement.Core.EntityFramework;
using OrderDispatcher.OrderManagement.Dal.Abstract;
using OrderDispatcher.OrderManagement.Entities;

namespace OrderDispatcher.OrderManagement.Dal.Concrete.EntityFramework
{
    public class EfBasketDetail : EfEntityRepositoryBase<BasketDetail, OrderManagementDBContext>, IBasketDetail
    {
        public EfBasketDetail(OrderManagementDBContext context) : base(context) { }
    }
}
