using Serilog;

namespace FoodDeliveryBot.Alice.AliceDialogs
{
	/// <summary>
	/// Диалог для печати чего-нибудь.
	/// </summary>
	public class PrintDialog : AbstractAliceDialog
	{
		public override bool NoAnswer => true;

		public string Text { get; set; }

		public override DialogType DialogType() => Alice.DialogType.Print;

		public override string Title => Text;

		public override AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null)
		{
			//Log.Debug("PrintDialog");
			//Log.Debug($"Title:{Title}");
			// todo: м.б. настроить возвращение в любое место
			return new InitialDialog();
		}
	}
}
