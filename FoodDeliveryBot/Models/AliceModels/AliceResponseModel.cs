using Newtonsoft.Json;

namespace FoodDeliveryBot.Models.AliceModels
{
	public class AliceResponseModel
	{
		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("tts")]
		public string Tts { get; set; }

		[JsonProperty("end_session")]
		public bool EndSession { get; set; }

		[JsonProperty("buttons")]
		public AliceButtonModel[] Buttons { get; set; }
	}
}
