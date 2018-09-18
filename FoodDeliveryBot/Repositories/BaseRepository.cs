namespace FoodDeliveryBot.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LiteDB;

    using FoodDeliveryBot.Db;

    public class BaseRepository<TEntity>
    {
        protected readonly LiteCollection<TEntity> collection;

        public BaseRepository(string collectionName)
        {
            this.collection = DbManager.Instance.GetCollection<TEntity>(collectionName);
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await Task.Run(() => collection.FindAll());
        }

        public async Task<TEntity> Get(BsonValue id)
        {
            return await Task.Run(() => collection.FindById(id));
        }

        public virtual async Task<BsonValue> Insert(TEntity entity)
        {
            return await Task.Run(() => collection.Insert(entity));
        }

        public async Task<bool> Update(TEntity entity)
        {
            return await Task.Run(() => collection.Update(entity));
        }

        public virtual async Task<bool> Upsert(TEntity entity)
        {
            return await Task.Run(() => collection.Upsert(entity));
        }

        public async Task<bool> Delete(BsonValue id)
        {
            return await Task.Run(() => collection.Delete(id));
        }
    }
}