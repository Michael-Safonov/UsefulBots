using System.Collections.Generic;

namespace FoodDeliveryBot.Alice
{
	public class AliceData
	{
		// TODO: из базы
		public static IdNameModel[] Deliveries = new[]
		{
				new IdNameModel { Id = 1, Name = "Шаурма Кинг" },
				new IdNameModel { Id = 2, Name = "Дёнер" },
		};

		public static Dictionary<int, IdNameModel[]> DeliveriesAndProducts = new Dictionary<int, IdNameModel[]>
		{
			{ 1, new [] {
				new IdNameModel { Id = 1, Name = "Кинг шавуха стандарт" },
				new IdNameModel { Id = 2, Name = "Кинг шавуха большая" }
				}},

			{ 2, new [] {
				new IdNameModel { Id = 3, Name = "Дёнер шавуха стандарт" },
				new IdNameModel { Id = 4, Name = "Дёнер шавуха большая" }
				}}
		};
	}
}
