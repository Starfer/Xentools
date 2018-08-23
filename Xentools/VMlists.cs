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

        /// <summary>
        /// Вывод списка всех виртуальных машин.
        /// </summary>
        /// <param name="isNeedSnap"> Нужно ли выводить Snapshot'ы.</param>
        public void PrintVM(bool isNeedSnap = false)
        {
            list = VM.get_all(session);
            for (int i = 0; i < list.Count; i++)
            {
                VM a = VM.get_record(session,list[i].opaque_ref);
                if (!a.is_a_template && !a.is_a_snapshot)
                {
                    System.Console.WriteLine("{0}   {1}    {2}", i, a.name_label, a.power_state);

                    if (isNeedSnap)
                    {
                        List<XenRef<VM>> snapshots = a.snapshots;                       
                        for (int j = 0; j < snapshots.Count; j++)
                        {
                            VM b = VM.get_record(session, snapshots[j].opaque_ref).con;
                            System.Console.WriteLine("   {0}    {1}", j, b.name_label);                           
                        }
                    }
                }
            }
        }

        public XenRef<VM> ChooseVM()
        {
            PrintVM(true);
            string input = "";
            System.Console.WriteLine("[VM Number] {Snapshot Number}");
            System.Console.Write("Choose VM: ");

            input = System.Console.ReadLine();
            int choice = Convert.ToInt32(input.Split(' ')[0]);

            if (choice >= list.Count) throw new Exception("Wrong VM");

            switch (input.Split(' ').Length)
            {
                case 1: return list[choice];
                case 2:
                    {
                        int choiceSnap = Convert.ToInt32(input.Split(' ')[1]);
                        List<XenRef<VM>> snap = VM.get_record(session, list[choice].opaque_ref).snapshots;
                        if (choiceSnap >= snap.Count)
                            throw new Exception("Wrong Snapshot");
                        return snap[choiceSnap];
                    }
                default: throw new Exception("Invalid input format");
            }
        }
    }
}
