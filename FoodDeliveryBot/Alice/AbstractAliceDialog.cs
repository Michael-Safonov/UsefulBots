namespace FoodDeliveryBot.Alice
{
	/// <summary>
	/// Один вопрос юзеру.
	/// </summary>
	public abstract class AbstractAliceDialog
	{
		public abstract DialogType DialogType();

		public abstract string Title();

		public virtual AliceButton[] Buttons { get; set; }

		/// <summary>
		/// При нажатии кнопки или отправке текста в диалоге.
		/// </summary>
		///<param name="command">Отправляется, если юзер ответил текстом.</param>
		///<param name="pressedButton">Отправляется, если юзер нажал кнопку.</param>
		public abstract AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null);
	}
}
