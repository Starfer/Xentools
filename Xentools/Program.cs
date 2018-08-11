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
        connect [ip]. Connect to XEN Server. Port is default (443)
        vmlist. Get list of virtual machines.
        disconnect
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
                            if (command.Split(' ').Length > 2)
                                session = new Session(command.Split(' ')[1], int.Parse(command.Split(' ')[2]));
                            else
                                session = new Session(command.Split(' ')[1], 443);
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
                    case "disconnect":
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
                            }
                        else
                            System.Console.WriteLine("Not connected");
                        break;
                    case "vmlist":
                        if (session != null)
                        {
                            List<XenRef<VM>> vms = new List<XenRef<VM>>();
                            vms = VM.get_all(session);
                            for (int i = 0; i < vms.Count; i++)
                            {
                                VM a = VM.get_record(session, vms[i].opaque_ref);
                                if (!a.is_a_snapshot && !a.is_a_template)
                                    System.Console.WriteLine("{0}",i,a.name_label);
                            }
                        }
                        else
                            System.Console.WriteLine("Not connected");
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
