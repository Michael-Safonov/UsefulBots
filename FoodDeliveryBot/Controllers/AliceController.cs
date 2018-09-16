using System.Linq;
using FoodDeliveryBot.Alice;
using FoodDeliveryBot.Alice.AliceDialogs;
using FoodDeliveryBot.Extensions;
using FoodDeliveryBot.Models.AliceModels;
using Microsoft.AspNetCore.Mvc;

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
		public AliceResponse WebHook([FromBody]AliceRequest req)
		{
			var userId = req.Session.UserId;
			AliceResponse GoToStart()
			{
				var dialog = new InitialDialog();
				// приветствие
				AlicePersistence.CurrentDialogs[userId] = dialog;

				var response = ConvertToAliceResponse(dialog, req);
				response.Response.Text = response.Response.Tts = "Привет, пользователь! Что хочешь?";

				return response;
			}

			if (req.Session.New) // новый пользователь
			{
				return GoToStart();
			}

			if (!AlicePersistence.CurrentDialogs.TryGetValue(userId, out var currentDialog))
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
				if (nextDialog.NoAnswer)
				{
					// просто берём текст этого диалога
					// и выполняем его Action
					var dialogText = nextDialog.Title;
					nextDialog = nextDialog.Action();
					nextDialog.Title = $"{dialogText}\n{nextDialog.Title}";
				}

				// TODO: make method dialog => alice response
				var aliceResponse = ConvertToAliceResponse(nextDialog, req);

				AlicePersistence.CurrentDialogs[userId] = nextDialog;

				return aliceResponse;
			}
			else if (req.Request.Type == AliceRequestType.SimpleUtterance)
			{
				// ну пока это запрос на код заказа
				// todo: сделать универсальный механизм
				if (!(currentDialog is GetOrderKeyDialog))
				{
					var printDialog = new PrintDialog
					{
						Text = "Данная команда не найдена"
					};

					return ConvertToAliceResponse(printDialog, req);
				}

				var nextDialog = currentDialog.Action(command: req.Request.Command);

				// TODO: make method dialog => alice response
				var aliceResponse = ConvertToAliceResponse(nextDialog, req);

				AlicePersistence.CurrentDialogs[userId] = nextDialog;

				return aliceResponse;
			}

			return req.Reply("Привет! Я FoodBoy. Хочешь заказать покушать?");
		}

		private AliceResponse ConvertToAliceResponse(AbstractAliceDialog dialog, AliceRequest req)
		{
			var aliceButtons = dialog.Buttons.Select(b => new AliceButtonModel
			{
				Title = b.Title,
				Payload = b.Payload
			}).ToArray();

			return req.Reply(dialog.Title, buttons: aliceButtons);
		}
	}
}
