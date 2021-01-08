

namespace OrleansNotifyPlugin
{
    using IngestDBCore;
    using IngestDBCore.Notify;
    using IngestTask.Abstraction.Grains;
    using Orleans;
    using Sobey.Core.Log;
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class OrleansNotify : SubNotify
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("OrleansNotify");
       
        public IClusterClient Client { get; }
        public OrleansNotify(IClusterClient client)
        {
            Client = client;
        }

        //static public string ADDTASK { get { return "TASK_ADD"; } }
        //static public string MODTASK { get { return "TASK_MOD"; } }
        //static public string DELTASK { get { return "TASK_DEL"; } }
        //static public string MLCRT { get { return "ML_CRT"; } }
        //static public string MLCPT { get { return "ML_CPT"; } }
        //static public string BACKUP { get { return "BACKUP"; } }
        public override void ActionNotify(object theClock, NotifyArgs ti)
        {
            //发送通知
            if ((ti.Intent & NotifyPlugin.Orleans) > 0)
            {
                switch (ti.Type)
                {
                    case GlobalStateName.ADDTASK:
                        {
                            var grain = Client.GetGrain<IDispatcherGrain>(0);
                            try
                            {
                                //grain.AddTaskAsync();
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e.Message);
                            }
                        } break;
                    case GlobalStateName.MODTASK:
                        { }
                        break;
                    case GlobalStateName.DELTASK:
                        { }
                        break;
                    case GlobalStateName.MLCRT:
                        { }
                        break;
                    case GlobalStateName.MLCPT:
                        { }
                        break;
                    case GlobalStateName.BACKUP:
                        { }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
