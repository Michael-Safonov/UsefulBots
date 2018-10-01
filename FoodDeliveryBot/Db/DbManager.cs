using System.Threading.Tasks;
using LiteDB;

namespace FoodDeliveryBot.Db
{
	public class DbManager
	{
		// todo: to appsettings.json
		private const string ConnectionString = "BotDb.db";
		public static LiteDatabase Instance { get; private set; }

		static DbManager()
		{
			Instance = new LiteDatabase(ConnectionString);
		}

	    public static async Task<bool> DropCollection(string collectionName)
	    {
	        return await Task.Run(() => Instance.DropCollection(collectionName));
	    }
	}
}
