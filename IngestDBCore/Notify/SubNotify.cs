using System;
using System.Collections.Generic;
using System.Text;

namespace IngestDBCore.Notify
{

    public static class NotifyPlugin
    {
        public const int Kafka = 0x01;
        public const int Msv = 0x10;
        public const int Orleans = 0x100;
        public const int Udp = 0x1000;
        public const int NotifyTask = Kafka | Orleans;
        public const int All = Kafka | Msv | Orleans;
    }
    public static class NotifyAction
    {
        public const string STOPTASK = "STOPTASK";
        public const string STOPGROUPTASK = "STOPGROUPTASK";
        public const string DELETETASK = "DELETETASK";
        public const string DELETEGROUPTASK = "DELETEGROUPTASK";
        public const string ADDTASK = "ADDTASK";
        public const string MODIFYTASK = "MODIFYTASK";
        public const string MODIFYCOOPERTASK = "MODIFYCOOPERTASK";
        public const string MODIFYPERIODCTASK = "MODIFYPERIODCTASK";
        public const string MODIFYTASKCOOPTYPE = "MODIFYTASKCOOPTYPE";
        public const string MODIFYTASKNAME = "MODIFYTASKNAME";
        public const string MODIFYTASKSTARTTIME = "MODIFYTASKSTARTTIME";
        public const string MODIFYTASKSTATE = "MODIFYTASKSTATE";


        public const string MSVRELOCATE = "MSVRELOCATE";
        public const string CREATEPERIODICTASK = "CREATEPERIODICTASK";
    }

    public static class IngestCmd
    {
        public const int StartCapture = 0x01;
        public const int StopCapture = 0x10;
        public const int CutClip = 0x100;
        public const int HaveSendBMP = 0x1000;
        public const int ClipFinish = 0x10000;
    }

    public interface ISubNotify
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
        public void ActionNotify(
         object theClock, NotifyArgs ti)
        {
            
        }
    }
}
