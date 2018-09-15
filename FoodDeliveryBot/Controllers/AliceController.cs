using System.Collections.Concurrent;
using System.Linq;
using FoodDeliveryBot.Alice;
using FoodDeliveryBot.Extensions;
using FoodDeliveryBot.Models.AliceModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace FoodDeliveryBot.Controllers
{
	public class AliceController : Controller
	{
		private static readonly ConcurrentDictionary<string, AbstractAliceDialog> CurrentDialogs 
			= new ConcurrentDictionary<string, AbstractAliceDialog>();

		/// <summary>
		/// Этот метод получает сообщения от Алисы.
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[HttpPost]
		public AliceResponse WebHook([FromBody] AliceRequest req)
		{
			var userId = req.Session.UserId;
			AliceResponse GoToStart() {
				var dialog = new InitialDialog();
				// приветствие
				CurrentDialogs.GetOrAdd(userId, dialog);

				var aliceButtons = dialog.Buttons.Select(b => new AliceButtonModel
				{
					Title = b.Title,
					Payload = b.Payload
				}).ToArray();

				return req.Reply($"Привет, пользователь! Вот, что у нас в dictionary: [{string.Join(',', CurrentDialogs.Select(c => c.Key))}]. Что хочешь?",
					buttons: aliceButtons);
			}

			AliceResponse ConvertToAliceResponse(AbstractAliceDialog dialog)
			{
				Log.Debug($"Dialog: {JsonConvert.SerializeObject(dialog)}");
				var aliceButtons = dialog.Buttons.Select(b => new AliceButtonModel
				{
					Title = b.Title,
					Payload = b.Payload
				}).ToArray();

				return req.Reply(dialog.Title(), buttons: aliceButtons);
			}

			if (req.Session.New) // новый пользователь
			{
				return GoToStart();
			}

			if (!CurrentDialogs.TryGetValue(userId, out var currentDialog))
			{
				return GoToStart();
			}

			if (req.Request.Type == AliceRequestType.ButtonPressed)
			{
				var payload = req.Request.Payload.ToObject<AliceButtonPayloadModel>();
				var clickedButton = new AliceButton
				{
					DialogType = currentDialog.DialogType(),
					Payload = payload
				};

				// TODO: send AliceButtonPayloadModel to .Action instead of AliceButton
				var nextDialog = currentDialog.Action(clickedButton);

				// TODO: make method dialog => alice response
				var aliceResponse = ConvertToAliceResponse(nextDialog);

				CurrentDialogs[userId] = nextDialog;

				return aliceResponse;
			}
			else if (req.Request.Type == AliceRequestType.SimpleUtterance)
			{
				return req.Reply("А вот это науке пока неизвестно!");
			}
			
			return req.Reply("Привет! Я FoodBoy. Хочешь заказать покушать?");
		}
	}
}
