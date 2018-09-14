using FoodDeliveryBot.Extensions;
using FoodDeliveryBot.Models.AliceModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryBot.Controllers
{
	public class AliceController : Controller
	{
		[HttpPost]
		public AliceResponse WebHook([FromBody] AliceRequest req)
		{
			return req.Reply("Привет! Я FoodBoy. Хочешь заказать покушать?");
		}
	}
}
