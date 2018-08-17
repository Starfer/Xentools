using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenAPI;

namespace Xentools
{
    static class OnOff
    {
        public static bool StartVM(Session session, XenRef<VM> vm)
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

        public  static bool ShutdownVM(Session session, XenRef<VM> vm)
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
    }
}
