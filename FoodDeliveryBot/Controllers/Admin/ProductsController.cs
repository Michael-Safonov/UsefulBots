using System.Threading.Tasks;
using FoodDeliveryBot.Models;
using FoodDeliveryBot.Models.ViewModels;
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

		[HttpGet]
		public IActionResult Create(int deliveryId)
		{
			var model = new ProductCreateModel
			{
				DeliveryId = deliveryId
			};
			
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Create(ProductCreateModel model)
		{
			if (ModelState.IsValid)
			{
				var newProduct = new Product
				{
					Name = model.Name,
					Desciption = model.Description,
					Price = model.Price
				};

				var delivery = await _deliveryServiceRepository.GetById(model.DeliveryId);
				delivery.Products.Add(newProduct);

				await _deliveryServiceRepository.Update(delivery);

				return RedirectToAction("GetById", "Deliveries", new { id = delivery.Id });
			}

			return View();
		}
	}
}