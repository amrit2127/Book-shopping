using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Types;
using Twilio;
using Microsoft.Extensions.Options;
using Twilio.Rest.Api.V2010.Account;

namespace Project_2_Ecomm_116.Utility
{
    public class SMSsender : ISmsSender
    {
        private TwilioSettings _twilioSettings { get; }
        public SMSsender(IOptions<TwilioSettings> twilioSettings)
        {
            _twilioSettings = twilioSettings.Value;
        }

        public Task SendSMSAsync(string phoneNumber, string SMSmessage)
        {
            Execute(phoneNumber, SMSmessage).Wait();
            return Task.FromResult(0);
        }
        public async Task Execute(string phoneNumber, string SMS)
        {
            try
            {  // Find your Account Sid and Auth Token at twilio.com/user/account

                TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
                var message = await MessageResource.CreateAsync(
                    to: new PhoneNumber("+91" + phoneNumber),
                    from: new PhoneNumber(_twilioSettings.From), //  From number, must be an SMS-enabled Twilio number ( This will send sms from ur "To" numbers ).
                    body: SMS);
            }

            catch (Exception ex)
            {
                string Str = ex.Message;
                Console.Write(Str);

            }
        }
    }
}
