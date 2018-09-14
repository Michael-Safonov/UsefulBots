namespace FoodDeliveryBot.Repositories
{
    using System.Threading.Tasks;
    using LiteDB;

    using FoodDeliveryBot.Models;

    public class ProductRepository : BaseRepository<Product>
    {
        public ProductRepository(string collectionName) : base(collectionName)
        {
        }

        public override async Task<BsonValue> Insert(Product entity)
        {
            var result = await base.Insert(entity);

            collection.EnsureIndex(e => e.Name);

            return result;
        }

        public override async Task<bool> Upsert(Product entity)
        {
            var result = await base.Upsert(entity);

            collection.EnsureIndex(e => e.Name);

            return result;
        }
    }
}