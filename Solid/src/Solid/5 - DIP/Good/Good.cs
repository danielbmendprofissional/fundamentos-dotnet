using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            //O Usuario só sabe que vai receber um e-mail service pelo contrato, não precisa saber nada da classe concreta
        }
    }

    public interface IEmailService
    {
        public void Send();
    }

    public class EmailService : IEmailService 
    { 
        public void Send()
        {
            Console.WriteLine("Enviado.");
        }
    }

    public class EmailServiceTest : IEmailService
    {
        public void Send()
        {
            Console.WriteLine("Teste enviado."); 
        }
    }

    public class UserService(IEmailService emailService) { }
}
