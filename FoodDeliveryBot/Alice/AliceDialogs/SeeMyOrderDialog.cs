namespace FoodDeliveryBot.Alice.AliceDialogs
{
	public class SeeMyOrderDialog : AbstractAliceDialog
	{
		public override bool NoAnswer { get => base.NoAnswer; set => base.NoAnswer = value; }

		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null) => throw new System.NotImplementedException();

		public override DialogType DialogType() => throw new System.NotImplementedException();
	}
}
