using System;
using System.Diagnostics;
using System.Text;

namespace SystemFrameWorks
{
	/// <summary>
	/// Summary description for BaseException.
	/// </summary>
	public class BaseException : System.ApplicationException
	{
		//构造
		//无错误码
		protected BaseException(string strMessage, System.Exception oInner, int LogLevel):base(strMessage,oInner)
		{
			Dump(Format(strMessage, oInner), LogLevel);
		}

		private void Dump(string str, int loglevel)
		{
			//Trace.WriteLine(str);
			ApplicationLog.WriteError(str,loglevel);
		}
		//有错误码
		protected BaseException(string strMessage, System.Exception oInner, int LogLevel, int lLogErrorCode):base(strMessage,oInner)
		{
			Dump(Format(strMessage, oInner), LogLevel,lLogErrorCode);
		}

		private void Dump(string str, int loglevel,int lLogErrorCode)
		{
			//Trace.WriteLine(str);   //在调试和release模式下输出到侦听器
			ApplicationLog.WriteError(str,lLogErrorCode,loglevel);
		}

		public static string Format(string msg, System.Exception inner)
		{
			StringBuilder newmsg = new StringBuilder();
			string errorstack = null;
			//获取错误堆栈
			errorstack = BuildErrorStack(inner);

			newmsg.Append("SobeyRecException Summary \n")
				//.Append("---------------------------\n")
				//.Append(DateTime.Now.ToShortDateString())
				//.Append(":")
			    //.Append(DateTime.Now.ToShortTimeString())
				.Append(" - ")
				.Append(msg)
				.Append("\n\n")
				.Append(errorstack);

			return newmsg.ToString();
		}

		private static string BuildErrorStack(System.Exception chain)
		{
			string error_stack = null;
			StringBuilder stack_builder = new StringBuilder();
			int num = 1;
			System.Exception inner = null;

			if(chain != null)
			{
				stack_builder.Append("Error Stack \n").Append("-------------------------\n");

				inner = chain;

				while(inner != null)
				{
					stack_builder.Append(num).Append(") ").Append(inner.Message).Append("\n");
					inner = inner.InnerException;
					num++;
				}

				stack_builder.Append("\n---------------------\n").Append("Call Stack\n");
				stack_builder.Append(chain.StackTrace);
				error_stack = stack_builder.ToString();
			}
			else
			{
//				error_stack = "exception was not chained";
				error_stack = "";
		//		error_stack+=Environment.StackTrace;
			}

			return error_stack;
		}
	}

	public class SobeyRecException:BaseException
	{
		private SobeyRecException(string strMessage, System.Exception oInner, int LogLevel, int lLogErrorCode):base(strMessage,oInner,LogLevel,lLogErrorCode)
		{
		}

        /// <summary>
        /// 通过自定义异常代码抛出自定义异常
        /// </summary>
        /// <param name="lLogErrorCode">自定义异常代码</param>
        /// <remarks>
        /// Add by chenzhi 2013-06-04
        /// </remarks>
        public static void ThrowSelf(int lLogErrorCode)
        {
            SobeyRecException ex = new SobeyRecException(GlobalDictionary.Instance.GetMessageByCode(lLogErrorCode), null, 0, lLogErrorCode);
            throw ex;
        }

        /// <summary>
        /// 通过自定义异常代码及上层异常抛出自定义异常
        /// </summary>
        /// <param name="lLogErrorCode">自定义异常代码</param>
        /// <param name="oInner">上层异常</param>
        /// <remarks>
        /// Add by chenzhi 2013-06-18
        /// </remarks>
        public static void ThrowSelf(int lLogErrorCode, System.Exception oInner)
        {
            string strMessage = GlobalDictionary.Instance.GetMessageByCode(lLogErrorCode);
            BuildErrInfo(oInner, ref strMessage);
            SobeyRecException ex = new SobeyRecException(strMessage, oInner, 0, lLogErrorCode);
            throw ex;
        }

		public static void ThrowSelf(string strMessage, int lLogErrorCode )
		{
			SobeyRecException ex = new SobeyRecException(strMessage,null, 0, lLogErrorCode);
			throw ex;
		}
		public static void ThrowSelf(string strMessage,int LogLevel, int lLogErrorCode )
		{
			SobeyRecException ex = new SobeyRecException(strMessage,null, LogLevel, lLogErrorCode);
			throw ex;
		}
		public static void ThrowSelf(string strMessage, System.Exception oInner, int lLogErrorCode )
		{
			BuildErrInfo(oInner, ref strMessage);
			SobeyRecException ex = new SobeyRecException(strMessage,oInner, 0, lLogErrorCode);
			throw ex;
		}
		public static void ThrowSelf(string strMessage, System.Exception oInner, int LogLevel, int lLogErrorCode )
		{
			BuildErrInfo(oInner, ref strMessage);
			SobeyRecException ex = new SobeyRecException(strMessage,oInner,LogLevel,lLogErrorCode);
			throw ex;
		}
		private static void BuildErrInfo(Exception oInner, ref string strMessage)
		{
			System.Exception walker = oInner;
			while (walker!=null)
			{
				strMessage+=":";
				strMessage+=walker.Message;
				walker =  walker.InnerException;
			}
		}
	}
}
