

namespace OrleansNotifyPlugin
{
    using IngestDBCore;
    using IngestDBCore.Notify;
    using IngestDBCore.Tool;
    using IngestTask.Abstraction.Grains;
    using IngestTask.Dto;
    using Orleans;
    using Sobey.Core.Log;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    public class OrleansNotify : ISubNotify
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("OrleansNotify");

        public IClusterClient Client { get; }
        private string[] excludeNotifyAction = { NotifyAction.MODIFYTASKNAME, NotifyAction.MODIFYTASKSTATE};
        public OrleansNotify(IClusterClient client)
        {
            Client = client;
            if (client == null)
            {
                Logger.Error("ingesttask cofig error");
            }
        }

        public void ActionNotify(object theClock, NotifyArgs ti)
        {
            //发送通知
            if ((ti.Intent & NotifyPlugin.Orleans) > 0 && !excludeNotifyAction.Contains(ti.Action))
            {
                try
                {
                    if (Client != null && !Client.IsInitialized)
                    {
                        Client.Connect().Wait();
                    }

                }
                catch (Exception e)
                {
                    Logger.Error("client connect error" + e.Message);
                    return;
                }

                switch (ti.Type)
                {
                    case GlobalStateName.ADDTASK:
                        {
                            var grain = Client.GetGrain<IDispatcherGrain>(0);
                            try
                            {
                                DispatchTask task = new DispatchTask();
                                ObjectTool.CopyObjectData(ti.Param, task, "", BindingFlags.Public | BindingFlags.Instance);
                                AutoRetry.RunSync(() => grain.AddTaskAsync(task).Wait());
                            }
                            catch (Exception e)
                            {
                                Logger.Error("ADDTASK" + e.Message);
                            }
                        }
                        break;
                    case GlobalStateName.MODTASK:
                        {
                            var grain = Client.GetGrain<IDispatcherGrain>(0);
                            try
                            {
                                DispatchTask task = new DispatchTask();
                                ObjectTool.CopyObjectData(ti.Param, task, "", BindingFlags.Public | BindingFlags.Instance);
                                AutoRetry.RunSync(() => grain.UpdateTaskAsync(task).Wait());
                                
                            }
                            catch (Exception e)
                            {
                                Logger.Error("MODTASK" + e.Message);
                            }
                        }
                        break;
                    case GlobalStateName.DELTASK:
                        {
                            var grain = Client.GetGrain<IDispatcherGrain>(0);
                            try
                            {
                                DispatchTask task = new DispatchTask();
                                ObjectTool.CopyObjectData(ti.Param, task, "", BindingFlags.Public | BindingFlags.Instance);
                                AutoRetry.RunSync(() => grain.DeleteTaskAsync(task).Wait());
                                
                            }
                            catch (Exception e)
                            {
                                Logger.Error("DELTASK" + e.Message);
                            }
                        }
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
                    case GlobalStateName.INCHANGE:
                        {}
                        break;
                    case GlobalStateName.INDELETE:
                        { }
                        break;
                    case GlobalStateName.OUTCHANGE:
                        { }
                        break;
                    case GlobalStateName.OUTDELETE:
                        { }
                        break;
                    case GlobalStateName.CHANNELCHANGE:
                        {
                            var grain = Client.GetGrain<IDeviceInspections>(0);
                            try
                            {
                                AutoRetry.RunSync(() => grain.NotifyDeviceChangeAsync().Wait());
                            }
                            catch (Exception e)
                            {
                                Logger.Error("CHANNELCHANGE" + e.Message);
                            }
                        }
                        break;
                    case GlobalStateName.CHANNELDELETE:
                        {
                            var grain = Client.GetGrain<IDeviceInspections>(0);
                            try
                            {
                                AutoRetry.RunSync(() => grain.NotifyChannelDeleteAsync((int)ti.Param).Wait());
                            }
                            catch (Exception e)
                            {
                                Logger.Error("CHANNELDELETE" + e.Message);
                            }
                        }
                        break;
                    case GlobalStateName.DEVICECHANGE:
                        {
                            var grain = Client.GetGrain<IDeviceInspections>(0);
                            try
                            {
                                AutoRetry.RunSync(() => grain.NotifyDeviceDeleteAsync((int)ti.Param).Wait());
                            }
                            catch (Exception e)
                            {
                                Logger.Error("DEVICECHANGE" + e.Message);
                            }
                        }
                        break;
                    case GlobalStateName.DEVICEDELETE:
                        {
                            var grain = Client.GetGrain<IDeviceInspections>(0);
                            try
                            {
                                AutoRetry.RunSync(() => grain.NotifyDeviceChangeAsync().Wait());
                            }
                            catch (Exception e)
                            {
                                Logger.Error("DEVICEDELETE" + e.Message);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
