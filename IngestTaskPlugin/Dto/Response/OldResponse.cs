using System;
using System.Collections.Generic;
using System.Text;

namespace IngestTaskPlugin.Dto
{
    public class OldResponse
    {
    }
    public enum MetaDataType
    {
        /// <summary>
        /// 采集元数据 = 0
        /// </summary>
		emCapatureMetaData = 0,
        /// <summary>
        /// 存储元数据 = 1
        /// </summary>
		emStoreMetaData,
        /// <summary>
        /// 任务上下文关联元数据 = 2
        /// </summary>
		emContentMetaData,
        /// <summary>
        /// Plan元数据 = 3
        /// </summary>
        emPlanMetaData,
        /// <summary>
        /// 编目信息元数据 = 4
        /// </summary>
		emCatalogueMetaData,
        /// <summary>
        /// 审查打回意见 = 5
        /// </summary>
        emCheckPassBackData,
        /// <summary>
        /// MSV本地素材GUID = 6
        /// </summary>
        emAmfsData,
        /// <summary>
        ///  = 7
        /// </summary>
        emOriginalMetaData,
        emSplitData
    }
    public class GetQueryTaskMetaData_param
    {
        public string MetaData;
        public string errStr;
        public bool bRet;
    }
}
