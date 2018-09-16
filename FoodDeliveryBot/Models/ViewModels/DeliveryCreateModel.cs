using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryBot.Models.ViewModels
{
	public class DeliveryCreateModel
	{
		[Required]
		public string Name { get; set; }
	}
}
