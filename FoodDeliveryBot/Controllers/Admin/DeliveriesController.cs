using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryBot.Models;
using FoodDeliveryBot.Models.ViewModels;
using FoodDeliveryBot.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryBot.Controllers.Admin
{
	public class DeliveriesController : Controller
	{
		private readonly DeliveryServiceRepository _deliveryServiceRepository;

		public DeliveriesController(DeliveryServiceRepository deliveryServiceRepository)
		{
			_deliveryServiceRepository = deliveryServiceRepository;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var deliveries = (await _deliveryServiceRepository.GetAll()).ToArray();

			return View(deliveries);
		}

		[HttpGet]
		public async Task<IActionResult> GetById(int id)
		{
			var delivery = await _deliveryServiceRepository.GetById(id);

			return View(delivery);
		}

		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(DeliveryCreateModel model)
		{
			if (ModelState.IsValid)
			{
				var newDelivery = new DeliveryService
				{
					Name = model.Name,
				};

				await _deliveryServiceRepository.Upsert(newDelivery);

				return RedirectToAction("Index", "Deliveries");
			}

			return View();
		}
	}
}
