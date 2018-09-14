using Newtonsoft.Json;

namespace FoodDeliveryBot.Models.AliceModels
{
	public class AliceButtonModel
	{
		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("payload")]
		public object Payload { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("hide")]
		public bool Hide { get; set; }
	}
}
