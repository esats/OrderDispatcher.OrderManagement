namespace OrderDispatcher.OrderManagement.API.Models
{
    public class BasketSaveRequest
    {
        public string UserId { get; set; }
        public string StoreId { get; set; }
        public int DeliveryAddressId { get; set; }
        public int ProductId { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public int UnitType { get; set; }
        public decimal Weight { get; set; } = 0;
    }
}
