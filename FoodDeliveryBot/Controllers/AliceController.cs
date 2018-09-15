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

				var dictionaryJson = JsonConvert.SerializeObject(CurrentDialogs.Select(cd => new { cd.Key, cd.Value }));
				Log.Debug($"CurrentDialogs: {dictionaryJson}");

				return req.Reply($"Привет, пользователь! Что хочешь?",
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

				return req.Reply(dialog.Title, buttons: aliceButtons);
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
				if (nextDialog.NoAnswer)
				{
					// просто берём текст этого диалога
					// и выполняем его Action
					var dialogText = nextDialog.Title;
					//Log.Debug($"dialogText: {dialogText}");
					nextDialog = nextDialog.Action();
					//var nextDialogTitle = nextDialog.Title;
					nextDialog.Title = $"{dialogText}\n{nextDialog.Title}";
					//Log.Debug($"nextDialog.Title: {nextDialog.Title}");
					//var initDialog = new InitialDialog();
					//initDialog.Title = "abc";//$"{dialogText} {initDialog.Title}";
					//Log.Debug($"initDialog.Title: {initDialog.Title}");
					//nextDialog = initDialog;
				}

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
