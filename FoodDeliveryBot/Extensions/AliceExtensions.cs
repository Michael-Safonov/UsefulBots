using FoodDeliveryBot.Models.AliceModels;

namespace FoodDeliveryBot.Extensions
{
	public static class AliceExtensions
	{
		public static AliceResponse Reply(
		this AliceRequest req,
		string text,
		bool endSession = false,
		AliceButtonModel[] buttons = null) => new AliceResponse
		{
			Response = new AliceResponseModel
			{
				Text = text,
				Tts = text,
				EndSession = endSession,
				Buttons = buttons
			},
			Session = req.Session
		};
	}
}
