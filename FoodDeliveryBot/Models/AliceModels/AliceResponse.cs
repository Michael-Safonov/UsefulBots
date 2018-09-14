using Newtonsoft.Json;

namespace FoodDeliveryBot.Models.AliceModels
{
	public class AliceResponse
	{
		[JsonProperty("response")]
		public AliceResponseModel Response { get; set; }

		[JsonProperty("session")]
		public AliceSessionModel Session { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; } = "1.0";
	}
}
