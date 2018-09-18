using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FoodDeliveryBot.Models;
using LiteDB;

namespace FoodDeliveryBot.Repositories
{
    public class UserOrderRepository : BaseRepository<UserOrder>
    {
        public UserOrderRepository() : base("UserOrders")
        {
        }

        public override async Task<BsonValue> Insert(UserOrder entity)
        {
            var result = await base.Insert(entity);

            collection.EnsureIndex(e => e.UserId);

            return result;
        }

        public override async Task<bool> Upsert(UserOrder entity)
        {
            var result = await base.Upsert(entity);

            collection.EnsureIndex(e => e.UserId);

            return result;
        }

        public async Task<IEnumerable<UserOrder>> GetBySessionId(Guid sessionId)
        {
            return await Task.FromResult(collection.Find(uo => uo.SessionId == sessionId));
        }
    }
}