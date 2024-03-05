namespace HGO.Hub.Test.Services
{
    public interface ISmsService
    {
        Task SendSms(string to, string text);
    }
    public class SmsService : ISmsService
    {
        public async Task SendSms(string to, string text)
        {
            await Task.Delay(10000);
        }
    }
}
