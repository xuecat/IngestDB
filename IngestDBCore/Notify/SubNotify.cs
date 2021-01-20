using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore.Notify
{

    public class NotifyPlugin
    {
        static public int Kafka { get { return 0x01; } }
        static public int Msv { get { return 0x10; } }
        static public int Orleans { get { return 0x100; } }
        static public int Udp { get { return 0x1000; } }
        static public int All { get { return Kafka | Msv | Orleans; } }
    }
    public class NotifyAction
    {
        static public string STOPTASK { get { return "STOPTASK"; } }
        static public string STOPGROUPTASK { get { return "STOPGROUPTASK"; } }
        static public string DELETETASK { get { return "DELETETASK"; } }
        static public string DELETEGROUPTASK { get { return "DELETEGROUPTASK"; } }
        static public string ADDTASK { get { return "ADDTASK"; } }
        static public string MODIFYTASK { get { return "MODIFYTASK"; } }
        static public string MODIFYCOOPERTASK { get { return "MODIFYCOOPERTASK"; } }
        static public string MODIFYPERIODCTASK { get { return "MODIFYPERIODCTASK"; } }
        static public string MODIFYTASKCOOPTYPE { get { return "MODIFYTASKCOOPTYPE"; } }
        static public string MODIFYTASKNAME { get { return "MODIFYTASKNAME"; } }
        static public string MODIFYTASKSTARTTIME { get { return "MODIFYTASKSTARTTIME"; } }
        static public string MODIFYTASKSTATE { get { return "MODIFYTASKSTATE"; } }


        static public string MSVRELOCATE { get { return "MSVRELOCATE"; } }

        static public string CREATEPERIODICTASK { get { return "CREATEPERIODICTASK"; } }
    }

    public class SubNotify
    {
        public void Subscribe(NotifyClock theClock)
        {
            theClock.NotifyChange +=
               new NotifyClock.NotifyChangeHandler(ActionNotify);
        }
        public T Subscribe<T>(NotifyClock theClock)
        {
            theClock.NotifyChange +=
               new NotifyClock.NotifyChangeHandler(ActionNotify);

            return (T)(object)this;
        }
        public virtual void ActionNotify(
         object theClock, NotifyArgs ti)
        {
            
        }
    }
}
