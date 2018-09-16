namespace FoodDeliveryBot.DataStubs
{
	public class DeliveryStub
	{
		public string Name { get; set; }

		public string Url { get; set; }

		public ProductStub[] Products { get; set; }
	}

	public class ProductStub
	{
		public string Name { get; set; }

		public int Price { get; set; }

		public string Description { get; set; }
	}
}
