namespace FoodDeliveryBot.Repositories
{
    using System.Threading.Tasks;
    using LiteDB;

    using FoodDeliveryBot.Models;

    public class DeliveryServiceInfoRepository : BaseRepository<DeliveryService>
    {
        public DeliveryServiceInfoRepository(string collectionName) : base(collectionName)
        {
        }

        public override async Task<BsonValue> Insert(DeliveryService entity)
        {
            var result = await base.Insert(entity);

            collection.EnsureIndex(e => e.Id);
            collection.EnsureIndex(e => e.Name);

            return result;
        }

        public override async Task<bool> Upsert(DeliveryService entity)
        {
            var result = await base.Upsert(entity);

            collection.EnsureIndex(e => e.Id);
            collection.EnsureIndex(e => e.Name);

            return result;
        }
    }
}