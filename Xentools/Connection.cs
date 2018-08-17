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
            System.Console.WriteLine("Login: ");
            login = System.Console.ReadLine();
            System.Console.WriteLine("Password: ");
            password = System.Console.ReadLine();
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
    }
}
