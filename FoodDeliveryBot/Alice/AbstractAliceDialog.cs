namespace FoodDeliveryBot.Alice
{
	/// <summary>
	/// Один вопрос юзеру.
	/// </summary>
	public abstract class AbstractAliceDialog
	{
		public abstract DialogType DialogType();

		public virtual string Title { get; set; }

		public virtual AliceButton[] Buttons { get; set; } = new AliceButton[0];

		/// <summary>
		/// При нажатии кнопки или отправке текста в диалоге.
		/// </summary>
		///<param name="command">Отправляется, если юзер ответил текстом.</param>
		///<param name="pressedButton">Отправляется, если юзер нажал кнопку.</param>
		public abstract AbstractAliceDialog Action(AliceButton pressedButton = null, string command = null);

		/// <summary>
		/// Если на диалог не нужен ответ (например, просто печатает текст).
		/// </summary>
		public virtual bool NoAnswer { get; set; } = false;
	}
}
