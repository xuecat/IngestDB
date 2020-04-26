using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MsvClientSDK
{
    //SDI信号源的详细信息
    public class SDISignalDetails
    {
        public int nSDIFormat;
        public int nDFMode;
        public int nWidth;
        public int nHeight;
        public float fFrameRate;
        public bool bIsBlack;
        public SDISignalDetails()
        {
            nSDIFormat = -1;
            nDFMode = 2;
            nWidth = 0;
            nHeight = 0;
            fFrameRate = 0.0F;
            bIsBlack = false;
        }

    }
    public class TaskParam
	{
		public int taskID;                          //任务ID
		public int cutLen;                          //任务分段长度
		public int inOutCount;                      //上载时多出和入点计数
		public int[] inFrame = new int[100];       //多出入点开始贞数组
		public int[] outFrame = new int[100];      //多出入点结束贞数组
		public int[] totalFrame = new int[100];    //任务总帧数数组(防止出点找不到的情况)
		public string taskName;                 //任务名称
		public bool isRetro;                        //是否是回溯任务
		public int retroFrame;                      //回溯的帧数
													// TODO: 为MSV的计划任务模式传递任务开始时间
		public DateTime tmBeg;                      //任务采集开始时间
		public bool isPlanMode;                 //是否为计划任务

		public TaskParam()
		{
			taskID = 0;
			cutLen = 0;
			inOutCount = 0;
			isRetro = false;
			retroFrame = 0;
			tmBeg = DateTime.Now; // 当前时间
			isPlanMode = false;             // 不是计划任务模式，默认值
		}
		~TaskParam()
		{
		}

	}
	//public struct _MSVC_TASK_PARAM
	//{
	//	public int bRetrospect;
	//	public int bUseTime;
	//	public int channel;
	//	public int dwCutClipFrame;
	//	public int[] dwInFrame;
	//	public int[] dwOutFrame;
	//	public int[] dwTotalFrame;
	//	public int nInOutCount;
	//	public int nSignalID;
	//	public int nTimeCode;
	//	public int TaskMode;
	//	public sbyte[] TaskName;
	//	public int TaskState;
	//	public int ulID;
	//}

	public class _ErrorInfo
	{
        public string errStr = "";
        public string getErrorInfo()
        {
            return errStr;
        }
	}




}
