using System.Collections.Generic;

namespace OrderDispatcher.OrderManagement.API.Models
{
    public class OrderSaveResponse
    {
        public long OrderId { get; set; }
        public string OrderNumber { get; set; } = default!;
    }

    public class OrderModel
    {
        public long Id { get; set; }
        public string CustomerId { get; set; }
        public string StoreId { get; set; }
        public string? ShopperId { get; set; }
        public int BasketMasterId { get; set; }
        public DateTime? AssignedAtUtc { get; set; }
        public byte Status { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? DeliveryFee { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? Tip { get; set; }
        public decimal? Total { get; set; }
        public string? Notes { get; set; }
    }

    public class OrderGetOneResponse
    {
        public OrderModel? Order { get; set; }
    }

    public class OrderGetAllResponse
    {
        public string CustomerId { get; set; }
        public List<OrderModel> Orders { get; set; } = new();
    }

    public class OrderAssignModel
    {
        public long OrderId { get; set; }
        public string ShopperId { get; set; }
    }

    public class ShopperEarningResponse
    {
        public decimal TotalEarning { get; set; }
        public int TotalOrder { get; set; }
    }
}
