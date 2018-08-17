using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenAPI;

namespace Xentools
{
    class VMlists
    {
        List<XenRef<VM>> list;
        Session session;
        public VMlists()
        {
            list = new List<XenRef<VM>>();
        }
        public VMlists(Session session)
        {
            list = VM.get_all(session);
            this.session = session;
        }
        public void PrintVM()
        {
            list = VM.get_all(session);
            for (int i = 0; i < list.Count; i++)
            {
                VM a = VM.get_record(session,list[i].opaque_ref);
                if (!a.is_a_template && !a.is_a_snapshot)
                {
                    System.Console.WriteLine("{0}   {1}    {2}", i, a.name_label, a.power_state);
                }
            }
        }
        public void PrintVMSnapshots()
        {
            list = VM.get_all(session);
            for (int i = 0; i < list.Count; i++)
            {
                VM a = VM.get_record(session, list[i].opaque_ref);
                if (a.is_a_template && !a.is_a_snapshot)
                {
                    List<XenRef<VM>> snapshots = a.snapshots;
                    System.Console.WriteLine("{0}   {1}    {2}", i, a.name_label, a.power_state);
                    for (int j = 0; j < snapshots.Count; j++)
                    {
                        VM b = VM.get_record(session, list[i].opaque_ref);
                        System.Console.WriteLine("{1}    {2}",j, a.name_label);
                    }
                }
            }
        }
        public XenRef<VM> ChooseVM()
        {
            PrintVM();
            int choise = 0;
            System.Console.WriteLine("Choose VM: ");
            try
            {
                choise = int.Parse(System.Console.ReadLine());
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            if (choise < list.Count) return list[choise];
            else throw new Exception("Wrong VM");
        }
    }
}
