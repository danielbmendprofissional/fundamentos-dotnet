namespace Solid.DIP.Bad
{
    internal class Bad
    {
        internal Bad()
        {
            var emailService = new EmailService();
            var usuario = new UserService(emailService);

            /*
              var emailServiceTest = new EmailServiceTest();
              var usuarioTest = new UserService(emailServiceTest);
            */
            // Now the user is aware of the concrete class, and this will break in the test email scenario.
        }
    }

    public class EmailService
    {
        public void Send()
        {
            Console.WriteLine("Sent.");
        }
    }

    public class EmailServiceTest
    {
        public void Send()
        {
            Console.WriteLine("Test email sent.");
        }
    }

    public class UserService(EmailService emailService) { }
}