using Microsoft.AspNetCore.Mvc;
using OrderDispatcher.CatalogService.Entities;
using OrderDispatcher.OrderManagement.API.Base;
using OrderDispatcher.OrderManagement.API.Models;
using OrderDispatcher.OrderManagement.Dal.Abstract;
using OrderDispatcher.OrderManagement.Entities;
using System.Linq;

namespace OrderDispatcher.OrderManagement.API.Controllers
{
    [Route("api/order-management/basket")]
    [Produces("application/json")]
    public class BasketController : APIControllerBase
    {
        private readonly IBasketMaster _basketMaster;
        private readonly IBasketDetail _basketDetail;

        public BasketController(IBasketMaster basketMaster, IBasketDetail basketDetail)
        {
            _basketMaster = basketMaster;
            _basketDetail = basketDetail;
        }

        [HttpPost("save")]
        public async Task<Response<BasketSaveResponse>> Save([FromBody] BasketSaveRequest model)
        {
            if (model == null)
            {
                return new Response<BasketSaveResponse>(false, "Request body is required.", new BasketSaveResponse());
            }

            var basketMaster = await _basketMaster.GetTAsync(x => x.UserId == model.UserId && x.StoreId == model.StoreId && x.IsActive);
            if (basketMaster == null)
            {
                basketMaster = new BasketMaster
                {
                    UserId = model.UserId,
                    StoreId = model.StoreId,
                    DeliveryAddressId = model.DeliveryAddressId,
                    CreatedBy = base.GetUser()
                };

                basketMaster = await _basketMaster.AddAsync(basketMaster);
            }

            var basketDetail = await _basketDetail.GetTAsync(x => x.BasketMasterId == basketMaster.Id && x.ProductId == model.ProductId);

            if (basketDetail == null)
            {
                basketDetail = new BasketDetail
                {
                    BasketMasterId = basketMaster.Id,
                    ProductId = model.ProductId,
                    ProductPrice = model.ProductPrice,
                    Quantity = model.Quantity,
                    UnitType = model.UnitType,
                    Weight = model.Weight,
                    CreatedBy = base.GetUser()
                };

                basketDetail = await _basketDetail.AddAsync(basketDetail);
            }
            else
            {
                basketDetail.ProductPrice = model.ProductPrice;
                basketDetail.Quantity = model.Quantity;
                basketDetail.UnitType = model.UnitType;
                basketDetail.Weight = model.Weight;

                await _basketDetail.UpdateAsync(basketDetail);
            }

            return new Response<BasketSaveResponse>(new BasketSaveResponse
            {
                BasketMasterId = basketMaster.Id,
                BasketDetailId = basketDetail.Id
            });
        }

        [HttpGet("getOne")]
        public async Task<BasketGetOneResponse> GetOne([FromQuery] string userId, [FromQuery] string storeId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(storeId))
            {
                return null;
            }

            var master = await _basketMaster.GetTAsync(x => x.UserId == userId && x.StoreId == storeId && x.IsActive);

            if (master == null)
            {
                return null;
            }

            var details = await _basketDetail.GetListAsync(x => x.BasketMasterId == master.Id);

            return (new BasketGetOneResponse
            {
                BasketMasterId = master.Id,
                StoreId = storeId,
                UserId = userId,
                DeliveryAddressId = master.DeliveryAddressId,
                Items = details
            });
        }

        [HttpGet("getAll")]
        public async Task<BasketGetAllResponse> GetAll()
        {
            var userId = base.GetUser();

            var masters = await _basketMaster.GetListAsync(x => x.UserId == userId && x.IsActive);
            if (masters == null || masters.Count == 0)
            {
                return new BasketGetAllResponse
                {
                    UserId = userId
                };
            }

            var masterIds = masters.Select(x => x.Id).ToList();
            var details = await _basketDetail.GetListAsync(x => masterIds.Contains(x.BasketMasterId));

            var productCounts = details
                .GroupBy(x => x.BasketMasterId)
                .ToDictionary(x => x.Key, x => x.Sum(d => d.Quantity));

            var response = new BasketGetAllResponse
            {
                UserId = userId
            };

            foreach (var master in masters)
            {
                response.Baskets.Add(new BasketSummaryResponse
                {
                    BasketMasterId = master.Id,
                    StoreId = master.StoreId,
                    DeliveryAddressId = master.DeliveryAddressId,
                    ProductCount = productCounts.TryGetValue(master.Id, out var count) ? count : 0
                });
            }

            return response;
        }
    }
}
