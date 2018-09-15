using System.Collections.Generic;

namespace FoodDeliveryBot.Alice.Domain
{
	public class AliceOrder
	{
		public List<IdNameModel> Products { get; set; } = new List<IdNameModel>();

		public bool IsCompleted { get; set; }
	}
}
