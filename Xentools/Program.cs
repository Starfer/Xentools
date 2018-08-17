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
        vmstart. Start selected VM.
        vmshutdown. Shutdown selected VM.
        disconnect
        exit
        ";

        /// <summary>
        /// Get list of VMs
        /// </summary>
        /// <param name="session"></param>
        /// <param name="snapshots">return snapshots too</param>
        /// <param name="templates">return templates too</param>
        /// <returns></returns>
        static List<VM> GetVMs(Session session, bool snapshots = false, bool templates = false)
        {
            List<VM> result = new List<VM>();
            List<XenRef<VM>> vms = new List<XenRef<VM>>();
            vms = VM.get_all(session);
            for (int i = 0; i < vms.Count; i++)
            {
                VM a = VM.get_record(session, vms[i].opaque_ref);
                if (a.is_a_snapshot && snapshots) result.Add(a);
                else if (a.is_a_template && templates) result.Add(a);
                else if (!a.is_a_template && !a.is_a_snapshot) result.Add(a);
            }
            return result;
        }

        static XenRef<VM> SelectVM(Session session)
        {
            List<XenRef<VM>> list = VM.get_all(session);
            VM a;
            for (int i = 0; i < list.Count; i++)
            {
                a = VM.get_record(session, list[i].opaque_ref);
                if (a.is_a_snapshot || a.is_a_template)
                {
                    list.RemoveAt(i);
                    i--;
                    continue;
                }
                System.Console.WriteLine("{0}   {1}", i, a.name_label);
            }
            System.Console.WriteLine("VM number: ");
            int choise = 0;
            try
            {
                choise = Convert.ToInt32(System.Console.ReadLine());
            }
            catch
            {
                //!!!
            }
            return list[choise];
        }

        static bool StartVM(Session session, XenRef<VM> vm)
        {
            bool result = true;
            try
            {
                VM.start(session, vm, false, false);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                result = false;
            }
            return result;
        }

        static bool ShutdownVM(Session session, XenRef<VM> vm)
        {
            bool result = true;
            try
            {
                VM.shutdown(session, vm);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                result = false;
            }
            return result;
        }

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
                            List<VM> vms = GetVMs(session);
                            foreach (var vm in vms)
                                System.Console.WriteLine("{0} {1}", vm.name_label, vm.power_state);
                        }
                        else
                            System.Console.WriteLine("Not connected");
                        break;
                    case "snaplist":
                        if (session != null)
                        {
                            List<VM> vms = GetVMs(session);
                            foreach (var vm in vms)
                            {
                                System.Console.WriteLine(vm.name_label);
                                List<XenRef<VM>> snaplist = vm.snapshots;
                                foreach (var snapshot in snaplist)
                                {
                                    VM snap = VM.get_record(session, snapshot.opaque_ref);
                                    System.Console.WriteLine("      {0}", snap.name_label);
                                }
                            }
                        }
                        else
                            System.Console.WriteLine("Not connected");
                        break;
                    case "vmstart":
                        if (session != null)
                        {
                            System.Console.WriteLine(StartVM(session, SelectVM(session)));
                        }
                        else
                            System.Console.WriteLine("Not connected");
                        break;
                    case "vmshutdown":
                        if (session != null)
                        {
                            System.Console.WriteLine(ShutdownVM(session, SelectVM(session)));
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
