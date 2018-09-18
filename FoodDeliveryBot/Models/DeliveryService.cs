using System.Collections.Generic;

namespace FoodDeliveryBot.Models
{
    public class DeliveryService
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public List<Product> Products { get; set; } = new List<Product>();

		//todo: add work time and additional information for delivery service
	}
}
