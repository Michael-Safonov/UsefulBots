using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBot.Models;
using LiteDB;

namespace FoodDeliveryBot.Repositories
{
	public class DeliveryServiceRepository : BaseRepository<DeliveryService>
	{
		public DeliveryServiceRepository(string collectionName) : base(collectionName)
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

		public async Task<DeliveryService> GetByName(string name)
		{
			return await Task.FromResult(collection.FindOne(s => s.Name == name));
		}

		public async Task<DeliveryService> GetById(int id)
		{
			return await Task.FromResult(collection.FindById(id));
		}
	}
}