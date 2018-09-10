using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBot.Models
{
	public class OrderInfo
	{
		public string NotificationId { get; set; }
		//todo: Потом поменять на guid
		public string OrderId { get; set; }
	}
}
