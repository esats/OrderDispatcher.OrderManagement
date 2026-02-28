using System.Collections.Generic;
using OrderDispatcher.CatalogService.Entities;
using OrderDispatcher.OrderManagement.Entities;

namespace OrderDispatcher.OrderManagement.API.Models
{
    public class BasketSaveResponse
    {
        public int BasketMasterId { get; set; }
        public int BasketDetailId { get; set; }
    }

    public class BasketGetOneResponse
    {
        public string UserId { get; set; }
        public string StoreId { get; set; }
        public int BasketMasterId { get; set; }
        public int DeliveryAddressId { get; set; }
        public List<BasketDetail> Items { get; set; } = new();
    }

    public class BasketGetAllResponse
    {
        public string UserId { get; set; }
        public List<BasketSummaryResponse> Baskets { get; set; } = new();
    }

    public class BasketSummaryResponse
    {
        public int BasketMasterId { get; set; }
        public string StoreId { get; set; }
        public int DeliveryAddressId { get; set; }
        public int ProductCount { get; set; }
    }
}
