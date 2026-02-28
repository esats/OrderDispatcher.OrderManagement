using Microsoft.AspNetCore.Mvc;
using OrderDispatcher.OrderManagement.API.Base;
using OrderDispatcher.OrderManagement.API.Models;
using OrderDispatcher.OrderManagement.API.Infrastructure;
using OrderDispatcher.OrderManagement.Dal.Abstract;
using OrderDispatcher.OrderManagement.Entities;

namespace OrderDispatcher.OrderManagement.API.Controllers
{
    [Route("api/order-management/order")]
    [Produces("application/json")]
    public class OrderController : APIControllerBase
    {
        private readonly IBasketMaster _basketMaster;
        private readonly IOrder _order;
        private readonly OrderMessagePublisher _publisher;

        public OrderController(IOrder order, IBasketMaster basketMaster, OrderMessagePublisher publisher)
        {
            _order = order;
            _basketMaster = basketMaster;
            _publisher = publisher;
        }

        [HttpPost("save")]
        public async Task<Response<OrderSaveResponse>> Save([FromBody] OrderModel model)
        {
            if (model == null)
            {
                return new Response<OrderSaveResponse>(false, "Request body is required.", new OrderSaveResponse());
            }

            Order order = new Order
            {
                CustomerId = model.CustomerId,
                StoreId = model.StoreId,
                ShopperId = model.ShopperId,
                BasketMasterId = model.BasketMasterId,
                AssignedAtUtc = model.AssignedAtUtc,
                Status = (int)OrderStatus.Created,
                Subtotal = model.Subtotal,
                DeliveryFee = model.DeliveryFee,
                ServiceFee = model.ServiceFee,
                Tip = model.Tip,
                Total = model.Total,
                Notes = model.Notes,
                CreatedBy = base.GetUser()
            };

            order = await _order.AddAsync(order);

            var publishOk = _publisher.PublishOrderId(order.Id, "order.created", out var publishError);

            if (!publishOk)
            {
                return new Response<OrderSaveResponse>(
                    false,
                    $"Order saved but publish failed: {publishError}",
                    new OrderSaveResponse
                    {
                        OrderId = order.Id,
                    });
            }

            return new Response<OrderSaveResponse>(new OrderSaveResponse
            {
                OrderId = order.Id,
            });
        }

        [HttpGet("getOne")]
        public async Task<OrderGetOneResponse> GetOne([FromQuery] long Id)
        {
            var order = await _order.GetTAsync(x => x.Id == Id);
            if (order == null)
            {
                return null;
            }

            return new OrderGetOneResponse
            {
                Order = new OrderModel
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    StoreId = order.StoreId,
                    ShopperId = order.ShopperId,
                    AssignedAtUtc = order.AssignedAtUtc,
                    Status = (byte)order.Status,
                    Subtotal = order.Subtotal,
                    DeliveryFee = order.DeliveryFee,
                    ServiceFee = order.ServiceFee,
                    Tip = order.Tip,
                    Total = order.Total,
                    Notes = order.Notes
                }
            };
        }

        [HttpGet("getAllUserOrders")]
        public async Task<OrderGetAllResponse> GetAllUserOrders([FromQuery] string customerId)
        {
            var orders = await _order.GetListAsync(x => x.CustomerId == customerId);

            var list = new List<OrderModel>();
            foreach (var order in orders)
            {
                list.Add(new OrderModel
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    StoreId = order.StoreId,
                    ShopperId = order.ShopperId,
                    AssignedAtUtc = order.AssignedAtUtc,
                    Status = (byte)order.Status,
                    Subtotal = order.Subtotal,
                    DeliveryFee = order.DeliveryFee,
                    ServiceFee = order.ServiceFee,
                    Tip = order.Tip,
                    Total = order.Total,
                    Notes = order.Notes
                });
            }

            return new OrderGetAllResponse
            {
                CustomerId = customerId,
                Orders = list
            };
        }

        [HttpGet("getAll")]
        public async Task<List<OrderModel>> GetAll()
        {
            var orders = await _order.GetListAsync(x => x.ShopperId == null);

            var list = new List<OrderModel>();
            foreach (var order in orders)
            {
                list.Add(new OrderModel
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    StoreId = order.StoreId,
                    BasketMasterId = order.BasketMasterId,
                    ShopperId = order.ShopperId,
                    AssignedAtUtc = order.AssignedAtUtc,
                    Status = (byte)order.Status,
                    Subtotal = order.Subtotal,
                    DeliveryFee = order.DeliveryFee,
                    ServiceFee = order.ServiceFee,
                    Tip = order.Tip,
                    Total = order.Total,
                    Notes = order.Notes
                });
            }

            return list;
        }

        [HttpPost("assignToShopper")]
        public async Task<Response<OrderSaveResponse>> AssignToShopper([FromBody] OrderAssignModel model)
        {
            var order = await _order.GetTAsync(x => x.Id == model.OrderId);
            order.ShopperId = model.ShopperId;
            order.Status = (int)OrderStatus.Assigned;

            _order.Update(order);

            var publishOk = _publisher.PublishOrderId(order.Id, "order.assigned", out var publishError);

            if (!publishOk)
            {
                return new Response<OrderSaveResponse>(
                    false,
                    $"Order assigned but publish failed: {publishError}",
                    new OrderSaveResponse
                    {
                        OrderId = model.OrderId,
                    });
            }

            return new Response<OrderSaveResponse>(new OrderSaveResponse
            {
                OrderId = order.Id,
            });
        }

        [HttpGet("getActiveOrder")]
        public async Task<OrderModel> GetActiveOrder([FromQuery] string userId)
        {
            var order = await _order.GetTAsync(x => x.ShopperId == userId && x.IsFinished == false);
            if (order == null)
            {
                return null;
            }

            return
                 new OrderModel
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    StoreId = order.StoreId,
                    ShopperId = order.ShopperId,
                    AssignedAtUtc = order.AssignedAtUtc,
                    Status = (byte)order.Status,
                    Subtotal = order.Subtotal,
                    DeliveryFee = order.DeliveryFee,
                    ServiceFee = order.ServiceFee,
                    Tip = order.Tip,
                    Total = order.Total,
                    Notes = order.Notes
            };
        }

        [HttpPost("finishOrder")]
        public async Task<Response<OrderModel>> FinishOrder([FromBody] long orderId)
        {
            var order = await _order.GetTAsync(x => x.Id == orderId);
            order.IsFinished = true;
            order.Status = (int)OrderStatus.Completed;

            _order.Update(order);

            var basketMaster = await _basketMaster.GetTAsync(x=> x.Id == order.BasketMasterId);
            basketMaster.IsActive = false;
            _basketMaster.Update(basketMaster);

            return new Response<OrderModel>(new OrderModel
            {
                Id = order.Id,
                Total = order.Total,
                Tip = order.Tip
            });
        }

        [HttpGet("getEarningForShopper")]
        public async Task<ShopperEarningResponse> GetEarningForShopper([FromQuery] string userId)
        {
            var shopperOrders = await _order.GetListAsync(x => x.ShopperId == userId);

            return new ShopperEarningResponse
            {
                TotalEarning = shopperOrders.Sum(x => x.Tip ?? 0),
                TotalOrder = shopperOrders.Count
            };
        }

        [HttpGet("getAllstoreOrders")]
        public async Task<List<OrderModel>> GetAllstoreOrders([FromQuery] string storeId)
        {
            var orders = await _order.GetListAsync(x => x.StoreId == storeId);

            var list = new List<OrderModel>();
            foreach (var order in orders)
            {
                list.Add(new OrderModel
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    StoreId = order.StoreId,
                    BasketMasterId = order.BasketMasterId,
                    ShopperId = order.ShopperId,
                    AssignedAtUtc = order.AssignedAtUtc,
                    Status = (byte)order.Status,
                    Subtotal = order.Subtotal,
                    Total = order.Total,
                });
            }

            return list;
        }
    }
}
