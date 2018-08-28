using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenAPI;

namespace Xentools
{
    static class Connect
    {
        public static bool Connection(ref Session session, string ip, int port = 443)
        {
            session = new Session(ip,port);
            string login, password;
            System.Console.Write("Login: ");
            login = System.Console.ReadLine();
            System.Console.Write("Password: ");
            password = getPassword();
            try
            {
                session.login_with_password(login, password);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        public static bool Disconnection(Session session)
        {
            if (session != null)
                try
                {
                    session.logout();
                    session = null;
                    System.Console.WriteLine("Disconnected");
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                    return false;
                }
            else return false;
            return true;
        }

        public static string getPassword()
        {
            string password = "";
            while (true)
            {
                var key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                System.Console.Write("*");
                password += key.KeyChar;
            }
            System.Console.WriteLine();
            return password;
        }
    }
}
