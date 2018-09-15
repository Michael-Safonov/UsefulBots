using System.Collections.Concurrent;
using FoodDeliveryBot.Alice.Domain;

namespace FoodDeliveryBot.Alice
{
	public static class AlicePersistence
	{
		public static readonly ConcurrentDictionary<string, AbstractAliceDialog> CurrentDialogs
			= new ConcurrentDictionary<string, AbstractAliceDialog>();

		public static ConcurrentDictionary<string, AliceOrder> UserOrders
			= new ConcurrentDictionary<string, AliceOrder>();
	}
}
