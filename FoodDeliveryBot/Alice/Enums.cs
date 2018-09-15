
namespace FoodDeliveryBot.Alice
{
	public enum DialogType
	{
		Initial, // диалог "Новый заказ" - "Существующий заказ"
		ChooseDelivery,
		EnterExistingOrderId,
		ChooseActionOnOrder, // выберите продукты, см. статистику, отменить заказ
		ChooseProducts, // "Шава1", "Шава2", "Отмена", "Завершить"
		Print, // фейковый диалог для просто выдачи текстового сообщения и перехода к след. диалогу
		GetOrderKey, // получить ключ существующего
	}

	public enum ButtonType
	{
		NewOrder,
		ExistingOrder,
		ClickOnDelivery, // клик по доставке (магазину)
		ChooseProduct, // Выберите продукты
		ClickOnProduct, // клик по продукту
		SeeMyOrder, // посмотреть мой заказ
		EndMyOrder, // завершить мой заказ
	}
}
