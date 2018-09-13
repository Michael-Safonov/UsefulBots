using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBot.Models
{
	public class UserInfo
	{
		public OrderInfo Order { get; set; }
		public DeliveryServiceInfo OrderDeliveryService { get; set; }
		public List<Product> OrderedProducts { get; set; }
	}
}