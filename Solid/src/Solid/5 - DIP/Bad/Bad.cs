using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
         //Agora o usuário está sabendo da classe concreta e agora ~vai quebrar no email de teste.
        }
    }

    public class EmailService
    {
        public void Send()
        {
            Console.WriteLine("Enviado.");
        }
    }

    public class EmailServiceTest
    {
        public void Send()
        {
            Console.WriteLine("Teste enviado.");
        }
    }

    public class UserService(EmailService emailService) { }
}
