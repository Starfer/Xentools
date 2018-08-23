using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenAPI;

namespace Xentools
{
    static class Snapshot
    {
        public static bool Create(Session session, XenRef<VM> vm)
        {
            string newName;
            System.Console.Write("Enter name of new snapshot: ");
            newName = System.Console.ReadLine();
            bool result = true;
            try
            {
                VM.snapshot(session, vm, newName);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                result = false;
            }
            return result;
        }

        public static bool Revert(Session session, XenRef<VM> vm)
        {
            bool result = true;
            try
            {
                VM.revert(session, vm);
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
