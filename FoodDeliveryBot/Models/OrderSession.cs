using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDeliveryBot.Models
{
	public class OrderSession
	{
		public Guid OrderSessionId { get; set; }
		public string Pincode { get; set; }
		public bool IsCompleted  { get; set; }
		public DeliveryService DeliveryService { get; set; }
		public string OwnerUserId { get; set; }
	}
}
