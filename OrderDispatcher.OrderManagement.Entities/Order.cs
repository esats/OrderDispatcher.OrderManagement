using OrderDispatcher.OrderManagement.Core.Entities;

namespace OrderDispatcher.OrderManagement.Entities
{
    public class Order : EntityBase<long>
    {
        public string CustomerId { get; set; }
        public string StoreId { get; set; }
        public string? ShopperId { get; set; }
        public int BasketMasterId { get; set; }
        public DateTime? AssignedAtUtc { get; set; }
        public int Status { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? DeliveryFee { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? Tip { get; set; }
        public decimal? Total { get; set; }
        public string? Notes { get; set; }
        public bool IsFinished  { get; set; } = false;
    }
}
