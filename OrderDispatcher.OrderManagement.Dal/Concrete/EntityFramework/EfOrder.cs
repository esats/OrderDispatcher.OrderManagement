using OrderDispatcher.CatalogService.Dal.Concrete.EntityFramework;
using OrderDispatcher.OrderManagement.Core.EntityFramework;
using OrderDispatcher.OrderManagement.Dal.Abstract;
using OrderDispatcher.OrderManagement.Entities;

namespace OrderDispatcher.OrderManagement.Dal.Concrete.EntityFramework
{
    public class EfOrder : EfEntityRepositoryBase<Order, OrderManagementDBContext>, IOrder
    {
    }
}
