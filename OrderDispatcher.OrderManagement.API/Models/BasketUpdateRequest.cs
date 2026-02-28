namespace OrderDispatcher.OrderManagement.API.Models
{
    public class BasketUpdateRequest
    {
        public string UserId { get; set; }
        public string StoreId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UnitType { get; set; }
        public decimal Weight { get; set; } = 0;
    }
}
