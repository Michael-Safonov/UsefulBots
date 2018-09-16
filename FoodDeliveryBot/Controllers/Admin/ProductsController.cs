using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBot.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryBot.Controllers.Admin
{
	public class ProductsController : Controller
	{
		private readonly DeliveryServiceRepository _deliveryServiceRepository;

		public ProductsController(DeliveryServiceRepository deliveryServiceRepository)
		{
			_deliveryServiceRepository = deliveryServiceRepository;
		}

		//[HttpGet]
		//public async Task<IActionResult> GetByDelivery(int deliveryId)
		//{
		//	var delivery = await _deliveryServiceRepository.GetById(deliveryId);
		//	var products = delivery.Range;

		//	return View(products);
		//}
	}
}