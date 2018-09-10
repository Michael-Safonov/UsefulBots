using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace FoodDeliveryBot.Models
{
	//todo: модели разбить
	public class UserInfo
	{
		public OrderInfo Order { get; set; }
		public List<Product> OrderedProducts { get; set; }
	}
	public class OrderInfo
	{
		public string NotificationId { get; set; }
		//todo: Потом поменять на guid
		public string OrderId { get; set; }
	}
	public class ConversationInfo : Dictionary<string, object> { }
	public class Product
	{
		public string Name { get; set; }
		public string Desciption { get; set; }
		public decimal Price { get; set; }
	}
}