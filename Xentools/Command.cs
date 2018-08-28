using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenAPI;

namespace Xentools
{
    class Command
    {
        public static bool Update(Session session, XenRef<VM> vm)
        {
            VM v_m = VM.get_record(session, vm.opaque_ref);
            VIF vif;
            string mac, ip, user, password;

            for (int i = 0; i < v_m.VIFs.Count; i++)
            {
                vif = VIF.get_record(session, v_m.VIFs[i]);
                mac = vif.MAC.Replace(':', '-');
                ip = Get_IP_by_MAC(mac);

                System.Console.WriteLine("VM name \"" + v_m.name_label + "\" . mac: " + mac + " . ip: " + ip);
                System.Console.Write("User (admin): ");
                user = System.Console.ReadLine();
                System.Console.Write("Password: ");
                password = Connect.getPassword();

                string cmd = "psexec \\\\" + ip + "-u " + user + " -p " + password + " cmd";
            }


            return true;
        }


        static string Get_IP_by_MAC(string mac)
        {
            string cmd = "/C arp -a | find /i \"" + mac + "\"";
            string output = SendToCMD(cmd);
            try
            {
                string[] arr = output.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                output = ""; int i = 0;
                do
                    output = arr[i++];
                while (!Is_IPv4(output) && i < arr.Length);
            }
            catch(Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            return output;
        }

        static bool Is_IPv4(string ip)
        {
            bool result = true; int octet;
            string[] check = ip.Split('.');
            if (check.Length != 4) result = false;
            for (int i = 0; i < check.Length && result; i++)
            {
                octet = Convert.ToInt32(check[i]);
                if (octet < 0 || octet > 255) result = false;
            }
            return result;
        }

        static string SendToCMD (string command)
        {
            System.Diagnostics.ProcessStartInfo psiOpt = new System.Diagnostics.ProcessStartInfo(@"cmd.exe", @command);
            psiOpt.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psiOpt.RedirectStandardOutput = true;
            psiOpt.UseShellExecute = false;
            psiOpt.CreateNoWindow = true;
            System.Diagnostics.Process procCommand = System.Diagnostics.Process.Start(psiOpt);

            string output = procCommand.StandardOutput.ReadToEnd();
            procCommand.WaitForExit();
            return output;
        }

    }
}
