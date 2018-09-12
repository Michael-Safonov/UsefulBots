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
	}
}
