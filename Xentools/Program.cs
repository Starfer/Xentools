using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenAPI;
using System.Net;

namespace Xentools
{
    class Program
    {
        const string GetHelp = 
        @"
        connect [ip] [port]. Connect to XEN Server. ip - destination ip address, port - destination port
        logout
        exit
        ";
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Session session = null;
            System.Console.WriteLine("Type \"help\" for a list of commands");
            while (true)
            {
                string command = System.Console.ReadLine();
                string action = command.Split(' ')[0];
                switch (action)
                {
                    case "help":
                        System.Console.WriteLine(GetHelp);
                        break;
                    case "connect":
                        string login, password;
                        try
                        {
                            session = new Session(command.Split(' ')[1], int.Parse(command.Split(' ')[2]));
                        }
                        catch(Exception ex)
                        {
                            System.Console.WriteLine(ex.Message);
                            break;
                        }
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
                            break;
                        }
                        System.Console.WriteLine("Connected!");
                        break;
                    case "logout":
                        if (session != null)
                            try
                            {
                                session.logout();
                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine(ex.Message);
                                break;
                            }
                        break;
                    case "exit":
                        if (session != null)
                            session.logout();
                        return;
                }
            }
        }
    }
}
