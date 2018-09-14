using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryBot.Controllers
{
	public class AliceController : Controller
	{
		[HttpGet]
		public IActionResult GetResponse()
		{
			return Json(new
			{
				Id = 123,
				Name = "item123"
			});
		}

		[HttpPost]
		public IActionResult PostRequest([FromBody]SomeRequest request)
		{
			return Content($"You posted: {request.Name}");
		}

		public class SomeRequest
		{
			public string Name { get; set; }
		}
	}
}
