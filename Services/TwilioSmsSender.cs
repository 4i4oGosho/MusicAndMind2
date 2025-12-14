using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace MusicAndMind2.Services
{
    public class TwilioSmsSender : ISmsSender
    {
        private readonly SmsSettings _settings;

        public TwilioSmsSender(IOptions<SmsSettings> options)
        {
            _settings = options.Value;
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
        }

        public async Task SendAsync(string to, string message)
        {
            await MessageResource.CreateAsync(
                to: new Twilio.Types.PhoneNumber(to),
                from: new Twilio.Types.PhoneNumber(_settings.FromNumber),
                body: message
            );
        }
    }
}
