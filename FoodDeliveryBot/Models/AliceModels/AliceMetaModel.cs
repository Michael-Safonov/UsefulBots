using Newtonsoft.Json;

namespace FoodDeliveryBot.Models.AliceModels
{
	public class AliceMetaModel
	{
		[JsonProperty("locale")]
		public string Locale { get; set; }

		[JsonProperty("timezone")]
		public string Timezone { get; set; }

		[JsonProperty("client_id")]
		public string ClientId { get; set; }
	}
}
