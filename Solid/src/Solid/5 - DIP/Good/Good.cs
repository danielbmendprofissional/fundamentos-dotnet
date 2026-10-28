namespace Solid.DIP.Good
{
    internal class Good
    {
        internal Good()
        {
            var emailService = new EmailService();
            var usuario = new UserService(emailService);

            var emailServiceTest = new EmailServiceTest();
            var usuarioTest = new UserService(emailServiceTest);

            // The User only knows it will receive an email service through the contract,
            // it doesn’t need to know anything about the concrete class.
        }
    }

    public interface IEmailService
    {
        void Send();
    }

    public class EmailService : IEmailService
    {
        public void Send()
        {
            Console.WriteLine("Sent.");
        }
    }

    public class EmailServiceTest : IEmailService
    {
        public void Send()
        {
            Console.WriteLine("Test email sent.");
        }
    }

    public class UserService(IEmailService emailService) { }
}