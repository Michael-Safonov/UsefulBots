using System.Threading.Tasks;
using FoodDeliveryBot.Models;
using LiteDB;

namespace FoodDeliveryBot.Repositories
{
    public class OrderInfoRepository : BaseRepository<OrderInfo>
    {
        public OrderInfoRepository(string collectionName) : base(collectionName)
        {
        }

        public override async Task<BsonValue> Insert(OrderInfo entity)
        {
            var result = await base.Insert(entity);

            collection.EnsureIndex(e => e.OrderId);
            collection.EnsureIndex(e => e.NotificationId);

            return result;
        }

        public override async Task<bool> Upsert(OrderInfo entity)
        {
            var result = await base.Upsert(entity);

            collection.EnsureIndex(e => e.OrderId);
            collection.EnsureIndex(e => e.NotificationId);

            return result;
        }
    }
}