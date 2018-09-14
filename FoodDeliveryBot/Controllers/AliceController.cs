using FoodDeliveryBot.Extensions;
using FoodDeliveryBot.Models.AliceModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FoodDeliveryBot.Controllers
{
	public class AliceController : Controller
	{
		/// <summary>
		/// Этот метод получает сообщения от Алисы.
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[HttpPost]
		public AliceResponse WebHook([FromBody] AliceRequest req)
		{
			if (req.Request.Type == AliceRequestType.SimpleUtterance)
			{
				return req.Reply("Жми на кнопку, кожаный ублюдок",
						buttons: new[] {
							new AliceButtonModel {
								Title = "Надпись1",
								Payload = new SimplePayload
								{
									Id = 1,
									Name = "Payload1"
								}
							},
							new AliceButtonModel {
								Title = "Надпись2",
								Payload = new SimplePayload
								{
									Id = 2,
									Name = "Payload2"
								}
							}
						});
			}
			else if (req.Request.Type == AliceRequestType.ButtonPressed)
			{
				var payload = req.Request.Payload?.ToObject<SimplePayload>();

				return req.Reply($"Payload: {JsonConvert.SerializeObject(payload)}, Command: {req.Request.Command}");
			}
			
			return req.Reply("Привет! Я FoodBoy. Хочешь заказать покушать?");
		}
	}

	public class SimplePayload
	{
		public int Id { get; set; }

		public string Name { get; set; }
	}
}
