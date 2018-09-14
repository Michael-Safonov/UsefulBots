using FoodDeliveryBot.Extensions;
using FoodDeliveryBot.Models.AliceModels;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryBot.Controllers
{
	public class AliceController : Controller
	{
		[HttpPost("/alice")]
		public AliceResponse WebHook([FromBody] AliceRequest req)
		{
			return req.Reply("Привет");
		}
	}
}
