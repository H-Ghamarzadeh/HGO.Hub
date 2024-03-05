namespace HGO.Hub.Test.Services
{
    public interface IEmailService
    {
        Task SendEmail(string to, string subject, string body);
    }
    public class EmailService: IEmailService
    {
        public async Task SendEmail(string to, string subject, string body)
        {
            await Task.Delay(10000);
        }
    }
}
