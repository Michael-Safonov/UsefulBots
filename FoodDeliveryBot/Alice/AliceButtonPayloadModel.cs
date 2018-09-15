namespace FoodDeliveryBot.Alice
{
	/// <summary>
	/// Payload кнопки, отдаваемый Алисе. 
	/// Его Алиса вернёт нам при нажатии на кнопку.
	/// </summary>
	public class AliceButtonPayloadModel
	{
		public IdNameModel Data { get; set; }

		public ButtonType Type { get; set; }
	}
}
