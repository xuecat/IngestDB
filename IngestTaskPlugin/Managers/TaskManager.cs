using AutoMapper;
using IngestDBCore;
using IngestTaskPlugin.Dto;
using IngestTaskPlugin.Stores;
using Sobey.Core.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IngestTaskPlugin.Managers
{
    public class TaskManager
    {
        public TaskManager(ITaskStore store, IMapper mapper)
        {
            Store = store;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected ITaskStore Store { get; }
        protected IMapper _mapper { get; }
        private readonly ILogger Logger = LoggerManager.GetLogger("TaskInfo");

        public async virtual Task<TResult> GetTaskMetadataAsync<TResult>( int taskid, int ntype)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a.Where(b => b.Taskid == taskid && b.Metadatatype == ntype));
            return _mapper.Map<TResult>(f);
        }

        public async Task StopGroupTask(int taskid)
        {
            var f = await Store.GetTaskMetaDataAsync(a => a.Where(b => b.Taskid == taskid && b.Metadatatype ==(int)MetaDataType.emContentMetaData));

            try
            {
                XDocument xd = new XDocument();
                xd = XDocument.Load(f.Metadatalong);

                var content = xd.Element("TaskContentMetaData");
                int groupcolor = int.Parse(content.Element("GroupColor").Value);

                var groupitems = content.Element("GroupItems").Elements();

                var channellist = groupitems.Select(l => int.Parse(l.Element("ItemData").Value));


            }
            catch (Exception e)
            {
                SobeyRecException.ThrowSelfNoParam(taskid.ToString(), GlobalDictionary.GLOBALDICT_CODE_FILL_GETTASKMETADATA_EXCEPTION, Logger, e);
            }
        }
    }
}
