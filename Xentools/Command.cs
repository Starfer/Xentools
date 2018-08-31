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
            string startpath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Utilities\\";
            if (!System.IO.File.Exists(startpath + "psexec.exe"))
                throw new Exception("File psexec.exe is not exist in " + startpath);

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

                System.Diagnostics.ProcessStartInfo psiOpt = new System.Diagnostics.ProcessStartInfo(@"cmd.exe");

                psiOpt.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psiOpt.RedirectStandardOutput = true;
                psiOpt.RedirectStandardInput = true;
                psiOpt.UseShellExecute = false;
                psiOpt.CreateNoWindow = true;

                System.Diagnostics.Process procCommand = System.Diagnostics.Process.Start(psiOpt);
                //wuauclt /ResetAuthorization /DetectNow
                //wuauclt /UpdateNow

                //psexec \\192.168.1.67 -u test -p "password-1" cmd /c "ipconfig /all >c:\users\test\desktop\ip2.txt &ipconfig >c:\users\test\desktop\ip3.txt"
                string startRemoteCMD = "/K " + startpath + "psexec \\\\" + ip + " -u " + user + " -p \"" + password + "\" cmd ";
                //string remoteUpdateWindows = "/C \"" + "" + "\"";
                //string remoteUpdateWindows = "/C \"" + "ipconfig /all >c:\\users\\" + user + "\\desktop\\ip2.txt &ipconfig >c:\\users\\" + user + "\\desktop\\ip3.txt" + "\"";
                //string remoteUpdateWindows = "\"/C " + "ipconfig" +  "\"";               
                string remoteUpdateWindows = "/C " + "ipconfig /all >c:\\users\\" + user + "\\desktop\\ip2.txt";
                string fffff = "/c " + startpath + "psexec.exe \\\\" + ip + " -u " + user + " -p \"" + password + "\" cmd /c \"ipconfig /all >c:\\users\\" + user + "\\desktop\\ip2.txt && exit\"";
                System.IO.StreamWriter In = procCommand.StandardInput;
                System.Console.WriteLine(@fffff);
                In.WriteLine(@fffff);
                System.IO.StreamWriter OUT = new System.IO.StreamWriter(@"D:\\log2.txt");               
                OUT.WriteLine( procCommand.StandardOutput.ReadToEnd());
                procCommand.WaitForExit();
                OUT.Close();

                
                //In.WriteLine(startRemoteCMD);
                //In.WriteLine(remoteUpdateWindows);
                //In.WriteLine("exit");

                //System.Console.WriteLine(startRemoteCMD + remoteUpdateWindows);
                //System.Console.WriteLine(SendToCMD(startRemoteCMD + remoteUpdateWindows, true));              
            }


            return true;
        }


        static string Get_IP_by_MAC(string mac)
        {
            string cmd = "/C arp -a | find /i \"" + mac + "\"";
            string output = SendToCMD(cmd, true);
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

        static string SendToCMD(string command, bool isNeedOutput = false)
        {
            System.Diagnostics.ProcessStartInfo psiOpt = new System.Diagnostics.ProcessStartInfo(@"cmd.exe");
            psiOpt.Arguments = @command;
            psiOpt.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psiOpt.RedirectStandardOutput = isNeedOutput;
            psiOpt.UseShellExecute = false;
            psiOpt.CreateNoWindow = true;         

            System.Diagnostics.Process procCommand = System.Diagnostics.Process.Start(psiOpt);

            string output = "";
            if (isNeedOutput) output = procCommand.StandardOutput.ReadToEnd();
            procCommand.WaitForExit();
            return output;
        }

        static System.IO.StreamWriter SendToCMD()
        {
            System.Diagnostics.ProcessStartInfo psiOpt = new System.Diagnostics.ProcessStartInfo(@"cmd.exe");

            psiOpt.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            psiOpt.RedirectStandardInput = true;
            psiOpt.UseShellExecute = false;
            psiOpt.CreateNoWindow = true;

            System.Diagnostics.Process procCommand = System.Diagnostics.Process.Start(psiOpt);

            return procCommand.StandardInput;
        }

    }
}
