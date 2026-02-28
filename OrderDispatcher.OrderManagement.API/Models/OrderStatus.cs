namespace OrderDispatcher.OrderManagement.API.Models
{
    public enum OrderStatus
    {
        Created = 0,
        Broadcasted = 1,
        Assigned = 2,
        Cancelled = 3,
        Completed = 4
    }
}
