using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FoodDeliveryBot.Models.AliceModels
{
	public class AliceRequestModel
	{
		[JsonProperty("command")]
		public string Command { get; set; }

		[JsonProperty("type")]
		public AliceRequestType Type { get; set; }

		[JsonProperty("original_utterance")]
		public string OriginalUtterance { get; set; }

		[JsonProperty("payload")]
		public JObject Payload { get; set; }
	}
}
