using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBot.Models.ViewModels
{
	public class ProductCreateModel
	{
		[Required]
		public int DeliveryId { get; set; }

		[Required]
		public string Name { get; set; }

		public string Description { get; set; }

		[Required]
		public decimal Price { get; set; }
	}
}
