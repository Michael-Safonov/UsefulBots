using System.Threading.Tasks;
using FoodDeliveryBot.Models;
using LiteDB;

namespace FoodDeliveryBot.Repositories
{
    public class OrderSessionRepository : BaseRepository<OrderSession>
    {
        public OrderSessionRepository(string collectionName) : base(collectionName)
        {
        }

        public override async Task<BsonValue> Insert(OrderSession entity)
        {
            var result = await base.Insert(entity);

            collection.EnsureIndex(e => e.OrderSessionId);
            collection.EnsureIndex(e => e.Pincode);

            return result;
        }

        public override async Task<bool> Upsert(OrderSession entity)
        {
            var result = await base.Upsert(entity);

            collection.EnsureIndex(e => e.OrderSessionId);
            collection.EnsureIndex(e => e.Pincode);

            return result;
        }

        public async Task<OrderSession> GetByPinCode(string pinCode)
        {
            return await Task.FromResult(base.collection.FindOne(s => s.Pincode == pinCode));
        }
    }
}