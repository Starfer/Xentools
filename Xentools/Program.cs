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
        snaplist. Get list of all snapshots.
        snapcreate. Create new snapshot.
        snaprevert. Revert VM to previous snaphot.
        vmstart. Start selected VM.
        vmshutdown. Shutdown selected VM.
        disconnect
        exit
        ";

        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            Session session = null;
            VMlists vmlist = new VMlists();
            if (args.Count() > 0)
                Connect.Connection(ref session, args[0]);
            System.Console.WriteLine("Type \"help\" for a list of commands");
            while (true)
            {
                bool result = true;
                string command = System.Console.ReadLine();
                string action = command.Split(' ')[0];
                switch (action)
                {
                    case "help":
                        System.Console.WriteLine(GetHelp);
                        break;
                    case "connect":
                        if (command.Split(' ').Length > 2)
                            result = Connect.Connection(ref session, command.Split(' ')[1], Convert.ToInt32(command.Split(' ')[2]));
                        else
                            result = Connect.Connection(ref session, command.Split(' ')[1]);
                        if (result)
                        {
                            System.Console.WriteLine("Connected!");
                            vmlist = new VMlists(session);
                        }
                        break;
                    case "disconnect":
                        result = Connect.Disconnection(session);
                        if (!result)
                            System.Console.WriteLine("Not connected");
                        break;
                    case "vmlist":
                        if (session != null)
                            vmlist.PrintVM();
                        else
                            System.Console.WriteLine("Not connected");
                        break;
                    case "snaplist":
                        if (session != null)
                        {
                            vmlist.PrintVM(true);
                        }
                        else
                            System.Console.WriteLine("Not connected");
                        break;
                    case "snapcreate":
                        if (session != null)
                        {
                            if (Snapshot.Create(session, vmlist.ChooseVM()))
                                System.Console.WriteLine("Succesful.");
                        }
                        else
                            System.Console.WriteLine("Not connected");
                        break;
                    case "snaprevert":
                        if (session != null)
                        {
                            if (Snapshot.Revert(session, vmlist.ChooseVM()))
                                System.Console.WriteLine("Succesful.");
                        }
                        else
                            System.Console.WriteLine("Not connected");
                        break;
                    case "vmstart":
                        if (session != null)
                        {
                            OnOff.StartVM(session, vmlist.ChooseVM());
                        }
                        else
                            System.Console.WriteLine("Not connected");
                        break;
                    case "vmshutdown":
                        if (session != null)
                        {
                            OnOff.ShutdownVM(session, vmlist.ChooseVM());
                        }
                        else
                            System.Console.WriteLine("Not connected");
                        break;
                    case "update":
                        if (session != null)
                        {
                            Command.Update(session, vmlist.ChooseVM());
                        }
                        else
                            System.Console.WriteLine("Not connected");
                        break;
                    case "exit":
                        if (session != null)
                            session.logout();
                        return;
                    default:
                        System.Console.WriteLine("Unknown command");
                        break;
                }
            }
        }
    }
}
