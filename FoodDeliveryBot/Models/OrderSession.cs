using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBot.Models
{
	public class OrderSession
	{
		public Guid OrderSessionId { get; set; }
		public int Pincode { get; set; }
		public bool IsCompleted  { get; set; }
		public DeliveryService DeliveryService { get; set; }
		public Guid OwnerUser { get; set; }

		public List<UserOrder> UserOrders { get; set; }

	}
}
