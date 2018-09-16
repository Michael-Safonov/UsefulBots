using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace FoodDeliveryBot.SMS
{
    public class SmsSender
    {
        private const string accountSid = "AC7c68e960dbab7fb611c9b9ff1b465d19";
        private string authToken = "96f512aa0934a3cbf8936e4e504e6f9d";

        public MessageResource.StatusEnum SendSms(string phone, string message)
        {
            TwilioClient.Init(accountSid, authToken);

            var messageResourse = MessageResource.Create(
                body: message, 
                from: new Twilio.Types.PhoneNumber("+15592061811"), 
                to: new Twilio.Types.PhoneNumber(phone));
            return messageResourse.Status;
        }
    }
}
