using Newtonsoft.Json;

namespace FoodDeliveryBot.Models.AliceModels
{
	public class AliceRequest
	{
		[JsonProperty("meta")]
		public AliceMetaModel Meta { get; set; }

		[JsonProperty("request")]
		public AliceRequestModel Request { get; set; }

		[JsonProperty("session")]
		public AliceSessionModel Session { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }
	}
}
