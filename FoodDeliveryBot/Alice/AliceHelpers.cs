using System;

namespace FoodDeliveryBot.Alice
{
	public static class AliceHelpers
	{
		private static Random random = new Random();

		public static string CreateOrderCode()
		{
			return random.Next(0, 9999).ToString("0000");
		}
	}
}
